using System;
using CSGenio.framework;

namespace GenioServer.security
{
    public abstract class Credential
    {
        public string Year { get; set; }
    }

    public class UserPassCredential : Credential
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class CertificateCredential : Credential
    {
        public ClientCertificate Certificate { get; set; }
    }

    public class DomainCredential : Credential
    {
        public string DomainUser { get; set; }
    }

    public class TokenCredential : Credential
    {
        public string Token { get; set; }
    }
	
	public class TokenCASCredential : TokenCredential
    {
        public string OriginUrl { get; set; }
    }

    public class Password
    {
        public Password(string newPass, string confirmPass)
        {
            New = newPass;
            Confirm = confirmPass;
        }
        public string New { get; set; }
        public string Confirm { get; set; }
    }

    public class InvalidPasswordException : FrameworkException
    {
        public InvalidPasswordException(string errorMsg,string site,string cause) : base(errorMsg, site, cause) { }
    }
}
