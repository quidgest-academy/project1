using CSGenio.business;
using CSGenio.persistence;
using Fido2NetLib;
using Fido2NetLib.Development;
using Fido2NetLib.Objects;
using Newtonsoft.Json;
using Quidgest.Persistence.GenericQuery;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace GenioServer.security
{
    public class WebAuthValues
    {
        public string MDSAccessKey { get; set; }
        public string MDSCacheDirPath { get; set; }
        public string TimestampDriftTolerance { get; set; }

        public WebAuthFido2Options Fido2Options { get; set; }
    }

    public class WebAuthFido2Options
    {
        private string serverDomain;
        public string ServerDomain
        {
            get
            {
                if (String.IsNullOrEmpty(serverDomain) && !String.IsNullOrEmpty(Origin) && Origin.IndexOf("://") != -1)
                {
                    Uri hostname = new Uri(Origin);
                    if (!String.IsNullOrEmpty(hostname.Host))
                        return hostname.Host;
                }
                return serverDomain;
            }
            set { serverDomain = value; }
        }
        public string ServerName { get; set; } = "Fido2";

        public string Origin { get; set; }
    }

    public class WebAuthReturn
    {
        public string Options { get; set; }
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
    }

    [CredentialProvider(typeof(TokenCredential))]
    [Description("Establishes identity using WebAuthN using hardware.")]
    [DisplayName("WebAuthN")]
    public class WebAuthIdentityProvider : IIdentityProvider
    {
        public IIdentity Authenticate(Credential credential)
        {
            throw new NotImplementedException();
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

        private Fido2 _lib;
        public static IMetadataService _mds;

        public WebAuthIdentityProvider(WebAuthValues valuesWA)
        {
			//Currently unused code
            //var MDSAccessKey = valuesWA.MDSAccessKey;
            //var MDSCacheDirPath = valuesWA.MDSCacheDirPath ?? Path.Combine(Path.GetTempPath(), "fido2mdscache");
            _mds = null; //_mds = string.IsNullOrEmpty(MDSAccessKey) ? null : MDSMetadata.Instance(MDSAccessKey, MDSCacheDirPath); //At this moment we are using MDSAccessKey with empty so I will assign null to _mds directly
            //Currently unused code
			/*
			if (null != _mds)
            {
                if (false == _mds.IsInitialized())
                    _mds.Initialize().Wait();
            }
			*/
            _lib = new Fido2(new Fido2Configuration()
            {
                ServerDomain = valuesWA.Fido2Options.ServerDomain,
                ServerName = valuesWA.Fido2Options.ServerName,
                Origin = valuesWA.Fido2Options.Origin,
                // Only create and use Metadataservice if we have an acesskey
                MetadataService = _mds,
                TimestampDriftTolerance = Convert.ToInt32(valuesWA.TimestampDriftTolerance)
            });
        }

        //****** To register new token *********
        public WebAuthReturn MakeCredentialOptions(string username)
        {
            try
            {
                //Define general options
                var attestation_type = AttestationConveyancePreference.None; // possible values: none, direct, indirect

                var authenticatorSelection = new AuthenticatorSelection
                {
                    RequireResidentKey = false, // possible values: true,false
                    UserVerification = UserVerificationRequirement.Preferred // possible values: preferred, required, discouraged
                    //AuthenticatorAttachment = AuthenticatorAttachment.Platform // possible values: <empty>, platform, cross-platform
                };

                var exts = new AuthenticationExtensionsClientInputs()
                {
                    Extensions = true,
                    UserVerificationIndex = true,
                    Location = true,
                    UserVerificationMethod = true,
                    BiometricAuthenticatorPerformanceBounds = new AuthenticatorBiometricPerfBounds
                    {
                        FAR = float.MaxValue,
                        FRR = float.MaxValue
                    }
                };

                //Get actual user
                Fido2User user = new Fido2User
                {
                    DisplayName = username,
                    Name = username,
                    Id = Encoding.UTF8.GetBytes(username) // byte representation of userID is required
                };

                // Create options
                var options = _lib.RequestNewCredential(user, new List<PublicKeyCredentialDescriptor>(), authenticatorSelection, attestation_type, exts); //existingKeys???

                //Temporarily store options, session/in-memory cache/redis/db
                //HttpContext.Session["fido2.attestationOptions"] = Newtonsoft.Json.JsonConvert.SerializeObject(options); //This will need on controller

                // return options to client
                return new WebAuthReturn()
                {
                    Success = true,
                    Options = Newtonsoft.Json.JsonConvert.SerializeObject(options),
                    ErrorMessage = null
                };
            }
            catch (Exception e)
            {
                return new WebAuthReturn()
                {
                    Success = false,
                    Options = null,
                    ErrorMessage = e.Message
                };
            }
        }

        public async Task<WebAuthReturn> MakeCredential(string data, string jsonOptions, string userCodPsw, PersistentSupport sp)
        {
            try
            {
                AuthenticatorAttestationRawResponse attestationResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<AuthenticatorAttestationRawResponse>(data);

                // 1. Get the assertion options we sent the client
                var options = CredentialCreateOptions.FromJson(jsonOptions);


                // 2. Create callback so that lib can verify credential id is unique to this user
                IsCredentialIdUniqueToUserAsyncDelegate callback = async (IsCredentialIdUniqueToUserParams args) =>
                {
                    //At this momment, we will only accept one credential by user
                    return await Task.FromResult(true);
                };

                // 2. Verify and make the credentials
                var success = await _lib.MakeNewCredentialAsync(attestationResponse, options, callback);

                // 3. Store the credentials in db
                if (success.Status != "ok")
                    return new WebAuthReturn()
                    {
                        Success = false,
                        Options = null,
                        ErrorMessage = success.ErrorMessage
                    };


                //save the 2FA secret and type
                var storedCred = new StoredCredential
                {
                    Descriptor = new PublicKeyCredentialDescriptor(success.Result.CredentialId),
                    PublicKey = success.Result.PublicKey,
                    UserHandle = success.Result.User.Id,
                    SignatureCounter = success.Result.Counter,
                    CredType = success.Result.CredType,
                    RegDate = DateTime.Now,
                    AaGuid = success.Result.Aaguid
                };

                UpdateQuery upd = new UpdateQuery()
                .Update(Area.AreaPSW)
                .Set(CSGenioApsw.FldPsw2fatp, GenioServer.security.Auth2FAModes.WebAuth.ToString())
                .Set(CSGenioApsw.FldPsw2favl, Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(storedCred))))
                .Where(CriteriaSet.And().Equal("psw", "codpsw", userCodPsw));
                bool openConn = true;
                if (sp.TransactionIsClosed)
                {
                    sp.openConnection();
                    openConn = false;
                }
                sp.Execute(upd);
                if (!openConn)
                    sp.closeConnection();

                // 4. return "ok" to the client
                return new WebAuthReturn()
                {
                    Success = true,
                    Options = Newtonsoft.Json.JsonConvert.SerializeObject(success),
                    ErrorMessage = null
                };
            }
            catch (Exception e)
            {
                return new WebAuthReturn()
                {
                    Success = false,
                    Options = null,
                    ErrorMessage = e.Message
                };
            }
        }

        //****** To login  *********
        public WebAuthReturn AssertionOptionsPost(string userCodPsw, PersistentSupport sp)
        {
            try
            {
                var existingCredentials = new List<PublicKeyCredentialDescriptor>();

                // 1. Get user from DB
                SelectQuery select = new SelectQuery()
                .Select("psw", "psw2favl")
                .From(Area.AreaPSW)
                .Where(CriteriaSet.And().Equal("psw", "codpsw", userCodPsw));

                var results = sp.executeReaderOneRow(select);
                if (results.Count == 0)
                    return null;

                string jsonCredVl = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(DBConversion.ToString(results[0])));
                var storedCred = Newtonsoft.Json.JsonConvert.DeserializeObject<StoredCredential>(jsonCredVl);

                //2. Get registered credentials from database
                existingCredentials.Add(storedCred.Descriptor);

                var exts = new AuthenticationExtensionsClientInputs()
                {
                    SimpleTransactionAuthorization = "FIDO",
                    GenericTransactionAuthorization = new TxAuthGenericArg
                    {
                        ContentType = "text/plain",
                        Content = new byte[] { 0x46, 0x49, 0x44, 0x4F }
                    },
                    UserVerificationIndex = true,
                    Location = true,
                    UserVerificationMethod = true
                };

                // 3. Create options
                var uv = UserVerificationRequirement.Discouraged; //string.IsNullOrEmpty(userVerification) ? UserVerificationRequirement.Discouraged : userVerification.ToEnum<UserVerificationRequirement>();
                var options = _lib.GetAssertionOptions(existingCredentials, uv, exts);

                // 4. Temporarily store options, session/in-memory cache/redis/db
                //HttpContext.Session.SetString("fido2.assertionOptions", options.ToJson());  //This will need on controller

                // 5. Return options to client
                return new WebAuthReturn()
                {
                    Success = true,
                    Options = Newtonsoft.Json.JsonConvert.SerializeObject(options),
                    ErrorMessage = null
                };
            }

            catch (Exception e)
            {
                return new WebAuthReturn()
                {
                    Success = false,
                    Options = null,
                    ErrorMessage = e.Message
                };
            }
        }

        public async Task<WebAuthReturn> MakeAssertion(string data, string jsonOptions, string userCodPsw, PersistentSupport sp)
        {
            try
            {
                AuthenticatorAssertionRawResponse assertionResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<AuthenticatorAssertionRawResponse>(data);

                // 1. Get the assertion options we sent the client
                var options = AssertionOptions.FromJson(jsonOptions);

                // 2. Get user from DB
                SelectQuery select = new SelectQuery()
                .Select("psw", "psw2favl")
                .From(Area.AreaPSW)
                .Where(CriteriaSet.And().Equal("psw", "codpsw", userCodPsw));

                var results = sp.executeReaderOneRow(select);
                if (results.Count == 0)
                    return null;

                string jsonCredVl = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(DBConversion.ToString(results[0])));
                var storedCred = Newtonsoft.Json.JsonConvert.DeserializeObject<StoredCredential>(jsonCredVl);

                // 3. Get credential counter from database
                var storedCounter = storedCred.SignatureCounter;

                // 4. Create callback to check if userhandle owns the credentialId
                IsUserHandleOwnerOfCredentialIdAsync callback = async (args) =>
                {
                    //At this momment, we will only accept one credential by user
                    return await Task.FromResult(false);
                };

                // 5. Make the assertion
                var res = await _lib.MakeAssertionAsync(assertionResponse, options, storedCred.PublicKey, storedCounter, callback);

                // 6. Store the updated counter
                storedCred.SignatureCounter = res.Counter;
                UpdateQuery upd = new UpdateQuery()
                .Update(Area.AreaPSW)
                .Set(CSGenioApsw.FldPsw2favl, Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(storedCred))))
                .Where(CriteriaSet.And().Equal("psw", "codpsw", userCodPsw));
                bool openConn = true;
                if (sp.TransactionIsClosed)
                {
                    sp.openConnection();
                    openConn = false;
                }
                sp.Execute(upd);
                if (!openConn)
                    sp.closeConnection();

                // 7. return OK to client
                return new WebAuthReturn()
                {
                    Success = true,
                    Options = Newtonsoft.Json.JsonConvert.SerializeObject(res),
                    ErrorMessage = null
                };
            }
            catch (Exception e)
            {
                return new WebAuthReturn()
                {
                    Success = false,
                    Options = null,
                    ErrorMessage = e.Message
                };
            }
        }
    }
}
