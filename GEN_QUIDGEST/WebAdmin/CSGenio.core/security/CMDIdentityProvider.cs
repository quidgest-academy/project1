using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;
using System.Security.Principal;
using System.Text;
using System.Web;
using CSGenio.business;
using CSGenio.framework;
using CSGenio.persistence;
using Newtonsoft.Json;
using Quidgest.Persistence.GenericQuery;
using Newtonsoft.Json.Linq;
using System.ComponentModel;

namespace GenioServer.security
{
    public class CMDIdentityProviderOptions
    {
        public CMDIdentityProviderOptions(string description, string jsonOptions)
        {
            Scope = new List<String>() 
            {
                "http://interop.gov.pt/MDC/Cidadao/NIF",
				"http://interop.gov.pt/MDC/Cidadao/NomeCompleto",
                "http://interop.gov.pt/MDC/Cidadao/NomeProprio",
				"http://interop.gov.pt/MDC/Cidadao/NomeApelido",
				//The NIC is exclusive to Portuguese citizens and is mandatory
				//which is why it gives an error when CMD is not Portuguese
                "http://interop.gov.pt/MDC/Cidadao/NIC",
				//In order for the CMD to be compatible with foreign users
				//the following DocType and DocNationality and DocNumber properties have been added
				//which are used instead of the NIC when the CMD is not Portuguese
                "http://interop.gov.pt/MDC/Cidadao/DocType",
                "http://interop.gov.pt/MDC/Cidadao/DocNationality",
                "http://interop.gov.pt/MDC/Cidadao/DocNumber"
            };
            ResponseType = new List<String>()
            {
                "token" //The ID_Token is represented as a JSON Web Token (JWT), which uses JSON Web Signature (JWS) and JSON Web Encryption (JWE) specifications enabling the claims to be digitally signed or MACed and/or encrypted.                
            };
            ResponseMode = "form_post";

            //load Options from configuracoes.xml
            Description = description;

            try
            {
                dynamic jsonOp = JObject.Parse(jsonOptions.Substring(jsonOptions.IndexOf("=") + 1));
                Authority = jsonOp.Authority;// "https://accounts.google.com/o/oauth2/v2/auth";
                ClientId = jsonOp.ClientId; //"226378632051-uvoonk0qhf3ee25tpbj5lu9m4noscor1.apps.googleusercontent.com";
                DataAPI = jsonOp.DataAPI;
                UserIdField = jsonOp.UserIdField;
            }
            catch
            {
                throw new Exception("Missing options! It's mandatory Authority, ClientId, DataAPI and UserIdField");
            }
        }

        /// <summary>
        /// Used when have multiple providers to distinct them for the final user and to switch authentication code between their.
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Authority are the url use when making OpenIdConnect calls.
        /// </summary>
        [SecurityProviderOption()]
        [Description("Authority is the url to use when making OpenIdConnect calls")]
        public string Authority { get; set; }        
        /// <summary>
        /// OAuth 2.0 Client Identifier valid at the Authorization Server.
        /// </summary>
        [SecurityProviderOption()]
        [Description("OAuth 2.0 Client Identifier valid at the Authorization Server")]
        public string ClientId { get; set; }        
        /// <summary>
        /// OAuth 2.0 Response Type value that determines the authorization processing flow to be used, including what parameters are returned from the endpoints used. When using the Authorization Code Flow, this value is code.
        /// </summary>
        [JsonIgnore]
        public List<string> ResponseType { get; set; }
        [JsonIgnore]
        public string ResponseMode { get; set; }
        /// <summary>
        /// The request path within the application's base path where the user-agent will be returned. 
        /// </summary>
        [JsonIgnore]
        public string CallbackPath { get; set; }
        /// <summary>
        /// Gets the list of permissions to request.
        /// </summary>
        [JsonIgnore]
        public List<string> Scope { get; private set; }

        /// <summary>
        /// API adress to get data associated with authenticated token
        /// </summary>
        [SecurityProviderOption()]
        [Description("API adress to get data associated with authenticated token")]
        public string DataAPI { get; set; }

        /// <summary>
        /// field used to connect to a database user Ex: "http://interop.gov.pt/MDC/Cidadao/NIF"
        /// </summary>
        [SecurityProviderOption()]
        [Description("Field reference to represent the unique user id: 'http://interop.gov.pt/MDC/Cidadao/NIF'")]
        public string UserIdField { get; set; }

    }


    [CredentialProvider(typeof(DomainCredential))]
    [Description("Establishes identity using citizen Digital Mobile Key.")]
    [DisplayName("Digital Mobile Key")]
    public class CMDIdentityProvider : IIdentityProvider
    {
        [SecurityProviderOption(isJson: true)]
        public CMDIdentityProviderOptions Options { get; set; }

        public CMDIdentityProvider()
        {
            var allOpenIdAuth = Configuration.Security.IdentityProviders.FindAll(x => x.Type == typeof(CMDIdentityProvider).FullName);
            if (allOpenIdAuth.Count == 0)
                return;

            Options = new CMDIdentityProviderOptions(allOpenIdAuth[0].Name, allOpenIdAuth[0].Config);
        }
        public CMDIdentityProvider(CMDIdentityProviderOptions op)
        {
            Options = op;
        }

