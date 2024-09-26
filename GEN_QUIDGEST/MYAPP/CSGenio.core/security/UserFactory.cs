using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using CSGenio.framework;
using CSGenio.business;
using CSGenio.persistence;
using Quidgest.Persistence.GenericQuery;
using System.Threading;
using CSGenio;
using System.Net.Mail;

namespace GenioServer.security

{
    public class UserFactory
    {

        private readonly PersistentSupport sp;
        private readonly User user;

        public UserFactory(PersistentSupport sp, User user)
        {
            this.sp = sp;
            this.user = user;
        }



		private static readonly string[] AVAILABLE_MODULES = new string[]
            {
                "PRO"
            };

        public static readonly Tuple<string, Role>[] MODULE_ROLES = new Tuple<string, Role>[]
        {
            new Tuple<string,Role>("PRO", Role.UNAUTHORIZED),

			new Tuple<string,Role>("PRO", Role.ROLE_1),
			new Tuple<string,Role>("PRO", Role.ROLE_2),
			new Tuple<string,Role>("PRO", Role.ROLE_3),
			new Tuple<string,Role>("PRO", Role.ADMINISTRATION)        };

        [Obsolete]
        private static readonly string[] AVAILABLE_ROLES = new string[]
            {
                "PRO.0"
				, "PRO.1"
				, "PRO.2"
				, "PRO.3"
				, "PRO.99"
            };

        private static List<string> GetYears(IPrincipal principal)
        {
            List<string> anos = new List<string>();
            foreach (var Qyear in Configuration.Years)
                if (principal.IsInRole(Qyear))
                    anos.Add(Qyear);
            return anos;
        }


        public static IUserBusinessManager BusinessManager { get; set; }


        /// <summary>
        /// Returns a user with the given userName from the login table. Returns null if the userid doesn't exist
        /// </summary>
        /// <param name="sp">Persistent support</param>
        /// <param name="user">Framework user</param>
        /// <param name="userName">User name to search by</param>
        /// <returns></returns>
        public CSGenioApsw GetUser(string userName)
        {
            var list = CSGenioApsw.searchList(sp, user, CriteriaSet.And().Equal(CSGenioApsw.FldNome, userName));
            if (list.Count == 0)
                return null;
            else
                return list[0];
        }

        /// <summary>
        /// Creates a new psw record while respecting security configurations.
        /// </summary>
        /// <param name="userName">User name to create</param>
        /// <param name="email">Email of the use</param>
        /// <param name="phone">User mobile number</param>
        /// <param name="status">Status of the psw record</param>
        /// <param name="password">Password to be added. Put null if you don't wish to specify a password</param>
        /// <returns>A UserLogin record with all necessary information. The record is not yet persisted.</returns>
        public CSGenioApsw CreateNewPsw(string userName, string email, string phone, int status,
            Password password)
        {
            //inserção de um novo user
            CSGenioApsw userPsw = new CSGenioApsw(user);
            FillPsw(userPsw, userName, email, phone, status, password);
            return userPsw;
        }

        /// <summary>
        /// Creates a new psw record while respecting security configurations.
        /// </summary>
        /// <param name="userName">User name to create</param>
        /// <param name="email">Email of the use</param>
        /// <param name="phone">User mobile number</param>
        /// <param name="status">Status of the psw record</param>
        /// <param name="password">Password to be added. Put null if you don't wish to specify a password</param>
        public void FillPsw(CSGenioApsw userPsw, string userName, string email, string phone, int status, Password password)
        {
			//NH(2021.12.20)- remove unnecessary spaces
            userPsw.ValNome = userName.Trim();
            if (!string.IsNullOrWhiteSpace(email))
                userPsw.ValEmail = email.Trim();

            if (!string.IsNullOrWhiteSpace(phone))
                userPsw.ValPhone = phone.Trim();
            userPsw.ValStatus = status;

            //Check for duplicated users
            if (GetUser(userPsw.ValNome) != null)
            {
                string msg = Translations.GetByCode("O_NOME_DE_UTILIZADOR53911", user.Language);
                throw new FrameworkException(msg, "CreateNewPsw", msg);
            }

            if (password != null && !string.IsNullOrWhiteSpace(password.New))
            {
                string error = CheckNewPassword(userPsw.ValNome, password.New, password.Confirm);
                if (error != "")
                    throw new InvalidPasswordException(error, "UserRegistration.CreateNewUser", error);

                FillPassword(userPsw, password.New);
            }
        }

