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
using Microsoft.CSharp.RuntimeBinder;
using System.ComponentModel;

namespace GenioServer.security
{
    /* Example of External Identity Providers
     *  - GOOGLE https://developers.google.com/identity/protocols/OpenIDConnect:
     *      Authorize URI -> https://accounts.google.com/o/oauth2/v2/auth
     *      TokenEndpoint -> https://oauth2.googleapis.com/token
     *  - AzureAD https://docs.microsoft.com/en-us/azure/active-directory/azuread-dev/v1-protocols-oauth-code: 
     *      Authorize URI -> https://login.microsoftonline.com/common/oauth2/v2.0/authorize
     *      TokenEndpoint -> https://login.microsoftonline.com/common/oauth2/v2.0/token
     */

    public class OpenIdConnectIdentityProviderOptions
    {
        public OpenIdConnectIdentityProviderOptions(string description, string jsonOptions)
        {
            Scope = new List<String>() 
            { 
                "openid", //This is the only mandatory scope and will return a sub claim which represents a unique identifier for the authenticated user.
                "profile" //This scope value requests access to the End-User’s default profile Claims, which are: name, family_name, given_name, middle_name, nickname, preferred_username, profile, picture, website, gender, birthdate, zoneinfo, locale, and updated_at.                
            };
            ResponseType = new List<String>()
            {
                "id_token", //The ID_Token is represented as a JSON Web Token (JWT), which uses JSON Web Signature (JWS) and JSON Web Encryption (JWE) specifications enabling the claims to be digitally signed or MACed and/or encrypted.
                "code"      //The Authorization Code Flow returns an Authorization Code to the Client. This provides the benefit of not exposing any tokens to the User Agent and possibly other malicious applications with access to the User Agent. The Authorization Server can also authenticate the Client before exchanging the Authorization Code for an Access Token. The Authorization Code flow is suitable for Clients that can securely maintain a Client Secret between themselves and the Authorization Server. More information at https://openid.net/specs/openid-connect-core-1_0.html#CodeFlowAuth
            };
            ResponseMode = "form_post";

            //load Options from configuracoes.xml
            Description = description;

            if (Log.IsDebugEnabled)
                Log.Debug(string.Format("OpenID Config: {0}", jsonOptions));

            try
            {
                dynamic jsonOp = JObject.Parse(jsonOptions.Substring(jsonOptions.IndexOf("=") + 1));
                Authority = jsonOp.Authority;// "https://accounts.google.com/o/oauth2/v2/auth";
                ClientId = jsonOp.ClientId; //"226378632051-uvoonk0qhf3ee25tpbj5lu9m4noscor1.apps.googleusercontent.com";
                try
                {
                    ClientSecret = jsonOp.ClientSecret; //"D5MCbKT64e9RrzckJddAdZA5";
                    TokenEndpoint = jsonOp.TokenEndpoint; //"https://oauth2.googleapis.com/token";
                }
                catch 
                {
                    ClientSecret = null;
                    TokenEndpoint = null;
                }
                                
                UserIdField = jsonOp.UserIdField;
                State = jsonOp.State;

                try
                {
                    Scopes = jsonOp.Scopes.ToObject<List<string>>();
                }
                catch { Scopes = null;}
                
                if(Scopes !=null)
                    Scope.AddRange(Scopes);                
            }
            catch(Exception ex)
            {
                Log.Error(string.Format("OpenIdConnectIdentityProviderOptions: {0}", ex.Message));
                throw new Exception("Missing options! It's mandatory Authority, ClientId, ClientSecret and TokenEndpoint");
            }
        }

