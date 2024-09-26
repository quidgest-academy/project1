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
using System.Xml;
using System.IO;
using System.ComponentModel;

namespace GenioServer.security
{
    public class CASIdentityProviderOptions
    {
        public CASIdentityProviderOptions(string description, string jsonOptions)
        {
            //load Options from configuracoes.xml
            Description = description;

            try
            {
                dynamic jsonOp = JObject.Parse(jsonOptions.Substring(jsonOptions.IndexOf("=") + 1));
                Authority = jsonOp.Authority; //URL to CAS server
                AttribValidation = jsonOp.AttribValidation; //Attribute from callback returned from CAS Server to validate if user exist
            }
            catch
            {
                throw new Exception("Missing options! It's mandatory Authority, ClientId, ClientSecret and TokenEndpoint");
            }
        }

        /// <summary>
        /// Used when have multiple providers to distinct them for the final user and to switch authentication code between their.
        /// </summary>
        public string Description { get; set; }
        /// <summary> 
        /// Authority are the url use when making CAS calls.
        /// </summary>
        [SecurityProviderOption()]
        [Description("Authority is the url to use when making CAS calls")]
        public string Authority { get; set; }
        /// <summary>
        /// Attribute from callback returned from CAS Server to validate if user exist
        /// </summary>
        [SecurityProviderOption()]
        [Description("Attribute from callback returned from CAS Server to validate if user exist")]
        public string AttribValidation { get; set; }
        /// <summary>
        /// The request path within the application's base path where the user-agent will be returned. 
        /// </summary>
        public string CallbackPath { get; set; }
    }

    [CredentialProvider(typeof(TokenCredential))]
    [Description("Establishes identity using Central Authentication Service protocol.")]
    [DisplayName("Central Authentication Service (CAS)")]
    public class CASIdentityProvider : IIdentityProvider
    {
        [SecurityProviderOption(isJson: true)]
        public CASIdentityProviderOptions Options { get; set; }

        public CASIdentityProvider()
        {
            var allCASAuth = Configuration.Security.IdentityProviders.FindAll(x => x.Type == typeof(CASIdentityProvider).FullName);
            if (allCASAuth.Count == 0)
                return;

            Options = new CASIdentityProviderOptions(allCASAuth[0].Name, allCASAuth[0].Config);
        }
        public CASIdentityProvider(CASIdentityProviderOptions op)
        {
            Options = op;
        }

        /// <summary>
        /// Generate URL with all options from the provider to can be redirect to the provider authentication page
        /// </summary>
        /// <returns>callbackPath are Mandatory, so that function will return error</returns>
        public string GetUrlToAuthenticate ()
        {
            return GetUrlToAuthenticate(String.Empty, String.Empty);
        }

        /// <summary>
        /// Generate URL with all options from the provider to can be redirect to the provider authentication page
        /// </summary>
        /// <param name="callbackPath">Url on our application to receive the request after login on external provider</param>
        /// <returns>Url to redirect our application to provider authentication page</returns>
        public string GetUrlToAuthenticate(string callbackPath, string path)
        {
            if (!String.IsNullOrEmpty(callbackPath))
                Options.CallbackPath = callbackPath;

            var uriBuilder = new UriBuilder(Options.Authority);
            if (!String.IsNullOrEmpty(path))
                uriBuilder.Path = path;
            var parameters = HttpUtility.ParseQueryString(string.Empty);
            if (!String.IsNullOrEmpty(Options.CallbackPath))
                parameters["TARGET"] = Options.CallbackPath;
            uriBuilder.Query = parameters.ToString();

            return uriBuilder.Uri.ToString();
        }