        /// <summary>
        /// Changes the password for a psw, checking all necessary security configurations.
        /// </summary>
        /// <param name="userPsw">A psw record</param>
        /// <param name="newPass">A password object</param>
        /// <param name="confirmPass">Repeat the password object</param>
        /// <param name="oldPass">Optionally check if the user knows the old password</param>
        /// <returns>The changed psw record. The changes will not be persisted</returns>
        public void ChangePassword(CSGenioApsw userPsw, string newPass, string confirmPass, string oldPass=null)
        {
            if (newPass != null)
            {
                string error = CheckChangePassword(userPsw, oldPass, newPass, confirmPass);
                if (error != "")
                    throw new InvalidPasswordException(error, "UserRegistration.CreateNewUser", error);

                FillPassword(userPsw, newPass);
            }
        }

        private static void FillPassword(CSGenioApsw userPsw, string newPass)
        {
            string pswEnc = PasswordFactory.Encrypt(newPass);
            userPsw.ValPassword = pswEnc; //Neste momento é gravado com o salt to facilitar na transição
            userPsw.ValSalt = "";
            userPsw.ValPswtype = Configuration.Security.PasswordAlgorithms.ToString();
            userPsw.ValDatexp = CalculateExpirationDate();
            userPsw.ValStatus = 0;
        }

        private string CheckChangePassword(CSGenioApsw userPsw, string oldPass, string newPass, string confirmPass)
        {
            //in some interfaces (like password recovery, the interface allows a password change without supplying the old password)
            if (oldPass != null)
            {
                if (userPsw.ValPassword != "" && !PasswordFactory.IsOK(oldPass, userPsw.ValPassword, userPsw.ValSalt, userPsw.ValPswtype)) //Verificar password antiga é a verdadeira
                    return Translations.GetByCode("A_PASSWORD_ANTIGA_NA50246", user.Language);
                if (oldPass == newPass || oldPass == confirmPass)
                    return Translations.GetByCode("A_NOVA_PALAVRA_PASSE58485", user.Language);
            }

            return CheckNewPassword(userPsw.ValNome, newPass, confirmPass);
        }

        private string CheckNewPassword(string username, string pass, string confirm)
        {
            if (pass == null) // null password protection
                pass = "";

            if (pass != confirm)
                return Translations.GetByCode("A_NOVA_PALAVRA_CHAVE41230", user.Language);
            if (pass.ToUpper() == username.ToUpper())
                return Translations.GetByCode("ATENCAO__NAO_PODE_CO49745", user.Language);

            return CheckPasswordRules(pass);
        }


        public string CheckPasswordRules(string pass)
        {
            string error = CheckPasswordChars(pass);
            if (!string.IsNullOrEmpty(error)) return error;
            error = CheckPasswordStrength(pass);
            if (!string.IsNullOrEmpty(error)) return error;
            error = CheckBlacklisted(pass);
            return error;
        }

        private string CheckPasswordChars(string pass)
        {
            if (Int32.TryParse(Configuration.Security.MinCharacters, out int minChar) && minChar > 0 && pass.Length < minChar)
                return string.Format(Translations.GetByCode("A_PALAVRA_PASSE_NAO_06382", user.Language) , Configuration.Security.MinCharacters);
            return "";
        }