        /// <summary>
        /// Used when have multiple providers to distinct them for the final user and to switch authentication code between their.
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Authority is the url used when making OpenIdConnect calls.
        /// </summary>
        [SecurityProviderOption()]
        [Description("Authority is the url used when making OpenIdConnect calls")]
        public string Authority { get; set; }
        /// <summary>
        /// To obtain an Access Token, an ID Token, and optionally a Refresh Token, the RP (Client) sends a Token Request to the Token Endpoint to obtain a Token Response. More info at https://openid.net/specs/openid-connect-core-1_0.html#TokenEndpoint
        /// </summary>
        [SecurityProviderOption()]
        [Description("To obtain an Access Token, an ID Token, and optionally a Refresh Token, the RP (Client) sends a Token Request to the Token Endpoint to obtain a Token Response. More info at https://openid.net/specs/openid-connect-core-1_0.html#TokenEndpoint")]
        public string TokenEndpoint { get; set; }
        /// <summary>
        /// OAuth 2.0 Client Identifier valid at the Authorization Server.
        /// </summary>
        [SecurityProviderOption()]
        [Description("OAuth 2.0 Client Identifier valid at the Authorization Server")]
        public string ClientId { get; set; }
        // <summary>
        /// Client secret provided by the idenity provider to double check if user are authenticated successfully.
        /// </summary>
        [SecurityProviderOption()]
        [Description("Client secret provided by the idenity provider to double check if user are authenticated successfully")]
        public string ClientSecret { get; set; }
        /// <summary>
        /// OAuth 2.0 Response Type value that determines the authorization processing flow to be used, including what parameters are returned from the endpoints used. When using the Authorization Code Flow, this value is code.
        /// </summary>
        public List<string> ResponseType { get; set; }
        public string ResponseMode { get; set; }
        /// <summary>
        /// The request path within the application's base path where the user-agent will be returned. 
        /// </summary>
        public string CallbackPath { get; set; }
        /// <summary>
        /// Gets the list of permissions to request.
        /// </summary>
        [JsonIgnore]
        public List<string> Scope { get; private set; }
        /// <summary>
        /// field used to connect to a database user Ex: "email"
        /// </summary>
        [SecurityProviderOption(optional: true)]
        [Description("Field used to connect to a database user Ex: 'email'")]
        public string UserIdField { get; set; }
        /// <summary>
        /// Gets the list of permissions to request (config).
        /// </summary>
        [SecurityProviderOption(optional: true)]
        [Description("List of permissions to request")]
        public List<string> Scopes { get; set; }
        /// <summary>
        /// Opaque value used to maintain state between the request and the callback. Typically, Cross-Site Request Forgery (CSRF, XSRF) mitigation is done by cryptographically binding the value of this parameter with a browser cookie.
        /// </summary>
        [SecurityProviderOption(optional:true)]
        [Description("Opaque value used to maintain state between the request and the callback. Typically, Cross-Site Request Forgery (CSRF, XSRF) mitigation is done by cryptographically binding the value of this parameter with a browser cookie")]
        public string State { get; set; }
    }

    [CredentialProvider(typeof(TokenCredential))]
    [Description("Establishes identity using an external OpenIdConnect provider.")]
    [DisplayName("OpenIdConnect")]
    public class OpenIdConnectIdentityProvider : IIdentityProvider
    {
        [SecurityProviderOption(isJson:true)]
        public OpenIdConnectIdentityProviderOptions Options { get; set; }

        public OpenIdConnectIdentityProvider ()
        {
            var allOpenIdAuth = Configuration.Security.IdentityProviders.FindAll(x => x.Type == typeof(OpenIdConnectIdentityProvider).FullName);
            if (allOpenIdAuth.Count == 0)
                return;

            Options = new OpenIdConnectIdentityProviderOptions(allOpenIdAuth[0].Name, allOpenIdAuth[0].Config);
        }
        public OpenIdConnectIdentityProvider(OpenIdConnectIdentityProviderOptions op)
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
        /// Hydrate URL, to avoid problems with reverse proxy 
        /// </summary>
        /// <param name="callBackpath">call back url</param>
        /// <returns></returns>
        public string HydrateURL(string callBackpath)
        {
            var callbackUri = new UriBuilder(callBackpath)
            {
                Port = -1,
                Scheme = Uri.UriSchemeHttps
            };
            return callbackUri.ToString();
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

            if (String.IsNullOrEmpty(Options.Authority) || String.IsNullOrEmpty(Options.ClientId) || String.IsNullOrEmpty(Options.CallbackPath))
                throw new Exception("It's mandatory to configure Authority, ClientId and CallbackPath options");

            var uriBuilder = new UriBuilder(Options.Authority);
            var parameters = HttpUtility.ParseQueryString(string.Empty);
            parameters["response_type"] = string.Join(" ", Options.ResponseType);
            parameters["response_mode"] = Options.ResponseMode;
            if (Options.Scope.Contains("openid")) //this parameter is mandatory when we have scope to made authentication using openid connect
                parameters["nonce"] = Guid.NewGuid().ToString("N").ToUpper(); //String value used to associate a Client session with an ID Token, and to mitigate replay attacks. 
            parameters["client_id"] = Options.ClientId;            
            parameters["redirect_uri"] =  HydrateURL(Options.CallbackPath);
            parameters["scope"] = string.Join(" ", Options.Scope);
            
            if(!string.IsNullOrEmpty(Options.State))            
                parameters["state"] = Options.State;

            uriBuilder.Query = System.Web.HttpUtility.UrlDecode(parameters.ToString());

            if (Log.IsDebugEnabled)
                Log.Debug(string.Format("GetUrlToAuthenticate: {0}", uriBuilder.Uri.ToString()));

            return uriBuilder.Uri.ToString();
        }

