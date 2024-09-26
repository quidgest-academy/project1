using System;
using System.Text;
using System.Security.Principal;
using CSGenio.business;
using CSGenio.persistence;
using Quidgest.Persistence.GenericQuery;
using System.Collections.Generic;
using CSGenio.framework;
using OtpNet;

namespace GenioServer.security
{

    [CredentialProvider(typeof(UserPassCredential))]
    public class TOTPIdentityProvider : IIdentityProvider
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
                if (!known)
                    throw new Exception("The type " + credential.GetType().FullName + " is not supported for QuidgestIdentityProvider authentication.");

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
        /// This is used to determine if username and password authentication is enabled.
        /// </remarks>
        public bool HasUsernameAuth()
        {
            return true;
        }

        public bool IsOk(string valPsw2favl, string pass)
        {
            var secretBytes = Encoding.ASCII.GetBytes(valPsw2favl);
            var p = new Totp(secretBytes);
            var genCode = p.ComputeTotp();
            return pass == genCode;
        }

        private IIdentity Authenticate(UserPassCredential credential, PersistentSupport sp)
        {
            SelectQuery select = new SelectQuery()
                .Select("psw", "psw2favl")
                .Select("psw", "psw2fatp")
                .Select("psw", "status")
                .Select("psw", "attempts")
                .From(Area.AreaPSW)
                .Where(CriteriaSet.And().Equal("psw", "nome", credential.Username));
            
            var results = sp.executeReaderOneRow(select);
            if (results.Count == 0)
                return null;
            string pass = DBConversion.ToString(results[0]);
            string pswtype = DBConversion.ToString(results[1]);
            int status = DBConversion.ToInteger(results[2]);
            int attempts = DBConversion.ToInteger(results[3]);

            int maxAttempts = Configuration.Security.MaxAttempts;

            if (pass == null || status == 2)
                return null;

            if (IsOk(pass, credential.Password))
            {
                if (maxAttempts != 0)
                {
                    UpdateQuery updQ = new UpdateQuery()
                        .Update(Area.AreaPSW)
                        .Set(CSGenioApsw.FldAttempts, 0)
                        .Where(CriteriaSet.And().Equal("psw", "nome", credential.Username));
                    sp.Execute(updQ);
                }

                return new GenericIdentity(credential.Username);
            }

            //add one more attempt if user fail the pass
            UpdateQuery upd = new UpdateQuery()
                .Update(Area.AreaPSW)
                .Set(CSGenioApsw.FldAttempts, attempts + 1)
                .Where(CriteriaSet.And().Equal("psw", "nome", credential.Username));
            if (maxAttempts != 0 && attempts + 1 == maxAttempts)
                upd.Set(CSGenioApsw.FldStatus, 2);
            sp.Execute(upd);

            return null;
        }
		
		public static string GetUrlQrCode (string username, string pass2Fa)
        { 
            byte[] secretByte = Encoding.ASCII.GetBytes(pass2Fa);
            var uri = new OtpUri(OtpType.Totp, secretByte, username, Configuration.Program);
            return uri.ToString();
        }
    }
}