        private string CheckPasswordStrength(string pass)
        {
            var configStrength = Configuration.Security.PasswordStrength;
            if (configStrength == PasswordStrength.Pobre)
                return "";

            double pswStrength = PasswordFactory.scorePassword(pass);
            
            if (!((configStrength == PasswordStrength.Forte && pswStrength > 80) ||
                (configStrength == PasswordStrength.Bom && pswStrength > 60) ||
                (configStrength == PasswordStrength.Fraco && pswStrength >= 30)))
                return string.Format(Translations.GetByCode("A_PALAVRA_PASSE_NAO_59708", user.Language), Configuration.Security.PasswordStrength.ToString());
            return "";
        }

        /// <summary>
        /// Checks if a passord is part of the blacklist
        /// </summary>
        /// <param name="pass">the password to check</param>
        /// <returns>Empty string if everything is ok. An error message if the password is blacklisted.</returns>
        public string CheckBlacklisted(string pass)
        {
            if (!Configuration.Security.UsePasswordBlacklist)
                return "";

            SelectQuery select = new SelectQuery()
                .Select(SqlFunctions.Count(1), "COUNT")
                .From("PswBlacklist")
                .Where(CriteriaSet.And().Equal("PswBlacklist", "pass", pass.ToLowerInvariant()));
            var ct = DBConversion.ToInteger(sp.executeScalar(select));
            if (ct > 0)
                return Translations.GetByCode("PASSWORD_VULNERAVEL_00083", user.Language);
            
            return "";
        }



        /// <summary>
        /// Gets an user from a specified email. Returns null if no user exists with that email.
        /// </summary>
        /// <param name="email">The user email</param>
        public CSGenioApsw GetUserFromEmail(string email)
        {
            var pswList = CSGenioApsw.searchList(sp, user, CriteriaSet.And().Equal(CSGenioApsw.FldEmail, email));

            /*Change by [TMV] - The old code didn't make sense when finding more than one email returned the first user found.
             * It now returns null in those cases
             * */
            if (pswList.Count == 0)
            {
                Log.Error($"No user found for the email {email}");
                return null;
            }
            else if (pswList.Count > 1)
            {
                Log.Error($"Found more than one user with email {email}");
                return null;
            }

            return pswList[0];
        }

        /// <summary>
        /// Sends an email for password recovery.
        /// </summary>
        /// <param name="email">The user email</param>
        /// <param name="email">A string with the desired html to send</param>
        public void SendPasswordRecoveryMail(string emailDestination, string mailContent)
        {
            //Check if the passowrd recovery email is configured
            string passwordRecoveryServer = Configuration.PasswordRecoveryEmail;
            if (string.IsNullOrEmpty(passwordRecoveryServer))
                throw new FrameworkException("There is no email server configuration for password recovery", "", "There is no email server configuration for password recovery");

            //Check if the user registration still exist
            var emailServer = Configuration.EmailProperties.Find(x => x.Id == passwordRecoveryServer);
            if (emailServer == null)
                throw new FrameworkException("There is no email server configuration for password recovery", "", "There is no email server configuration for password recovery");

            string subject = Translations.Get("Password Recovery", user.Language);

            var body = System.Net.Mail.AlternateView.CreateAlternateViewFromString(mailContent, null, System.Net.Mime.MediaTypeNames.Text.Html);
            UserRegistration.SendEmail(emailServer, emailDestination, subject, body);
        }

		public void SendUserInactivationEmail(CSGenioApsw loginInfo, string ConfirmURL, string language)
        {
            ResourceUser rec = new ResourceUser(loginInfo.ValNome, loginInfo.ValCodpsw);
            string recSer = QResources.CreateTicketEncryptedBase64(loginInfo.ValNome, loginInfo.User.Location, rec);
            ConfirmURL = ConfirmURL.Replace("fldTicket", recSer);
            var subject = Translations.Get("Deactivação de utilizador", user.Language);
            var keywords = new Dictionary<string, string>()
            {
                {"fldName", loginInfo.ValNome},
                {"fldToken", ConfirmURL},
            };

            var path = AppDomain.CurrentDomain.BaseDirectory;
            var linkedResources = new Dictionary<string, string>()
            {
                {"logo", path + "Content\\Email\\Logo.png"}
            };

            var body = UserRegistration.CreateBodyFromTemplate("InactivationEmailTemplate", language, keywords, linkedResources);
            TemplatedMailSender(loginInfo.ValEmail, subject, body, user.Language);
        }

