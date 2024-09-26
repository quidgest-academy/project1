using System;
using System.ComponentModel;
using System.DirectoryServices;
using System.Security.Principal;
using CSGenio.framework;

namespace GenioServer.security
{
    [CredentialProvider(typeof(UserPassCredential))]
    [CredentialProvider(typeof(DomainCredential))]
    [Description("Performs a local LDAP connection to validate user credentials")]
    [DisplayName("LDAP Simple")]
    public class LdapIdentityProvider : IIdentityProvider
    {
        [SecurityProviderOption()]
        [Description("Local domain. In the form of 'example.org'")]
        public string Domain { get; set; }
		
        [SecurityProviderOption()]
        [Description("Port number of the LDAP service. Leave blank for default. LDAPS usually 636.")]
		public string Port { get; set; }

        public IIdentity Authenticate(Credential credential)
        {
            Type classname = credential.GetType();
            IIdentity id = null;

            if (classname == typeof(UserPassCredential))
            {
                id = Authenticate_p(credential as UserPassCredential);
            }
            if (classname == typeof(DomainCredential))
            {
                id = Authenticate_p(credential as DomainCredential);
            }

            return id;
        }

        private IIdentity Authenticate_p(UserPassCredential credential)
        {
            try
            {
				string connection = "LDAP://" + Domain;
                if (!string.IsNullOrEmpty(Port))
                    connection += ":" + Port;
				
                DirectoryEntry entry = new DirectoryEntry(connection, Domain + "\\" + credential.Username, credential.Password);
                object nativeObject = entry.NativeObject;
				return new GenericIdentity(credential.Username);
            }
            catch (Exception ex)
            {
                Log.Error(string.Format("GenioServer.security.LdapIdentityProvider.Autheticate [user] {0} [domain] {1}, [Exception message] {2}", credential.Username, Domain, ex.Message));
            }
            return null;
        }

        private IIdentity Authenticate_p(DomainCredential credential)
        {
            //if (DirectoryEntry.Exists("LDAP://" + Domain + "/" + credential.DomainUser))
				return new GenericIdentity(credential.DomainUser);

            //return null;
        }

        /// <summary>
        /// Determines whether username and password authentication is enabled.
        /// </summary>
        /// <remarks>
        /// This is used to determine if username and password authentication is enabled.
        /// </remarks>
        public bool HasUsernameAuth()
        {
            return true;
        }
	}
}