        /// <summary>
        /// Will check credentials and will find the "authenticated" user are on our application
        /// </summary>
        /// <param name="credential">Token identification to user on external provider</param>
        /// <returns>Internal Identity when user are found and success login on external provider</returns>
        public IIdentity Authenticate(Credential credential)
        {
            return Authenticate(credential, "");
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
        /// Validate token with optional to make other call to external identity provider to double check authentication validity
        /// </summary>
        /// <param name="credential">Token identification to user on external provider</param>
        /// <param name="code">When not empty will double check if code returned from JWT are alright authenticated on external provider</param>
        /// <returns>Return true when token are valid otherwise will return false</returns>
        public bool ValidateToken(Credential credential, string code)
        {
            if (!(credential is TokenCredential))
                return false;
            if (String.IsNullOrEmpty(((TokenCredential)credential).Token))
                return false;

            //validate token if have code, client_secret and url to the token endpoint
            //this will double check if user is authenticated well
            if (!String.IsNullOrEmpty(code) && !String.IsNullOrEmpty(Options.ClientSecret) && !String.IsNullOrEmpty(Options.TokenEndpoint))
            {
                using (var wb = new WebClient())
                {
                    var data = new NameValueCollection();
                    data["grant_type"] = "authorization_code";
                    data["code"] = code;
                    data["client_id"] = Options.ClientId;
                    data["client_secret"] = Options.ClientSecret;
                    data["redirect_uri"] =  HydrateURL(Options.CallbackPath);
                    //If the post message have error try put this header (Only microsoft recommend that but works with default one)
                    //wb.Headers.Add("Content-Type", "application/x-www-form-urlencoded");

                    try
                    {
                        var response = wb.UploadValues(Options.TokenEndpoint, "POST", data);
                        string responseInString = Encoding.UTF8.GetString(response);
                        Log.Error("resposta:" +responseInString);
                        var jsonMsg = JsonConvert.DeserializeObject(responseInString); //If the user are succefull authenticated the return message have to be a good json                        
                        
                        if (Log.IsDebugEnabled)
                            Log.Debug(string.Format("ValidateToken reponse: {0}", jsonMsg.ToString()));
                        
                        return true;
                    }
                    catch (Exception ex)
                    {
                        if(Log.IsDebugEnabled)
                            Log.Debug(string.Format("ValidateToken: {0}", ex.Message));  
                        
                        throw new Exception(ex.Message);
					}                      
                }
            }
            return true;
        }

        /// <summary>
        /// Will check credentials and will find the "authenticated" user are on our application
        /// </summary>
        /// <param name="credential">Token identification to user on external provider</param>
        /// <param name="code">When not empty will double check if code returned from JWT are alright authenticated on external provider</param>
        /// <returns>Internal Identity when user are found and success login on external provider</returns>
        public IIdentity Authenticate(Credential credential, string code)
        {
            if (!ValidateToken(credential, code))
                return null;

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
                    id = Authenticate(credential as TokenCredential, sp);
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

        private IIdentity Authenticate(TokenCredential credential, PersistentSupport sp)
        {
            try
            {
                if (Log.IsDebugEnabled)
                    Log.Debug(string.Format("Authenticate token: {0}", credential.Token));
                                                
                dynamic jsonPayload = JObject.Parse(credential.Token.Substring(credential.Token.IndexOf("}.{") + 2));

                string username = jsonPayload.sub.Value + //Subject
                                "@" + jsonPayload.iss.Value; //Issuer
                                
                SelectQuery select = new SelectQuery()
                    .Select("psw", "status")
                    .Select("psw", "nome")
                    .From(Area.AreaPSW)
                    .Where(CriteriaSet.And().Equal("psw", "userid", username));

                var results = sp.executeReaderOneRow(select);
                //Only create the connection to the database user if the 'UserIdField' is not empty
                if (results.Count == 0 && !string.IsNullOrEmpty(Options.UserIdField))
                {
                    if (Log.IsDebugEnabled)
                        Log.Debug("Authenticate, Creating connection with the database user");

                    try
                    {
                        string email = jsonPayload[Options.UserIdField].Value;
                        //No value for IDField 
                        if (string.IsNullOrEmpty(email))
                            return null;
                        
                        select = new SelectQuery()
                                .Select("psw", "status")
                                .Select("psw", "nome")
                                .From(Area.AreaPSW)
                                .Where(CriteriaSet.And().Equal("psw", "email", email));
                        results = sp.executeReaderOneRow(select);
                        if (results.Count > 0)
                        {
                            UpdateQuery updatequery = new UpdateQuery().Update(Area.AreaPSW).Set("userid", username).Where(CriteriaSet.And().Equal("psw", "email", email));
                            sp.Execute(updatequery);

                            if (Log.IsDebugEnabled)
                                Log.Debug("Authenticate, Database user connected");
                        }
                        else
                            return null;
                    }
                    catch (RuntimeBinderException)
                    {
                        return null;
                    }                    
                }
                int status = DBConversion.ToInteger(results[0]);
                string name = DBConversion.ToString(results[1]);

                if (status == 2)
                    return null;

                return new GenericIdentity(name);
            }
            catch (Exception ex)
            {
                Log.Error(String.Format("Authenticate: {0}", ex.Message));
                return null;
            }
        }
    }
}