		public static void MailSender(CSGenioApsw user, string ConfirmURL, string lang = "")
        {
			//Check if the user registration email is configured
            string userRegistrationEmail = Configuration.UserRegistrationEmail;
            if (string.IsNullOrEmpty(userRegistrationEmail))
                throw new FrameworkException("There is no email server configuration for user registrations", "", "There is no email server configuration for user registrations");

            //Check if the user registration still exist
            var emailServer = Configuration.EmailProperties.Find(x => x.Id == userRegistrationEmail);
            if(emailServer == null)
                throw new FrameworkException("There is no email server configuration for user registrations", "", "The defined email server configuration doesn't exist");

            else
            {
                try
                {
                    ResourceUser rec = new ResourceUser(user.ValNome, user.ValCodpsw);
                    string recSer = QResources.CreateTicketEncryptedBase64(user.ValNome, user.User.Location, rec);
                    var body = UserRegistration.CreateBody(AppDomain.CurrentDomain.BaseDirectory, ReplaceLastOccurrence(ConfirmURL, "fldTicket", recSer), user.ValNome, lang);
                    string subject = Translations.Get("Confirmação de endereço de email", user.User.Language);

                    // USE /[MANUAL PRO CUSTOM_USER_REGISTRATION]/
                    UserRegistration.SendEmail(emailServer, user.ValEmail, subject , body);
                }
                catch (Exception e)
                {

                    throw new BusinessException(Translations.Get("Não foi possível enviar o email.", user.User.Language), "UserFactory.MailSender", "Email wasn't sent.", e);
                }
            }

        }


		public static string ReplaceLastOccurrence(string Source, string Find, string Replace)
        {
            int place = Source.LastIndexOf(Find);

            if (place == -1)
                return Source;

            string result = Source.Remove(place, Find.Length).Insert(place, Replace);
            return result;
        }



		public static User UpdateEPH(User user, string ephID, string[] ephValues)
        {
            bool userHasEph = user.hasEph(ephID);

            if (!userHasEph)
                throw new BusinessException("O utilizador não tem o EPH definido.", "UserFactory.UpdateEPH", "O utilizador não tem o EPH definido.");
            else
                user.SetEph(user.CurrentModule, ephID, ephValues);

            return user;
        }

        public static string[] GetEPH(User user, string ephID)
        {
            return user.GetEph(user.CurrentModule, ephID);
        }

        public static User FillUser(IPrincipal principal, User user)
        {
            return FillUser(principal, user, "");
        }

        public static bool IsGuest(string username)
        {
            if (Configuration.Security == null || Configuration.Security.Users == null)
            {
                return false;
            }

            return Configuration.Security.Users.Find((x) => x.Name == username && x.Type == UserType.Guest) != null;
        }

		public static bool IsGuest(IIdentity identity)
		{
			return IsGuest(identity.Name);
		}