        /// <summary>
        /// Will check credentials and will find the "authenticated" user are on our application
        /// </summary>
        /// <param name="credential">Token identification to user on external provider</param>
        /// <returns>Internal Identity when user are found and success login on external provider</returns>
        public IIdentity Authenticate(Credential credential)
        {
            string usernameCred = "";

            if (!(credential is TokenCASCredential) || String.IsNullOrEmpty(((TokenCASCredential)credential).Token))
                return null;

            //Find on response from CAS server the username
            XmlDocument xmlReturn = getResponseCAS(((TokenCASCredential)credential).Token, ((TokenCASCredential)credential).OriginUrl);
            XmlNodeList userAttrib = xmlReturn.GetElementsByTagName("saml1:Attribute");
            foreach (XmlNode xmlNode in userAttrib)
            {
                if (xmlNode.Attributes["AttributeName"].Value.Equals(Options.AttribValidation))
                { 
                    usernameCred = xmlNode.InnerText;
                    break;
                }
            }

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
                    id = Authenticate(usernameCred, sp);
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
        /// Method that validates the authentication of a user and returns all data related to the same
        /// https://stackoverflow.com/questions/4791794/client-to-send-soap-request-and-receive-response
        /// </summary>
        /// <param name="ticket">Validator identifier returned in the first CAS request</param>
		/// <param name="originUrl">Place of origin from which the order was placed, the CAS service validates that the ticket is valid from the place where the order was requested</param>
        /// <returns>The validation response xml with all authenticated user data</returns>
        private XmlDocument getResponseCAS(string ticket, string originUrl)
        {
            XmlDocument soapEnvelopeXml = createSoapEnvelope(ticket);
            HttpWebRequest request = createWebRequest((new CASIdentityProvider()).GetUrlToAuthenticate(
                originUrl,
                "samlValidate" //path to contact
                ));
			request.ContentLength = Encoding.UTF8.GetByteCount(soapEnvelopeXml.OuterXml);
            insertSoapEnvelopeIntoWebRequest(soapEnvelopeXml, request);

            //Log.Error("Request CAS:" + Environment.NewLine + soapEnvelopeXml.OuterXml);

            // get the response from the completed web request
            string soapResult = "";
            using (WebResponse webResponse = request.GetResponse())
            {
                using (StreamReader rd = new StreamReader(webResponse.GetResponseStream()))
                {
                    soapResult = rd.ReadToEnd();
                }
            }

            //Log.Error("Result CAS:" + Environment.NewLine + soapResult);

            //convert response from server to xmldocument
            XmlDocument xmlCASResult = null;
            if (!String.IsNullOrEmpty(soapResult))
            {
                try
                {
                    xmlCASResult = new XmlDocument();
                    xmlCASResult.LoadXml(soapResult);
                }
                catch
                { 
                    xmlCASResult = null; 
                }
            }

            return xmlCASResult;
        }

        private static HttpWebRequest createWebRequest(string url)
        {
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url);
            webRequest.Method = "POST";
            webRequest.ContentType = "text/xml;charset=\"utf-8\"";
            webRequest.CookieContainer = new CookieContainer();
            webRequest.Headers.Add("SOAPAction", "http://www.oasis-open.org/committees/security");
            webRequest.CachePolicy = new System.Net.Cache.RequestCachePolicy(System.Net.Cache.RequestCacheLevel.NoCacheNoStore);
            return webRequest;
        }

        private XmlDocument createSoapEnvelope(string ticket)
        {
            XmlDocument soapEnvelopeDocument = new XmlDocument();
            soapEnvelopeDocument.LoadXml(
                    @"<SOAP-ENV:Envelope xmlns:SOAP-ENV=""http://schemas.xmlsoap.org/soap/envelope/"" 
                       xmlns:xsi=""http://www.w3.org/1999/XMLSchema-instance"" 
                       xmlns:xsd=""http://www.w3.org/1999/XMLSchema"">
                        <SOAP-ENV:Body>
                            <samlp:Request xmlns:samlp=""urn:oasis:names:tc:SAML:1.0:protocol"" 
                                MajorVersion=""1"" 
                                MinorVersion=""1"" 
                                RequestID=""" + Guid.NewGuid() + @""" 
                                IssueInstant=""" + DateTime.Now.ToString() + @""">
									<samlp:AssertionArtifact>" + ticket + @"</samlp:AssertionArtifact>
							</samlp:Request>
                        </SOAP-ENV:Body>
                      </SOAP-ENV:Envelope>");
            return soapEnvelopeDocument;
        }

        private static void insertSoapEnvelopeIntoWebRequest(XmlDocument soapEnvelopeXml, HttpWebRequest webRequest)
        {
            using (Stream stream = webRequest.GetRequestStream())
            {
                stream.Write(Encoding.UTF8.GetBytes(soapEnvelopeXml.OuterXml), 0, soapEnvelopeXml.OuterXml.Length);
            }
        }

        private IIdentity Authenticate(string username, PersistentSupport sp)
        {
            try
            {
                SelectQuery select = new SelectQuery()
                    .Select("psw", "status")
                    .Select("psw", "nome")
                    .From(Area.AreaPSW)
                    .Where(CriteriaSet.And().Equal("psw", "nome", username));

                var results = sp.executeReaderOneRow(select);
                if (results.Count == 0)
                    return null;
                int status = DBConversion.ToInteger(results[0]);
                string name = DBConversion.ToString(results[1]);

                if (status == 2)
                    return null;

                return new GenericIdentity(name);
            }
            catch 
            {
                return null;
            }
        }
    }
}