        /// <summary>
        /// Generate URL with all options from the provider to can be redirect to the provider authentication page
        /// </summary>
        /// <returns>callbackPath are Mandatory, so that function will return error</returns>
        public string GetUrlToAuthenticate ()
        {
            return GetUrlToAuthenticate(String.Empty);
        }

        /// <summary>
        /// Generate URL with all options from the provider to can be redirect to the provider authentication page
        /// </summary>
        /// <param name="callbackPath">Url on our application to receive the request after login on external provider</param>
        /// <returns>Url to redirect our application to provider authentication page</returns>
        public string GetUrlToAuthenticate(string callbackPath)
        {
            if (!String.IsNullOrEmpty(callbackPath))
                Options.CallbackPath = callbackPath;

            if (String.IsNullOrEmpty(Options.Authority) || String.IsNullOrEmpty(Options.ClientId) || String.IsNullOrEmpty(Options.CallbackPath) || String.IsNullOrEmpty(Options.DataAPI) || String.IsNullOrEmpty(Options.UserIdField))
                throw new Exception("It's mandatory to configure Authority, ClientId, DataAPI, UserIdField and CallbackPath options");

            var uriBuilder = new UriBuilder(Options.Authority);
            var parameters = HttpUtility.ParseQueryString(string.Empty);
            parameters["response_type"] = string.Join("+", Options.ResponseType);
            parameters["response_mode"] = Options.ResponseMode;
            if (Options.Scope.Contains("openid")) //this parameter is mandatory when we have scope to made authentication using openid connect
                parameters["nonce"] = Guid.NewGuid().ToString("N").ToUpper(); //String value used to associate a Client session with an ID Token, and to mitigate replay attacks.                        
            
            parameters["client_id"] = Options.ClientId;
            var callbackUri = new UriBuilder (Options.CallbackPath)
            {
                Port = -1,
                Scheme = Uri.UriSchemeHttps
            };
            parameters["redirect_uri"] = callbackUri.ToString();
            parameters["scope"] = string.Join(" ", Options.Scope);
            uriBuilder.Query = parameters.ToString();
          
            return uriBuilder.Uri.ToString();
        }   
        
        /// <summary>
        /// Will check credentials and will find the "authenticated" user are on our application
        /// </summary>
        /// <param name="userData">Token identification to user on external provider</param>        
        /// <returns>Internal Identity when user are found and success login on external provider</returns>
        public IIdentity Authenticate(Credential credencial)
        {
            //At this moment the user is authenticated and we have to check if that user exist on database
            IList<string> anos = new List<string>(Configuration.Years);
            if (Configuration.Years.Count == 0)
                anos.Add(Configuration.DefaultYear);

            IIdentity id = null;
            foreach (string Qyear in anos)
            {
                PersistentSupport sp = PersistentSupport.getPersistentSupport(Qyear);
                try
                {
                    sp.openConnection();
                    id = Authenticate(credencial as DomainCredential, sp);
                }
                catch { }
                finally
                {
                    if (!sp.TransactionIsClosed)
                        sp.closeConnection();
                }

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
            return false;
        }

        /// <summary>
        /// Decode returned da from data API to a valid credential
        /// </summary>
        /// <param name="userData">Data returned from API</param>
        /// <returns></returns>
        public DomainCredential ValidateCredencial(string userData)
        {
            try
            {
                string userName = "";
                JArray jsonPayload = JArray.Parse(userData);
                if (jsonPayload.Count == 0)
                    return null;

                foreach (JObject item in jsonPayload)
                {
                    string name = item.GetValue("name").ToString();
                    string value = item.GetValue("value").ToString();

                    if (name.Equals(Options.UserIdField, StringComparison.OrdinalIgnoreCase))
                    {
                        userName = value;
                        break;
                    }
                }
                if (string.IsNullOrEmpty(userName))
                    return null;

                DomainCredential credential = new DomainCredential { DomainUser = userName };                
                return credential;
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return null;
            }            
        }

        private IIdentity Authenticate(DomainCredential credential, PersistentSupport sp)
        {
            try
            {
                Log.Error("Credencial: " + credential.DomainUser);

                SelectQuery select = new SelectQuery()
                    .Select("psw", "status")
                    .Select("psw", "nome")
                    .From(Area.AreaPSW)
                    .Where(CriteriaSet.And().Equal("psw", "userid", credential.DomainUser));

                var results = sp.executeReaderOneRow(select);
                if (results.Count == 0)
                    return null;
                int status = DBConversion.ToInteger(results[0]);
                string name = DBConversion.ToString(results[1]);

                if (status == 2)
                    return null;

                return new GenericIdentity(name);
            }
            catch(Exception ex)
            {
                Log.Error(ex.Message);
                return null;
            }
        }
    }    
}