        public static User FillUser(IPrincipal principal, User user, string location)
        {
			// USE /[MANUAL PRO USERPROVIDER]/
			System.Diagnostics.Debug.Assert(principal != null);
            System.Diagnostics.Debug.Assert(!(principal is ErrorPrincipal));

            if(user == null){
              user = new User(principal.Identity.Name, "guest", Configuration.DefaultYear, location);
              user.Language = Thread.CurrentThread.CurrentCulture.Name.Replace("-", "").ToUpperInvariant();
            }

			if (IsGuest(principal.Identity))
			{
				foreach (string module in AVAILABLE_MODULES)
				{
                    user.AddModuleRole(module, Role.UNAUTHORIZED);
				}

                user.AddModuleRole("Public", Role.UNAUTHORIZED);

				return user;
			}

            Exception lastException = null;
            bool sucess = false;
            string _year = user.Year, firstAvailableYear = string.Empty;
            foreach (string Qyear in Configuration.Years)
            {
                if (principal.IsInRole(Qyear))
                {
                    user.Year = Qyear;

                    try
                    {
                        user = fillUser(principal, user);
                        sucess = true;
                        if (string.IsNullOrEmpty(firstAvailableYear))
                            firstAvailableYear = Qyear;
                    }
                    catch (Exception e)
                    {
                        lastException = e; //guarda a excepção e tenta o proximo Qyear
                    }
                }
            }

            user.Year = _year;
            //filtrar a lista de anos
            user.Years = GetYears(principal);

            if (!principal.IsInRole(_year) && sucess)
                user.Year = firstAvailableYear;

            // caso não tenhamos entrado em nenhum Qyear relançamos a excepção
            // entra aqui caso as autorizações nem tenham roles to nenhum Qyear
            if (!sucess)
            {
                user.Public = true;
                user.Year = Configuration.DefaultYear;
                if (lastException != null)
                    throw lastException;
                else
                    throw new BusinessException("O utilizador não pode aceder a nenhum módulo web.", "GlobalFunctions.logonEXW", "O utilizador não pode aceder a nenhum módulo web.");
            }

            return user;
        }

		private static void TemplatedMailSender(string destination, string subject, AlternateView body, string language)
        {
            //Check if the user registration email is configured
            string userRegistrationEmail = Configuration.UserRegistrationEmail;
            if (string.IsNullOrEmpty(userRegistrationEmail))
                throw new FrameworkException("There is no email server configuration for user registrations", "", "There is no email server configuration for user registrations");

            //Check if the user registration still exist
            var emailServer = Configuration.EmailProperties.Find(x => x.Id == userRegistrationEmail);
            if (emailServer == null)
            {
                throw new FrameworkException("There is no email server configuration for user registrations", "", "The defined email server configuration doesn't exist");
            }

            try
            {
                UserRegistration.SendEmail(emailServer, destination, subject, body);
            }
            catch (Exception e)
            {
                throw new BusinessException(Translations.Get("Não foi possível enviar o email.", language), "UserFactory.TemplatedMailSender", "Email wasn't sent.", e);
            }
        }

