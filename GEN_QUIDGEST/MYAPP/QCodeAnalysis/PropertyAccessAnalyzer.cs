using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Threading;
using System.Linq;

namespace QCodeAnalysis
{
    /// <summary>
    /// A diagnostic analyzer for C# that identifies direct property assignments outside of their intended context within ViewModels.
    /// Such assignments need to be reviewed because the assigned values might not be persisted during the form's save operation.
    /// If altering the value is necessary and it must be persisted, an alternative approach should be considered,
    /// or the field should be explicitly marked as blocked or hidden in the form's definition.
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class PropertyAccessAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "QUID001";
        private static readonly LocalizableString Title = "Review Direct Property Access";
        private static readonly LocalizableString MessageFormat = "Direct assignment to property '{0}' may not persist. Review or modify form definition.";
        private static readonly LocalizableString Description = "Direct property assignments should be reviewed to ensure data persistence. The value assigned to the field might not be persisted during the form's save operation.";
        private const string Category = "Usage";

        /// <summary>
        /// The rule for reporting the diagnostic.
        /// </summary>
        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(
            DiagnosticId,
            Title,
            MessageFormat,
            Category,
            DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            description: Description);

        /// <summary>
        /// Returns an immutable array of diagnostics that this analyzer is capable of producing.
        /// </summary>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        /// <summary>
        /// Initializes the analyzer and registers an action to analyze simple assignment expressions.
        /// </summary>
        /// <param name="context">Analysis context.</param>
        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();

            // Register an action to analyze direct property assignments.
            context.RegisterSemanticModelAction(AnalyzeAssignment);
        }

        /// <summary>
        /// Analyzes assignment expressions to identify direct access to properties that require review due to potential persistence issues.
        /// </summary>
        /// <param name="context">Semantic model analysis context.</param>
        private void AnalyzeAssignment(SemanticModelAnalysisContext context)
        {
            var semanticModel = context.SemanticModel;
            var rootNode = semanticModel.SyntaxTree.GetRoot(context.CancellationToken);

            // Perform a single scan on all relevant nodes
            var assignments = rootNode.DescendantNodes()
                .OfType<AssignmentExpressionSyntax>()
                .Where(n => n.Left is MemberAccessExpressionSyntax);

            foreach (var assignment in assignments)
            {
                var memberAccess = (MemberAccessExpressionSyntax)assignment.Left;
                IPropertySymbol propertySymbol = semanticModel.GetSymbolInfo(memberAccess, context.CancellationToken).Symbol as IPropertySymbol;

                // Check if the left-hand side of the assignment is a property with public setter.
                if (propertySymbol != null &&
                    propertySymbol.SetMethod?.DeclaredAccessibility == Accessibility.Public &&
                    // Check if the class belongs to the ViewModels namespace.
                    IsFormViewModelProperty(propertySymbol) &&
                    // Check for the ValidateSetAccess attribute to identify properties that need to be analyzed.
                    IsNonEditableProperty(propertySymbol) &&
                    // Validate if the assignment is within the allowed context of the ViewModel.
                    !IsAssignmentWithinAllowedContext(assignment, propertySymbol, semanticModel, context.CancellationToken))
                {
                    // Issue a warning if the assignment is not within the allowed context.
                    var diagnostic = Diagnostic.Create(Rule, assignment.GetLocation(), propertySymbol.Name);
                    context.ReportDiagnostic(diagnostic);
                }
            }
        }

        // Check if that the type containing the property is derived from FormViewModel.
        private bool IsFormViewModelProperty(IPropertySymbol propertySymbol) => propertySymbol.ContainingType.BaseType?.Name == "FormViewModel";

        private static readonly ConcurrentDictionary<string, bool> attributeCheckCache = new ();
        private bool IsNonEditableProperty(IPropertySymbol propertySymbol)
        {
            // Check for the ValidateSetAccess attribute to identify properties that need to be analyzed.
            var propertySignature = $"{propertySymbol.ContainingType}.{propertySymbol.MetadataName}";
            return attributeCheckCache.GetOrAdd(propertySignature, _ => propertySymbol.GetAttributes().Any(attr => attr.AttributeClass.Name == "ValidateSetAccessAttribute"));
        }

        /// <summary>
        /// Checks if the property assignment occurs within an allowed context, such as within the same class as the property.
        /// This helps ensure that the assignment is intentional and conforms to expected data persistence patterns.
        /// </summary>
        /// <param name="assignmentExpression">The assignment expression syntax node.</param>
        /// <param name="propertySymbol">The property symbol.</param>
        /// <param name="semanticModel">The semantic model.</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the process of obtaining the semantic info.</param>
        /// <returns>True if the assignment is within the allowed context; otherwise, false.</returns>
        private bool IsAssignmentWithinAllowedContext(AssignmentExpressionSyntax assignmentExpression, IPropertySymbol propertySymbol, SemanticModel semanticModel, CancellationToken cancellationToken)
        {
            var classDeclaration = assignmentExpression.FirstAncestorOrSelf<ClassDeclarationSyntax>();
            if (classDeclaration != null)
            {
                var classSymbol = semanticModel.GetDeclaredSymbol(classDeclaration, cancellationToken);
                // Check if the class of the assignment matches the class of the property to validate the context.
                return SymbolEqualityComparer.Default.Equals(classSymbol, propertySymbol.ContainingType);
            }

            return false;
        }
    }
}
