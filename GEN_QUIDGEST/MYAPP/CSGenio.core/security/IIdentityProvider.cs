using System;
using System.Security.Principal;

namespace GenioServer.security
{
    /// <summary>
    /// Common interface for authentication providers
    /// </summary>
    public interface IIdentityProvider
    {
        /// <summary>
        /// Authenticates a user
        /// </summary>
        /// <param name="credential">The user credentials</param>
        /// <returns>The user identity</returns>
        IIdentity Authenticate(Credential credential);

        /// <summary>
        /// Determines whether username and password authentication is enabled.
        /// </summary>
        /// <remarks>
        /// This is used to determine if username and password authentication is enabled.
        /// Note: This method checks for the presence of specific identity providers but does not verify the specific login mode (e.g., AD, Certificate, Username/Password) they are configured for. 
        /// 	Further checks might be necessary to determine the exact authentication mode each provider supports.
        /// </remarks>
        bool HasUsernameAuth();
    }

    /// <summary>
    /// Declares that the class attached can handle authentication of a Credential type
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    public sealed class CredentialProviderAttribute : Attribute
    {
        /// <summary>
        /// The Credential type
        /// </summary>
        public Type CredentialType { get; set; }

        /// <summary>
        /// Positional constructor
        /// </summary>
        /// <param name="credentialType">The Credential type</param>
        public CredentialProviderAttribute(Type credentialType)
        {
            this.CredentialType = credentialType;
        }
    }

    /// <summary>
    /// Marks a property of a Security provider as user defined
    /// This allows the UI to present an adequate editor for this property
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public sealed class SecurityProviderOptionAttribute : Attribute
    {
        /// <summary>
        /// Optional property, default is mandatory
        /// </summary>
        public bool Optional { get; set; }

        /// <summary>
        /// This property is saved as a complex object json that requires further parsing
        /// </summary>
        public bool IsJson { get; set; }

        /// <summary>
        /// Positional constructor
        /// </summary>
        /// <param name="optional">Optional property, default is mandatory</param>
        public SecurityProviderOptionAttribute(bool optional=false, bool isJson=false)
        {
            Optional = optional;
            IsJson = isJson;
        }
    }

}