		private static User fillUser(IPrincipal principal, User user)
        {
            PersistentSupport sp = null;
            try
            {
                if (!principal.IsInRole(user.Year))
                    throw new BusinessException("Login não foi encontrado.", "GlobalFunctions.logonEXW", "Não foi possivel encontrar a chave correspondente ao login " + principal.Identity.Name);
                sp = PersistentSupport.getPersistentSupport(user.Year, user.Name);
                CSGenioApsw psw = SecurityFactory.GetUser(principal.Identity,user,sp);

                sp.openConnection();
                //introduce o name dos modulos
                //UserFactory factory = new UserFactory(sp, user);
                //CSGenioApsw psw = factory.GetUser(principal.Identity.Name);

                if (psw == null)
                    throw new BusinessException("Login não foi encontrado.", "UserFactory.fillUser", "Não foi possivel encontrar o registo correspondente ao login " + principal.Identity.Name);

                string tpPsw2FA = psw.ValPsw2fatp;

                user.Name = principal.Identity.Name;
                user.Codpsw = psw.ValCodpsw;
                user.Status = (int)psw.ValStatus;
                user.Auth2FA = !(string.IsNullOrEmpty(tpPsw2FA) || tpPsw2FA == GenioServer.security.Auth2FAModes.None.ToString());
                user.Auth2FATp = user.Auth2FA ? tpPsw2FA: "";
                bool hasAnyModule = false;
                Hashtable ephs = new Hashtable(); //variavel que vai ter as ephs do user
                List<Tuple<string, EPHCondition>> ephsToFill = new List<Tuple<string, EPHCondition>>();
                //Dictionary<string,EPHCondition> ephsToFill = new Dictionary<string, EPHCondition>(); //variavel que vai ter as ephs iniciais do user
                foreach (var moduleRole in MODULE_ROLES)
                {
                    string module = moduleRole.Item1;
                    Role role = moduleRole.Item2;
                    if (principal.IsInRole(user.Year + "." + module + "." + role.Id))
                    {
                        hasAnyModule = true;
                        //retirar todas as condições eph que o user está sujeito em cada module, tendo em conta o seu level
                        EPH eph = EPH.getEPH(module);
                        if (eph != null)
                        {
                            if (!role.Equals(Role.UNAUTHORIZED))// se não é desautorizado, se é desautorizado não se adiciona nada
                            {
                                EPHCondition[] condicoesEPH = (EPHCondition[])eph.EphsPerModule[role.ToString()];
                                if (condicoesEPH != null)//se exists condição eph do módulo to esse level
                                {
                                    bool haAssocicaoEPH = true;
                                    foreach (EPHCondition condition in condicoesEPH)
                                    {
                                        if (String.IsNullOrWhiteSpace(condition.IntialForm))
                                        {
                                            string[] valoresEPH;//variavel com as conditions eph to este module
                                            valoresEPH = sp.ValuesEPH(user.Codpsw, condition, module);
                                            string ephKey = module + "_" + condition.EPHName;
                                            if (valoresEPH.Length > 0 && !ephs.Contains(ephKey))
                                            {
                                                ephs.Add(ephKey, valoresEPH);
                                            }
                                            else if (valoresEPH.Length == 0)
                                            {
                                                haAssocicaoEPH = false;
                                                break;
                                            }
                                        }
                                        else
                                        {
                                            if(haAssocicaoEPH)
                                                ephsToFill.Add( new Tuple<string, EPHCondition>(module, condition));
                                        }
                                    }
                                    if (haAssocicaoEPH)
                                    {
                                        user.AddModuleRole(module.ToUpper(), role);

                                        if (ephsToFill.Count > 0)
                                        {
                                            if(user.EphTofill == null)
                                                user.EphTofill = new EphsToFill();
                                            foreach (Tuple<string, EPHCondition> keyValue in ephsToFill)
                                                user.EphTofill.AddNew(keyValue.Item1, keyValue.Item2);
                                        }
                                    }

                                }
                                else
                                {
                                    user.AddModuleRole(module.ToUpper(), role);
                                }
                            }
                        }
                        else//se não é sujeito a eph não se deixa aceder se for desautorizado
                        {
                            if (!role.Equals(Role.UNAUTHORIZED))
                            {
                                user.AddModuleRole(module.ToUpper(), role);
                            }
                        }
                    }
                }
                user.AddModuleRole("Public", Role.UNAUTHORIZED);
                if (!hasAnyModule)//se não existem módulo definidos
                {
                    throw new BusinessException("Não existem módulos web definidos.", "GlobalFunctions.logonEXW", "Não existem módulos web definidos.");
                }
                if (ephs.Count != 0)//se existem EPHs
                {
                    user.Ephs = ephs;
                }
                return user;
            }
            catch (Exception ex)
            {
                throw new BusinessException("Erro no Login", "GlobalFunctions.logonEXW", "Erro na função de login: " + ex.Message, ex);
            }
            finally
            {
                if (sp != null)
                    sp.closeConnection();
            }
        }

