using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Principal;
using CSGenio.framework;
using CSGenio.business;
using CSGenio.persistence;
using Quidgest.Persistence.GenericQuery;
using System.Threading;
using CSGenio;

namespace GenioServer.security
{    
    /// <summary>
    /// Business layer methods that are necessary to create users
    /// </summary>
    public class UserBusinessService : IUserBusinessManager
    {
        private PersistentSupport sp;
        private  User user;

        public void SetLocalProperties(PersistentSupport sp, User user)
        {
            this.sp = sp;
            this.user = user;
        }

    }
}