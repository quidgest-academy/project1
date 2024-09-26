using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Security.Principal;

namespace GenioServer.security
{
    // Summary:
    //     Represents a generic principal.
    [Serializable]
    [ComVisible(true)]
    public class ErrorPrincipal : IPrincipal
    {
        private string errorMessage;
        
        // Summary:
        //     Initializes a new instance of the System.Security.Principal.GenericPrincipal
        //     class from a user identity and an array of role names to which the user represented
        //     by that identity belongs.
        //
        // Parameters:
        //   identity:
        //     A basic implementation of System.Security.Principal.IIdentity that represents
        //     any user.
        //
        //   roles:
        //     An array of role names to which the user represented by the identity parameter
        //     belongs.
        //
        // Exceptions:
        //   System.ArgumentNullException:
        //     The identity parameter is null.
        public ErrorPrincipal(string message)
        {
            ErrorMessage = message;
        }

        // Summary:
        //     Gets the System.Security.Principal.GenericIdentity of the user represented
        //     by the current System.Security.Principal.GenericPrincipal.
        //
        // Returns:
        //     The System.Security.Principal.GenericIdentity of the user represented by
        //     the System.Security.Principal.GenericPrincipal.
        public virtual IIdentity Identity
        {
            get { return null; }
        }

        // Summary:
        //     Determines whether the current System.Security.Principal.GenericPrincipal
        //     belongs to the specified role.
        //
        // Parameters:
        //   role:
        //     The name of the role for which to check membership.
        //
        // Returns:
        //     true if the current System.Security.Principal.GenericPrincipal is a member
        //     of the specified role; otherwise, false.
        public virtual bool IsInRole(string role)
        {
            return false;
        }

        /// <summary>
        /// Devolve ou coloca a mensagem de erro no atributo errorMessage
        /// </summary>
        public string ErrorMessage
        {
            get { return errorMessage; }
            set { errorMessage = value; }
        }
    }
}
