using GenioMVC.Models.Navigation;
using GenioMVC.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace GenioMVC;

public class ModuleActionFilter : IActionFilter
{
    UserContext m_userContext;
    public ModuleActionFilter(UserContextService userContext)
    {
        m_userContext = userContext.Current;
    }

    /// <summary>
    /// Before controller action executes
    /// </summary>
    /// <param name="context"></param>
    public void OnActionExecuting(ActionExecutingContext context)
    {
        AuthorizeForUsers(context);
        
        // Ensure UserContext is initialized after the model binding deserialized objects 
        //  using the empty construction.
        foreach(var m in context.ActionArguments)
        {
            if (m.Value is ViewModelBase vmb)
                vmb.Init(m_userContext);
        }
    }

    /// <summary>
    /// After controller action executes
    /// </summary>
    /// <param name="context"></param>    
    public void OnActionExecuted(ActionExecutedContext context)
    {
        var qAjaxId = context.HttpContext.Request.Headers["QAjaxIdentifier"];
        if (!string.IsNullOrEmpty(qAjaxId))
            context.HttpContext.Response.Headers["QAjaxIdentifier"] = qAjaxId;

        // MH (07/09/2017) - Ensure that the transaction was not left open after processing the request. And if transaction is still open it will be closed automatically.
        if (m_userContext.PersistentSupport != null && !m_userContext.PersistentSupport.TransactionIsClosed)
        {
            CSGenio.framework.Log.Error(string.Format("The transaction still open after the action was executed. The transaction will be closed automatically by the application. (URL: {0})",
                context.HttpContext.Request.Path));

            try
            {
                m_userContext.PersistentSupport.closeTransaction();
            }
            catch (Exception ex)
            {
                CSGenio.framework.Log.Error(ex.ToString());
            }
        }
    }

    private void AuthorizeForUsers(ActionExecutingContext context)
    {
        var u = m_userContext.User;
        if (!u.IsGuest())
        {
            // Check if user has their account disabled
            if (u.Status == 2)
            {
                //Force the user to logout
                context.HttpContext.SignOutAsync().Wait();
                context.Result = new RedirectToRouteResult(new RouteValueDictionary { { "action", "HomeRedirect" }, { "controller", "Home" } });
            }
            // Check if user needs to change password
            else if (u.NeedsToChangePassword() && !ActionsAllowed(context))
                context.Result = new RedirectToRouteResult(new RouteValueDictionary { { "action", "ProfileRedirect" }, { "controller", "Home" } });
            // Check if user has to setup 2FA
            else if (u.NeedsToSetup2FA() && !ActionsAllowed(context))
                context.Result = new RedirectToRouteResult(new RouteValueDictionary { { "action", "Change2FARedirect" }, { "controller", "Home" } });
        }       
    }

    private bool ActionsAllowed(ActionExecutingContext filterContext)
    {
        var allowedActions = new HashSet<(string action, string controller)>
        {
            ("Profile", "Home"),
            ("LogOff", "Account"),
            ("GetIfUserLogged", "Account"),
            ("UserAvatar", "Account"),
            ("NavigationalBar", "Home"),
            ("GetImage", "Account"),
            ("Change2FA", "Home"),
            ("GetConfig", "Config"),
            ("ProfileRedirect", "Home"),
            ("HomeRedirect", "Home"),
            ("Change2FARedirect", "Home")
        };

        var currentAction = filterContext.RouteData.Values["action"].ToString();
        var currentController = filterContext.RouteData.Values["controller"].ToString();

        return allowedActions.Contains((currentAction, currentController));
    }
}
