using System;
using System.Security.Principal;
using CSGenio.business;
using CSGenio.persistence;
using Quidgest.Persistence.GenericQuery;
using System.Collections.Generic;
using CSGenio.framework;
using System.ComponentModel;

namespace GenioServer.security
{

    [CredentialProvider(typeof(UserPassCredential))]
    [CredentialProvider(typeof(CertificateCredential))]
    [CredentialProvider(typeof(DomainCredential))]
    [Description("Establishes identity according to the current application database.")]
    [DisplayName("Application Database identity")]
    public class QuidgestIdentityProvider : IIdentityProvider
    {
        public IIdentity Authenticate(Credential credential)
        {
            IList<string> anos = new List<string>(Configuration.Years);
            if (Configuration.Years.Count == 0)
            {
                anos.Add(Configuration.DefaultYear);
            }
            Type classname = credential.GetType();
            IIdentity id = null;

            foreach (string Qyear in anos)
            {
                PersistentSupport sp = PersistentSupport.getPersistentSupport(Qyear);
                sp.openConnection();

                bool known = false;
                if (classname == typeof(UserPassCredential))
                {
                    id = Authenticate(credential as UserPassCredential, sp);
                    known = true;
                }
                if (classname == typeof(CertificateCredential))
                {
                    id = Authenticate(credential as CertificateCredential, sp);
                    known = true;
                }
                if (classname == typeof(DomainCredential))
                {
                    id = Authenticate(credential as DomainCredential, sp);
                    known = true;
                }
                if (!known)
                    throw new FrameworkException("The type " + credential.GetType().FullName + " is not supported for QuidgestIdentityProvider authentication.", "Authenticate", "Credential type not supported");

                sp.closeConnection();

                if (id != null)
                    break;
            }

            return id;
        }

        /// <summary>
        /// Determines whether username and password authentication is enabled.
        /// </summary>
        /// <remarks>
        /// This is used to determine if username and password authentication is enabled
        /// Note: This method checks for the presence of specific identity providers but does not verify the specific login mode (e.g., AD, Certificate, Username/Password) they are configured for. 
        /// 	Further checks might be necessary to determine the exact authentication mode each provider supports.
        /// </remarks>
        public bool HasUsernameAuth()
        {
            return true;
        }

        private IIdentity Authenticate(UserPassCredential credential, PersistentSupport sp)
        {
            SelectQuery select = new SelectQuery()
                .Select("psw", "password")
                .Select("psw", "pswtype")
                .Select("psw", "salt")
                .Select("psw", "status")
                .Select("psw", "attempts")
				.Select("psw", "datexp")
                .From(Area.AreaPSW)
                .Where(CriteriaSet.And().Equal("psw", "nome", credential.Username));
            
            var results = sp.executeReaderOneRow(select);
            if (results.Count == 0)
                return null;
            string pass = DBConversion.ToString(results[0]);
            string pswtype = DBConversion.ToString(results[1]);
            string salt = DBConversion.ToString(results[2]);
            int status = DBConversion.ToInteger(results[3]);
            int attempts = DBConversion.ToInteger(results[4]);
			DateTime datexp = DBConversion.ToDateTime(results[5]);

            int maxAttempts = Configuration.Security.MaxAttempts;

            if (pass == null)
                return null;

            if (PasswordFactory.IsOK(credential.Password, pass, salt, pswtype))
            {
                if (maxAttempts != 0)
                {
                    UpdateQuery upd = new UpdateQuery()
                    .Update(Area.AreaPSW)
                    .Set(CSGenioApsw.FldAttempts, 0)
                    .Where(CriteriaSet.And().Equal("psw", "nome", credential.Username));
                    sp.Execute(upd);
                }
				
				//Date expiration Validation
                if (Configuration.Security.ExpirationDateBool)
                {
                    try
                    {
                        bool statusChange = false;
                        if (datexp == DateTime.MinValue)
                            statusChange = true;

                        int daysExpirecy = Int32.Parse(Configuration.Security.ExpirationDate);
                        DateTime expirationCheckDate = datexp.AddDays(daysExpirecy);

                        if (expirationCheckDate <= DateTime.Now)
                            statusChange = true;

                        if (statusChange)
                        {
                            UpdateQuery upd = new UpdateQuery()
                            .Update(Area.AreaPSW)
                            .Set(CSGenioApsw.FldStatus, 1)
                            .Where(CriteriaSet.And().Equal("psw", "nome", credential.Username));
                            sp.Execute(upd);
                        }
                    }
                    catch (FormatException)
                    {
                        Log.Error("Password expiration days have a wrong format. Please review administration website to fix it.");
                    }
                }
				
                return new GenericIdentity(credential.Username);
            }


            if (maxAttempts!= 0 && attempts + 1 == maxAttempts)
            {
                UpdateQuery upd = new UpdateQuery()
                    .Update(Area.AreaPSW)
                    .Set(CSGenioApsw.FldAttempts, attempts + 1)
                    .Set(CSGenioApsw.FldStatus, 2)
                    .Where(CriteriaSet.And().Equal("psw", "nome", credential.Username));
                sp.Execute(upd);
            }
            else
            {
                UpdateQuery upd = new UpdateQuery()
                    .Update(Area.AreaPSW)
                    .Set(CSGenioApsw.FldAttempts, attempts + 1)
                    .Where(CriteriaSet.And().Equal("psw", "nome", credential.Username));
                sp.Execute(upd);
            }
            return null;
        }

        private IIdentity Authenticate(CertificateCredential credential, PersistentSupport sp)
        {
            SelectQuery select = new SelectQuery()
                .Select("psw", "nome")
                .From(Area.AreaPSW)
                .Where(CriteriaSet.And().Equal("psw", "certsn", credential.Certificate.returnSerialNumber()));
            string name = sp.ExecuteScalar(select) as string;
            if (name == null)
                return null;

            return new GenericIdentity(name);
        }

        private IIdentity Authenticate(DomainCredential credential, PersistentSupport sp)
        {
            SelectQuery select = new SelectQuery()
                .Select("psw", "nome")
                .From(Area.AreaPSW)
                .Where(CriteriaSet.And().Equal("psw", "nome", credential.DomainUser));
            string name = sp.ExecuteScalar(select) as string;
            if (name == null)
                return null;

            return new GenericIdentity(name);
        }
    }
}
