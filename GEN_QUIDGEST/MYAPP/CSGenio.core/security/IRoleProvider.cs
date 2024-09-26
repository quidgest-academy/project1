using System;
using System.Security.Principal;

namespace GenioServer.security
{
    /// <summary>
    /// Common interface for role providers
    /// </summary>
    public interface IRoleProvider
    {
        /// <summary>
        /// Obtains the roles for an authenticated user
        /// </summary>
        /// <param name="identity">the user identity</param>
        /// <returns>The user roles</returns>
        IPrincipal GetUserRoles(IIdentity identity);

        /// <summary>
        /// Get user by username
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="user"></param>
        /// <param name="sp"></param>
        /// <returns></returns>
        CSGenio.business.CSGenioApsw GetUser(string userName, CSGenio.framework.User user, CSGenio.persistence.PersistentSupport sp);

        /// <summary>
        /// Get user by email
        /// </summary>
        /// <param name="email"></param>
        /// <param name="user"></param>
        /// <param name="sp"></param>
        /// <returns></returns>
        CSGenio.business.CSGenioApsw GetUserFromEmail(string email, CSGenio.framework.User user, CSGenio.persistence.PersistentSupport sp);
    }
}
