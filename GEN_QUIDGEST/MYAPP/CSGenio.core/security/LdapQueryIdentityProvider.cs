using System;
using System.ComponentModel;
using System.DirectoryServices;
using System.DirectoryServices.Protocols;
using System.Net;
using System.Security.Principal;
using CSGenio.framework;

namespace GenioServer.security
{
    [CredentialProvider(typeof(UserPassCredential))]
    [CredentialProvider(typeof(DomainCredential))]
    [Description("Performs a LDAP Query to validate user credentials")]
    [DisplayName("LDAP Query")]
    class LdapQueryIdentityProvider : IIdentityProvider
    {
        [SecurityProviderOption()]
        [Description("Query to perform against ldap. In the form of 'cn=username,ou=domain users,dc=example,dc=com'")]
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
			//this method support OpenLDAP

            //get complete domain value separated by dot
			string domainValue = "";
			string[] paramDomain = Domain.Split(',');
			foreach (string param in paramDomain)
			{
				string key = param.Substring(0, param.IndexOf("="));
				string value = param.Substring(param.IndexOf("=") + 1);
				if (key.ToUpper() == "DC")
					domainValue += value + ".";
			}
			domainValue = domainValue.Remove(domainValue.Length - 1);
			if (!string.IsNullOrEmpty(Port))
				domainValue += ":" + Port;

			//this method is used for example on OpenLDAP
			using (var ldap = new LdapConnection(new LdapDirectoryIdentifier(domainValue)))
			{
				ldap.SessionOptions.ProtocolVersion = 3;

				try
				{
					ldap.AuthType = AuthType.Anonymous;
					ldap.Bind();
				}
				catch (Exception ex)
				{
					Log.Error(string.Format("GenioServer.security.LdapIdentityProvider.Autheticate Invalid value for domain, cannot contact domain {0}, [Exception message] {1}", domainValue, ex.Message));
				}

				var dn = String.Format(Domain, credential.Username);

				if (dn != null)
				{
					try
					{
						ldap.AuthType = AuthType.Basic;
						ldap.Bind(new NetworkCredential(dn, credential.Password));
						return new GenericIdentity(credential.Username);
					}
					catch (DirectoryOperationException ex1)
					{ //Invalid user.
						Log.Error(string.Format("GenioServer.security.LdapIdentityProvider.Autheticate Invalid [user] {0} [domain] {1}, [Exception message] {2}", credential.Username, domainValue, ex1.Message));
					}
					catch (LdapException ex2)
					{ //Invalid password.
						Log.Error(string.Format("GenioServer.security.LdapIdentityProvider.Autheticate Invalid [user] {0} [domain] {1}, [Exception message] {2}", credential.Username, domainValue, ex2.Message));
					}
					catch (Exception ex)
					{
						Log.Error(string.Format("GenioServer.security.LdapIdentityProvider.Autheticate [user] {0} [domain] {1}, [Exception message] {2}", credential.Username, domainValue, ex.Message));
					}
				}
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