        /// <summary>
        /// add eph that matches the module and user level
        /// </summary>
        /// <param name="user">user to add eph</param>
        /// <param name="modules">Module to be afected</param>
        /// <param name="values">Values to add the eph</param>
        /// <param name="formId">Eph form id</param>
        /// <returns>Returns information about the filled Initial EPH values to place it to the Cache</returns>
        public static Dictionary<string, InitialEPHCache> FillEphRuntime(ref User user, List<string> modules, string[] values, string formId)
        {
            var result = new Dictionary<string, InitialEPHCache>();
            PersistentSupport sp = PersistentSupport.getPersistentSupport(user.Year, user.Name);

            foreach (string module in modules)
            {
                InitialEPHCache moduleEPHCache = new InitialEPHCache() { Module = module };

                List<Role> roles = user.GetModuleRoles(user.CurrentModule);

                List<string> ephNames = EPH.getEPHName(module, roles, formId);

                foreach (string ephID in ephNames)
                {
                    //Obter a eph correspodente aquele id
                    EPH eph = EPH.getEPH(module);
                    if (eph != null)
                    {
                        foreach (Role role in roles)
                        {
                            EPHCondition[] ephConditions = (EPHCondition[])eph.EphsPerModule[role.ToString()];
                            if (ephConditions == null)
                                continue;
                            foreach (EPHCondition condition in ephConditions)
                            {
                                if (condition.EPHName == ephID)
                                {
                                    string[] setValues = sp.ValuesEphInitial(user.Codpsw, condition, module, values);

                                    if (setValues.Length == 0)
                                        setValues = new string[] { "" };

                                    if (user.Ephs != null && user.Ephs.ContainsKey(module + "_" + condition.EPHName))
                                    {
                                        user.SetEph(user.CurrentModule, ephID, setValues);
                                    }
                                    else
                                    {
                                        if (user.Ephs == null)
                                            user.Ephs = new Hashtable();

                                        user.Ephs.Add(module + "_" + condition.EPHName, setValues);

                                        user.EphTofill.Remove(module, condition);

                                        if (!String.IsNullOrEmpty(condition.IntialForm))
                                        {
                                            string identifier = "ML" + condition.IntialForm;

                                            if (eph.MenusNotSubjectEPH == null)
                                            {
                                                eph.MenusNotSubjectEPH = new Dictionary<string, List<string>>();
                                                eph.MenusNotSubjectEPH.Add(identifier, new List<string>() { condition.EPHName });
                                            }
                                            else if (!eph.MenusNotSubjectEPH.ContainsKey(identifier))
                                            {
                                                eph.MenusNotSubjectEPH.Add(identifier, new List<string>() { condition.EPHName });
                                            }
                                        }
                                    }

                                    // Save the initial PHE value so you can store it in the cache.
                                    if (!moduleEPHCache.EPHValues.ContainsKey(condition.EPHName))
                                    {
                                        moduleEPHCache.EPHValues.Add(condition.EPHName, setValues);
                                    }
                                }
                            }
                        }
                    }
                }

                if(moduleEPHCache.EPHValues.Any())
                    result.Add(module, moduleEPHCache);
            }

            return result;
        }

        /// <summary>
        /// An attempt will be made to recover the Initial EPH.
        /// </summary>
        /// <param name="user">User to revalidate initial eph</param>
        /// <param name="initialEphCache"></param>
        public static void FillEphRuntime(ref User user, Dictionary<string, InitialEPHCache> initialEphCache)
        {
            if (user != null && initialEphCache != null)
            {
                foreach (InitialEPHCache iephCache in initialEphCache.Values)
                {
                    foreach (var iephValue in iephCache.EPHValues)
                    {
                        if(!user.hasEph(iephCache.Module, iephValue.Key))
                            user.Ephs.Add(iephCache.Module + "_" + iephValue.Key, iephValue.Value);
                    }
                }
                user.RevalidateEPHRuntime();
            }
        }

		public static DateTime CalculateExpirationDate()
        {
            DateTime expDate = DateTime.MinValue;
            bool passwordExpActive = Configuration.Security.ExpirationDateBool;
            if (passwordExpActive)
            {
                string passwordDurationStr = Configuration.Security.ExpirationDate;
                if (int.TryParse(passwordDurationStr, out int passwordDuration))
                {
                    expDate = DateTime.Today.AddDays(passwordDuration);
                }
            }

            return expDate;
        }
    }
}