using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlTypes;
using System.DirectoryServices;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Principal;
using System.Security.Cryptography.Pkcs;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Net.Http;
using System.Threading.Tasks;

using CSGenio.framework;
using CSGenio.persistence;
using CSGenio.core.persistence;
using GenioServer.security;
using Quidgest.Persistence;
using Quidgest.Persistence.GenericQuery;

// USE /[MANUAL PRO IMPORTS]/
// USE /[MANUAL PRO IMPORTS GlobalFunctions]/

namespace CSGenio.business
{
    /// <summary>
    /// Summary description for GlobalFunctions.
    /// </summary>
    public sealed partial class GlobalFunctions
    {
        static Hashtable todasFuncoes;
        private User user;
        private string module;
        private PersistentSupport sp;

        /// <summary>
        /// Funções Globais do programa
        /// </summary>
        static GlobalFunctions()
        {
            try
            {
                initTodasFuncoes();
            }
            catch (Exception exc)
            {
                Log.Error($"[GlobalFunctions] Error in static constructor: {exc.Message}");
                throw exc;
            }
        }

        /// <summary>
        /// Constructor da classe
        /// </summary>
        /// <param name="utilizador">User em sessão</param>
        /// <param name="modulo">module que o user está a aceder</param>
        /// <param name="sp">suporte persistente que o user está a utilizar</param>
        public GlobalFunctions(User user, string module, PersistentSupport sp)
        {
            if (Log.IsDebugEnabled)
                Log.Debug(string.Format("Cria instância de GlobalFunctions. [utilizador] {0} [modulo {1}", ( user != null ? user.Name : ""), module));

            this.user = user;
            this.module = module;
            this.sp = sp;
            if (this.sp == null && user != null)
                this.sp = PersistentSupport.getPersistentSupport(user.Year, User.Name);
        }

        /// <summary>
        /// Constructor da classe
        /// </summary>
        /// <param name="utilizador">User em sessão</param>
        /// <param name="modulo">module que o user está a aceder</param>
        public GlobalFunctions(User user, string module) : this(user, module, null)
        { }

        /// <summary>
        /// module
        /// </summary>
        public string Module
        {
            get { return module; }
        }

        /// <summary>
        /// user
        /// </summary>
        public User User
        {
            get { return user; }
        }

        /// <summary>
        /// Função to verificar se a função é válida
        /// </summary>
        /// <param name="funcao">name da função</param>
        /// <returns>true se for válida, false caso contrário</returns>
        public static bool functionValidate(string function)
        {
            return todasFuncoes.ContainsKey(function);
        }

        private void checkFunctionArgs(string[] obj, int minLength = 4)
        {
            if (obj.Length < minLength)
                throw new ArgumentOutOfRangeException("obj", $"Object that represents the function arguments has a length inferior to {minLength}");
        }

        /// <summary>
        /// Executes a named global function
        /// </summary>
        /// <param name="nome">Name of the function</param>
        /// <param name="obj">function arguments</param>
        /// <returns>The result of the executed function</returns>
        public object executeFunction(string name,string[] obj)
        {
            try
            {
                if (obj == null)
                    throw new ArgumentNullException("obj", "The string array that was passed to executeFunction is null");

                if (!todasFuncoes.ContainsKey(name))
                    throw new ArgumentException("The function required is unknown", "name");

                if (Log.IsDebugEnabled) Log.Debug($"Executing global function: {name} args: {String.Join(", ", obj)}");

                switch ((int) todasFuncoes[name])
                {
                    case 0:
                        {
                            checkFunctionArgs(obj, 4);

                            string arg0 = obj[0];
                            string arg1 = obj[1];
                            string arg2 = obj[2];
                            string arg3 = obj[3];
                            return password_alterar(arg0,arg1,arg2,arg3);
                        }
                    case 1:
                        {
                            checkFunctionArgs(obj, 2);

                            string arg0 = obj[0];
                            string arg1 = obj[1];
                            return password_verificaAntiga(arg0,arg1);
                        }
                    case 2:
                        {
                            checkFunctionArgs(obj, 2);

                            string arg0 = obj[0];
                            string arg1 = obj[1];
                            return validateSignature(arg0, arg1);
                        }
                    case 3:
                        {
                            checkFunctionArgs(obj, 2);

                            string arg0 = obj[0];
                            string arg1 = obj[1];
                            return returnFieldsSignature(arg0, arg1);
                        }
                    case 4:
                        {
                            checkFunctionArgs(obj, 3);

                            string arg0 = obj[0];
                            string arg1 = obj[1];
                            string arg2 = obj[2];
                            return writeSignature(arg0, arg1, arg2);
                        }
                    case 5:
                        {
                            checkFunctionArgs(obj, 2);

                            string arg0 = obj[0];
                            string arg1 = obj[1];
                            return password_gerar(arg0, arg1);
                        }
                    case 6:
                        {
                            checkFunctionArgs(obj, 4);

                            string arg0 = obj[0];
                            string arg1 = obj[1];
                            string arg2 = obj[2];
                            string arg3 = obj[3];
                            return CreateDocumQweb(arg0,arg1,arg2,arg3);
                        }
                    case 7:
                        {
                            return GetUserProfile();
                        }
                    default:
                        throw new BusinessException(null, "GlobalFunctions.executaFuncao", "Unknown function name: " + name);
                }
            }
            catch (GenioException ex)
            {
                if (ex.ExceptionSite == "GlobalFunctions.executaFuncao")
                    throw;
                throw new BusinessException(ex.UserMessage, "GlobalFunctions.executaFuncao", "Error executing global function " + name + ": " + ex.Message, ex);
            }
            catch (Exception ex)
            {
                throw new BusinessException(null, "GlobalFunctions.executaFuncao", "Error executing global function " + name + ": " + ex.Message, ex);
            }
        }

        /// <summary>
        /// Regista o Certificado na base de dados se a identificacion e password fizerem match
        /// </summary>
        /// <param name="page"></param>
        /// <param name="identificaco"></param>
        /// <param name="password"></param>
        /// <param name="ano"></param>
        /// <param name="certificado"></param>
        public void regista_certificado(String identificacion, String password, ClientCertificate Qcertificate)
        {
            //Verifica a password (se não for correcta, uma excepção é lançada)
            GenioServer.security.UserPassCredential credential = new GenioServer.security.UserPassCredential();
            credential.Year = Configuration.DefaultYear;
            credential.Username = identificacion;
            credential.Password = password;
            IPrincipal principal = GenioServer.security.SecurityFactory.Authenticate(credential);
            if (principal == null || principal is ErrorPrincipal)
            {
                string error = "Login ou password incorretos.";
                if (principal is ErrorPrincipal)
                    error = (principal as ErrorPrincipal).ErrorMessage;

                throw new BusinessException("Dados de login incorretos.", "GlobalFunctions.regista_certificado", error);
            }

            SelectQuery certificateNotUsedQuery = new SelectQuery()
                .Select(CSGenioApsw.FldCertsn)
                .From(CSGenioApsw.AreaPSW)
                .Where(CriteriaSet.And()
                    .Equal(CSGenioApsw.FldCertsn, Qcertificate.returnSerialNumber()))
                .PageSize(1);

            DataMatrix certificateNotUsed = sp.Execute(certificateNotUsedQuery);

            if (certificateNotUsed.NumRows > 0)
                throw new BusinessException("Dados de login incorretos.", "GlobalFunctions.regista_certificado", "Certificate already used by another user.");

            GenioServer.security.UserFactory.FillUser(principal, User);

            //Numero Serie do Certificado
            registerCertificateSerialNumber(Qcertificate.returnSerialNumber());
        }

        /// <summary>
        /// Regista o Certificado na base de dados (User já autenticado).
        /// </summary>
        /// <param name="identificaco"></param>
        /// <param name="ano"></param>
        /// <param name="certificado"></param>
        public void regista_certificado(String identificacion, ClientCertificate Qcertificate)
        {
            SelectQuery certificateNotUsedQuery = new SelectQuery()
                .Select(CSGenioApsw.FldCertsn)
                .From(CSGenioApsw.AreaPSW)
                .Where(CriteriaSet.And()
                    .Equal(CSGenioApsw.FldCertsn, Qcertificate.returnSerialNumber()))
                .PageSize(1);

            DataMatrix certificateNotUsed = sp.Execute(certificateNotUsedQuery);

            if (certificateNotUsed.NumRows > 0)
                throw new BusinessException("Dados de login incorretos.", "GlobalFunctions.regista_certificado", "Certificate already used by another user.");

            //Numero Serie do Certificado
            registerCertificateSerialNumber(Qcertificate.returnSerialNumber());
        }

        /// <summary>
        /// Método to preencher uma instancia da table psw e validar se o Qcertificate está associado a alguma conta.
        /// </summary>
        /// <param name="psw">instancia da classe psw</param>
        /// <param name="certificado">Certificado Cliente</param>
        /// <returns>classe psw preenchida e validada</returns>
        public CSGenioApsw certificado_preencheValida(CSGenioApsw psw, ClientCertificate Qcertificate)
        {
            try
            {
                string[] modulos = psw.getModules();

                //introduce o name dos modulos
                for (int i = 0; i < modulos.Length; i++)
                    psw.insertNameValueField("psw." + modulos[i].ToLower(), "");
                psw.insertNameValueField("psw.codpsw", "");
                psw.insertNameValueField("psw.password", "");
                psw.insertNameValueField("psw.nome", "");
                psw.insertNameValueField("psw.certsn", "");

                sp.selectOne(CriteriaSet.And().Equal("psw", "certsn", Qcertificate.returnSerialNumber()), null, psw, "");
                return psw;
            }
            catch (GenioException ex)
            {
                throw new BusinessException("Erro na validação das credenciais.", "GlobalFunctions.certificado_preencheValida", "Error validating certificate: " + ex.Message, ex);
            }
        }

        /// <summary>
        /// Método to validar se a password antiga inserida está válida
        /// </summary>
        /// <param name="passAntiga">password antiga</param>
        /// <returns>true se estiver válida, excepção caso contrário</returns>
        public bool password_verificaAntiga(string codpsw, string oldPass)
        {
            try
            {
                AreaInfo pswInfo = CSGenioApsw.GetInformation();
                //string codpsw = User.Codpsw;
                CSGenioApsw area = new CSGenioApsw(user, user.CurrentModule);
                if (sp.getRecord(area, codpsw, new[] { "password", "salt", "pswtype" }) && GenioServer.security.PasswordFactory.IsOK(oldPass, area.ValPassword, area.ValSalt, area.ValPswtype))
                    return true;
                else
                    throw new BusinessException("Erro na verificação da password antiga.", "GlobalFunctions.password_verificaAntiga", "The old password is not correct.");
            }
            catch (GenioException ex)
            {
                if (ex.ExceptionSite == "GlobalFunctions.password_verificaAntiga")
                    throw;
                if (ex.UserMessage == null)
                    throw new BusinessException("Erro na verificação da password antiga.", "GlobalFunctions.password_verificaAntiga", "The old password is not correct.");
                else
                    throw new BusinessException("Erro na verificação da password antiga: " + ex.UserMessage, "GlobalFunctions.password_verificaAntiga", "The old password is not correct.");
            }
            catch (Exception ex)
            {
                throw new BusinessException("Erro na verificação da password antiga.", "GlobalFunctions.password_verificaAntiga", "Error verifying old password: " + ex.Message, ex);
            }
        }

        /// <summary>
        /// Método to change uma password
        /// </summary>
        /// <param name="passAntiga">password antiga</param>
        /// <param name="passNova">nova password</param>
        /// <param name="repeticaoPassNova">repetição da nova password</param>
        /// <returns>true a alteração for bem sucedida, false caso contrário</returns>
        public object password_alterar(string codpsw, string oldPass, string newPass,string newPassRepetition)
        {
            newPass = newPass.TrimEnd();
            newPass = newPass.TrimStart();
            newPassRepetition = newPassRepetition.TrimEnd();
            newPassRepetition = newPassRepetition.TrimStart();
            PersistentSupport sp = PersistentSupport.getPersistentSupport(User.Year, User.Name);

            try
            {
                if (string.IsNullOrEmpty(codpsw))
                    codpsw = User.Codpsw;

                if (User.Codpsw == codpsw || User.IsAdminInAnyModule())
                {
                    var uf = new UserFactory(sp, User);
                    sp.openConnection();
                    var psw = uf.GetUser(User.Name);
                    uf.ChangePassword(psw, newPass, newPassRepetition, oldPass);                        
                    sp.closeConnection();

                    sp.openTransaction();
                    psw.update(sp);
                    sp.closeTransaction();
                }
                return true;
            }
            catch (GenioException ex)
            {
                sp.rollbackTransaction();
                if (ex.ExceptionSite == "GlobalFunctions.password_alterar")
                    throw;
                if (ex.UserMessage == null)
                    throw new BusinessException("Erro na verificação da password antiga.", "GlobalFunctions.password_alterar", "Error verifying old password: " + ex.Message, ex);
                else
                    throw new BusinessException("Erro na verificação da password antiga: " + ex.UserMessage, "GlobalFunctions.password_alterar", "Error verifying old password: " + ex.Message, ex);
            }
            catch (Exception ex)
            {
                // [RC] 06/06/2017 We must rollback in every error situation
                //if (ex is PersistenceException)
                    //sp.rollbackTransaction();
                sp.rollbackTransaction();
                throw new BusinessException("Erro na verificação da password antiga.", "GlobalFunctions.password_alterar", "Error verifying old password: " + ex.Message, ex);
            }
        }

        public object password_gerar(string codpsw, string email)
        {
            PersistentSupport sp = PersistentSupport.getPersistentSupport(User.Year, User.Name);

            try
            {
                if (string.IsNullOrEmpty(Configuration.PP_Name) || string.IsNullOrEmpty(Configuration.PP_Email))
                    throw new BusinessException("Não podem ser geradas passwords sem o smtp e email de envio configurados.", "GlobalFunctions.password_gerar", "Email is not configured.");

                CSGenioApsw psw = new CSGenioApsw(User, module);

                if (string.IsNullOrEmpty(codpsw))
                    codpsw = User.Codpsw;

                if (User.Codpsw == codpsw || User.IsAdminInAnyModule())
                {
                    var newpass = GenioServer.security.PasswordFactory.StringRandom(9, true);
                    sendEmail(Configuration.PP_Name, "password CAV", email, newpass);

                    string passwordEncriptada = GenioServer.security.PasswordFactory.Encrypt(newpass);
                    psw.insertNameValueField("psw.password", passwordEncriptada.Replace("'", "''"));
                    psw.insertNameValueField("psw.codpsw", codpsw);
                    psw.insertNameValueField("psw.salt", "");
                    psw.insertNameValueField("psw.pswtype", Configuration.Security.PasswordAlgorithms.ToString());
                    sp.openTransaction();
                    psw.change(sp, (CriteriaSet)null);
                    sp.closeTransaction();
                }
                return true;
            }
            catch (GenioException ex)
            {
                sp.rollbackTransaction();
                if (ex.ExceptionSite == "GlobalFunctions.password_gerar")
                    throw;
                if (ex.UserMessage == null)
                    throw new BusinessException("Erro ao gerar password.", "GlobalFunctions.password_gerar", "Error generating password: " + ex.Message, ex);
                else
                    throw new BusinessException("Erro ao gerar password: " + ex.UserMessage, "GlobalFunctions.password_gerar", "Error generating password: " + ex.Message, ex);
            }
            catch (Exception ex)
            {
                // [RC] 06/06/2017 We must rollback in every error situation
                //if (ex is PersistenceException)
                    //sp.rollbackTransaction();
                sp.rollbackTransaction();
                throw new BusinessException("Erro ao gerar password.", "GlobalFunctions.password_gerar", "Error generating password: " + ex.Message, ex);
            }
        }

        /// <summary>
        /// Implementação do método iif
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="teste"></param>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        public static T iif<T>(bool test, T v1, T v2)
        {
            return (test ? v1 : v2);
        }

        public static T iif<T>(SqlBoolean test, T v1, T v2)
        {
            return (test ? v1 : v2);
        }

        public static T iif<T>(int test, T v1, T v2)
        {
            return (test == 1 ? v1 : v2);
        }

        /// <summary>
        /// Converte um objecto num inteiro
        /// </summary>
        /// <param name="a">objecto a converter</param>
        /// <returns>retorna o inteiro correspondente ao objecto</returns>
        public static DateTime SumDays(DateTime data, decimal days)
        {
            if (data == DateTime.MinValue)
                return DateTime.MinValue;
            return data.AddDays((double)days);
        }

        /// <summary>
        /// Verifica se um Qyear é bissexto
        /// </summary>
        /// <param name="year">Qyear a verificar</param>
        /// <returns>true caso seja bissexto. Caso contrário false.</returns>
        public static bool IsLeapYear(int year){ return year % 4 == 0 && (year % 100 != 0 || year % 400 == 0); }

        /// <summary>
        /// Coloca a primeira letra da frase/palavra em maiúscula
        /// </summary>
        /// <param name="text">text ou palavra a converter</param>
        /// <returns></returns>
        public static string Capitalize(string text)
        {
            return string.IsNullOrEmpty(text) ? string.Empty : char.ToUpper(text[0]) + text.Substring(1, text.Length-1);
        }

        /// <summary>
        /// Coloca a primeira letra de cada palavra na frase em maiúsculas.
        /// </summary>
        /// <param name="text">text ou palavra a converter</param>
        /// <returns></returns>
        public static string CapitalizeInitials(string text)
        {
            return new System.Globalization.CultureInfo(System.Globalization.CultureInfo.CurrentUICulture.LCID).TextInfo.ToTitleCase(text.ToLower());
        }

        /// <summary>
        /// Converte um objecto num inteiro
        /// </summary>
        /// <param name="a">objecto a converter</param>
        /// <returns>retorna o inteiro correspondente ao objecto</returns>
        public static int atoi(object a)
        {
            if (string.IsNullOrEmpty(a as string)) //DQ 01/09/2006 : se a string é vazia retorna 0;
            {
                return 0;
            }
            return int.Parse(a.ToString());
        }

        /// <summary>
        /// Método que permite converter um inteiro to string
        /// </summary>
        /// <param name="a">parametro que vai ser convertido</param>
        /// <returns>string com o Qvalue convertido</returns>
        public static string IntToString(decimal a)
        {
            return ((int)a).ToString();
        }

        /// <summary>
        /// Método que permite converter um numérico para string
        /// </summary>
        /// <param name="valor">Qvalue que vai ser convertido</param>
        /// <param name="casasDecimais">número de digits decimais</param>
        /// <returns>string com o Qvalue convertido</returns>
        public static string NumericToString(decimal Qvalue, int decimalDigits)
        {
            return Math.Round(Qvalue, decimalDigits).ToString();
        }

        /// <summary>
        /// Método que verifica se uma data está vazia
        /// </summary>
        /// <param name="data">verifica se uma data está vazia</param>
        /// <returns>1 se a data está vazia, 0 o caso contrário</returns>
        public static int emptyD(object data)
        {
            if (data == DBNull.Value || data == null)
                return 1;
            else
                if (data.Equals(DateTime.MinValue)) //SO 20061006 alteração das datas de DateTime to DateTime
                    return 1;
                return 0;
        }

        /// <summary>
        /// Método que verifica se uma key interna está vazia
        /// </summary>
        /// <param name="data">verifica se uma key interna está vazia</param>
        /// <returns>1 se a key interna está vazia, 0 o caso contrário</returns>
        public static int emptyG(object characters)
        {
            if (characters == null || characters.Equals("") || characters.Equals(Guid.Empty.ToString()) || characters.Equals(Guid.Empty.ToString("B")) || characters.Equals("0"))
                return 1;
            else
                return 0;
        }

        /// <summary>
        /// Função que verifica se um objecto está vazio
        /// </summary>
        /// <param name="caracteres">objecto que vai ser testado</param>
        /// <returns>true se está vazia, false caso contrário</returns>
        public static int emptyC(object characters)
        {
            if (characters == null || characters.Equals(""))
                return 1;
            else if (characters.Equals(Guid.Empty.ToString()))
                return 1;
            else
                return 0;
        }

        /// <summary>
        /// Verifica se um numérico está vazio
        /// </summary>
        /// <param name="valor">número a ser comparado</param>
        /// <returns>1 se está vazio, 0 caso contrário</returns>
        public static int emptyN(object Qvalue)
        {
            if (Qvalue == null || Qvalue.Equals(0m) || Qvalue.Equals(0d) || Qvalue.Equals(0))
                return 1;
            else
                return 0;
        }

        /// <summary>
        /// verifica se uma hour está vazia
        /// </summary>
        /// <param name="caracteres">hour a ser comparada</param>
        /// <returns>true se está vazia, false caso contrário</returns>
        public static int emptyT(object characters)
        {
            if (characters == null || characters.Equals("__:__") || characters.Equals(""))
                return 1;
            else
                return 0;
        }

        /// <summary>
        /// verifica se um int está vazio
        /// </summary>
        /// <param name="valor">número a ser comparado</param>
        /// <returns>true se está vazio, false caso contrário</returns>
        public static int emptyL(object Qvalue)
        {
            string type = Qvalue?.GetType().Name;
            if (Qvalue == null || type == null || Qvalue.Equals(0) || Qvalue.Equals(0.0) || (type.Contains("Bool") && !((bool)Qvalue)) || (type.Contains("Logical") && !((Logical)Qvalue)))
                return 1;
            else
                return 0;
        }

        /// <summary>
        /// Função que permite formatar uma data
        /// </summary>
        /// <param name="valor">Qvalue da data</param>
        /// <param name="format">formatação</param>
        /// <returns>A data formatada</returns>
        public static string FormatDate(DateTime Qvalue, string format)
        {
            //TODO: o metodo anterior tambem nao usava a formatação mas convém compatibilizar com o backoffice
            return Qvalue.ToString("{dd/mm/yyyy}");
        }

        /// <summary>
        /// Função que tira os espaços à esquerda de uma função
        /// </summary>
        /// <param name="valor">Qvalue que queremos tirar os espaços</param>
        /// <returns>a string sem os espaços à esquerda</returns>
        public static string LTRIM(string Qvalue)
        {
            return Qvalue.TrimStart();
        }

        /// <summary>
        /// Função que tira os espaços à direita de uma função
        /// </summary>
        /// <param name="valor">Qvalue que queremos tirar os espaços</param>
        /// <returns>a string sem os espaços à direita</returns>
        public static string RTRIM(string Qvalue)
        {
            return Qvalue.TrimEnd();
        }

        /// <summary>
        /// Função que permite obter o Qyear de uma data
        /// </summary>
        /// <param name="valor">Qvalue com a data</param>
        /// <returns>o Qyear da data</returns>
        public static int Year(DateTime Qvalue)
        {
            //SO 20061006 alteração das datas de DateTime to DateTime
            if (Qvalue==null || Qvalue==DateTime.MinValue)
                return 0;
            return Qvalue.Year;
        }

        /// <summary>
        /// Função que permite obter o mês de uma data
        /// </summary>
        /// <param name="valor">Qvalue com a data</param>
        /// <returns>o mês da data</returns>
        public static int Month(DateTime Qvalue)
        {
            //SO 20061006 alteração das datas de DateTime to DateTime
            if (Qvalue == null || Qvalue == DateTime.MinValue)
                return 0;
            return Qvalue.Month;
        }

        /// <summary>
        /// Função que permite obter o day de uma data
        /// </summary>
        /// <param name="valor">Qvalue com a data</param>
        /// <returns>o day da data</returns>
        public static int Day(DateTime Qvalue)
        {
            //SO 20061006 alteração das datas de DateTime to DateTime
            if (Qvalue == null || Qvalue == DateTime.MinValue)
                return 0;
            return Qvalue.Day;
        }

        /// <summary>
        /// Função que retorna string referente ao Qyear de uma data
        /// </summary>
        /// <param name="valor">Qvalue com a data</param>
        /// <returns>o Qyear da data</returns>
        public static string strYear(DateTime Qvalue)
        {
            //SO 20061006 alteração das datas de DateTime to DateTime
            if (Qvalue==null || Qvalue==DateTime.MinValue)
                return IntToString(0);
            return IntToString(Qvalue.Year);
        }

        /// <summary>
        /// Função que devolve a data de hoje
        /// </summary>
        /// <returns>DateTime com a data de hoje</returns>
        ///SO 20061006 alteração das datas de DateTime to DateTime
        public static DateTime Today()
        {
            return DateTime.Today;
        }

        /// <summary>
        /// Method that return date and time
        /// </summary>
        /// <returns>Date and time in DateTime format</returns>
        public static DateTime Now()
        {
            return DateTime.Now;
        }

        /// <summary>
        // Converte um Qfield com o format de horas do Genio em um real com o
        // número de horas decorridas desde 00:00 com os minutes nas digits decimais
        /// </summary>
        /// <param name="time">A hour em format __:__</param>
        /// <returns>O número de horas decorridos desde 00:00</returns>
        public static decimal HoursToDouble(string time)
        {
            return HourFunctions.HoursToDouble(time);
        }

        /// <summary>
        ///  Adiciona minutes a um fields no format de horas do Genio
        /// Não decresce de 00:00 ou incrementa de 23:59
        /// </summary>
        /// <param name="time">A hour em format __:__</param>
        /// <param name="minutos">O number de minutes a adicionar</param>
        /// <returns>A nova hour com os minutes adicionados</returns>
        public static string HoursAdd(string time, decimal minutes)
        {
            return HourFunctions.HoursAdd(time, minutes);
        }

        /// <summary>
        /// Transforma uma key (guid ou interna) numa string
        /// </summary>
        /// <param name="chave">O Qvalue da key</param>
        /// <returns>Uma string com representado a key interna</returns>
        public static string KeyToString(string key)
        {
            if (emptyG(key) == 1)
                return "";

            string res = key;
            res = res.Replace("{", "");
            res = res.Replace("}", "");
            res = res.Replace("-", "");
            return res.ToUpper();
        }

        public static string StringToKey(string str)
        {
            string res = str;
            if (res.Length == 32)
            {
                res = res.Insert(8, "-");
                res = res.Insert(13, "-");
                res = res.Insert(18, "-");
                res = res.Insert(23, "-");
            }

            if (res.Length == 36)
                res = "{"+res+"}";

            return res;
        }

        /// <summary>
        // Converte um Qfield real com o número de horas decorridas desde 00:00 em um
        // Qfield no format de horas do Genio
        /// </summary>
        /// <param name="time">O número de horas decorridos desde 00:00</param>
        /// <returns>A hour em format __:__</returns>
        public static string DoubleToHours(decimal time)
        {
            int minutosTotais = (int)Math.Round(time * 60);
            int horasParte = minutosTotais / 60;
            int minutosParte = minutosTotais % 60;
            return horasParte.ToString("D2") + ':' + minutosParte.ToString("D2");
        }

        /// <summary>
        /// Create a date from its parts.
        /// </summary>
        /// <param name="year">year</param>
        /// <param name="month">month</param>
        /// <param name="day">day</param>
        /// <param name="hour">hour</param>
        /// <param name="minute">minute</param>
        /// <param name="second">second</param>
        /// <returns>A DateTime with the specified parameters</returns>
        public static DateTime CreateDateTime(decimal year, decimal month, decimal day, decimal hour, decimal minute, decimal second)
        {
            try
            {
                return new DateTime((int)year, (int)month, (int)day, (int)hour, (int)minute, (int)second);
            }
            catch (ArgumentOutOfRangeException)
            {
                //É um pouco discutível se isto devia falhar silenciosamente, se alguém passa parâmetros inválidos
                // to esta função devia ter cuidado com isso e ser avisado o mais cedo possível em desenvolvimento.
                return DateTime.MinValue;
            }
        }

        /// <summary>
        /// Create a date from its parts.
        /// </summary>
        /// <param name="year">year</param>
        /// <param name="month">month</param>
        /// <param name="day">day</param>
        /// <returns>A DateTime with the specified parameters</returns>
        public static DateTime CreateDateTime(decimal year, decimal month, decimal day)
        {
            return CreateDateTime(year, month, day, 0, 0, 0);
        }

        /// <summary>
        /// Creates a date with the specified time included.
        /// </summary>
        /// <param name="date">A date</param>
        /// <param name="time">A time with format __:__</param>
        /// <returns>A DateTime with the specified parameters</returns>
        [Obsolete("Use DateSetTime instead")]
        public static DateTime CriaDataHora(DateTime date, string time)
        {
            if (emptyD(date)==1) return DateTime.MinValue;
            int h0 = 0, m0 = 0;
            if (time.Length == 5)
            {
                //corrigir a string
                time = time.Replace('_', '0');
                Int32.TryParse(time.Substring(0, 2), out h0);
                Int32.TryParse(time.Substring(3, 2), out m0);
                if (h0 < 0 || h0 > 23 || m0 < 0 || m0 > 59)
                {
                    h0 = 0;
                    m0 = 0;
                }
            }
            return CreateDateTime(date.Year, date.Month, date.Day, h0, m0, 0);
        }

        /// <summary>
        /// Set a specific time on a date.
        /// </summary>
        /// <param name="date">A date</param>
        /// <param name="time">A time with format __:__</param>
        /// <returns>A DateTime with the specified parameters</returns>
        public static DateTime DateSetTime(DateTime date, string time)
        {
            if (date == DateTime.MinValue)
                return date;
            const decimal epsilon = 0.1m;
            decimal full = HourFunctions.HoursToDouble(time);
            int hh = (int)full;            
            int mm = (int)(epsilon + (full - hh) * 60m);
            return CreateDateTime(date.Year, date.Month, date.Day, hh, mm, 0);
        }

        /// <summary>
        /// Compare two dates and return an integer that indicates their chronology.
        /// Whether the first instance is earlier than, the same as, or later than the second instance.
        /// </summary>
        /// <param name="date1">date1</param>
        /// <param name="date2">date2</param>
        /// <returns>0 if equal, <0 date1 is earlier than date2, >0 date1 is later than date2</returns>
        public static int DateCompare(DateTime date1, DateTime date2)
        {
            return DateTime.Compare(date1, date2);
        }

        /// <summary>
        /// Create a duration/timespan from its parts.
        /// </summary>
        /// <param name="days">Number of days</param>
        /// <param name="hours">Number of hours</param>
        /// <param name="minutes">Number of minutes</param>
        /// <param name="seconds">Number of seconds</param>
        /// <returns>A TimeSpan with the specified parameters</returns>
        public static TimeSpan CreateDuration(int days, int hours, int minutes, int seconds)
        {
            // TODO: add try-catch to avoid runtime exceptions
            return new TimeSpan(days, hours, minutes, seconds);
        }

        /// <summary>
        /// Compare two dates and return the difference.
        /// </summary>
        /// <param name="startDate">startDate</param>
        /// <param name="endDate">endDate</param>
        /// <returns>Duration between startDate and endDate</returns>
        public static TimeSpan DateDiff(DateTime startDate, DateTime endDate)
        {
            return endDate.Subtract(startDate);
        }

        /// <summary>
        /// Compare two dates and return the difference in a specific unit.
        /// </summary>
        /// <param name="startDate">startDate</param>
        /// <param name="endDate">endDate</param>
        /// <param name="unit">unit</param>
        /// <returns>Duration between startDate and endDate</returns>
        public static decimal DateDiffPart(DateTime startDate, DateTime endDate, string unit)
        {
            TimeSpan diff = endDate.Subtract(startDate);
            if (unit == "D")
                return (decimal)Math.Floor(diff.TotalDays);
            if (unit == "H")
                return (decimal)Math.Floor(diff.TotalHours);
            if (unit == "M")
                return (decimal)Math.Floor(diff.TotalMinutes);
            if (unit == "S")
                return (decimal)Math.Floor(diff.TotalSeconds);

            return 0;
        }

        /// <summary>
        /// Sum a duration to a date.
        /// </summary>
        /// <param name="date">Date to increment</param>
        /// <param name="duration">A TimeSpan representing a duration</param>
        /// <returns>A DateTime with the specified duration added</returns>
        public static DateTime DateAddDuration(DateTime date, TimeSpan duration)
        {
            return date + duration;
        }

        /// <summary>
        /// Subtract a duration from a date.
        /// </summary>
        /// <param name="date">Date to reduce</param>
        /// <param name="duration">A TimeSpan representing a duration</param>
        /// <returns>A DateTime with the specified duration subtracted</returns>
        public static DateTime DateSubtractDuration(DateTime date, TimeSpan duration)
        {
            return date - duration;
        }

        /// <summary>
        /// Add a duration to a date.
        /// </summary>
        /// <param name="date">Date to change</param>
        /// <param name="years">Number of years, each year equals 365 days</param>
        /// <returns>A DateTime with the specified duration added/subtracted</returns>
        public static DateTime DateAddYears(DateTime date, decimal years)
        {
            return date.AddYears((int)years);
        }

        /// <summary>
        /// Add a duration to a date.
        /// </summary>
        /// <param name="date">Date to change</param>
        /// <param name="months">Number of months, each month equals 30 days</param>
        /// <returns>A DateTime with the specified duration added/subtracted</returns>
        public static DateTime DateAddMonths(DateTime date, decimal months)
        {
            return date.AddMonths((int)months);
        }

        /// <summary>
        /// Add a duration to a date.
        /// </summary>
        /// <param name="date">Date to change</param>
        /// <param name="days">Number of days</param>
        /// <returns>A DateTime with the specified duration added/subtracted</returns>
        public static DateTime DateAddDays(DateTime date, decimal days)
        {
            return date.AddDays((double)days);
        }

        /// <summary>
        /// Add a duration to a date.
        /// </summary>
        /// <param name="date">Date to change</param>
        /// <param name="hours">Number of hours</param>
        /// <returns>A DateTime with the specified duration added/subtracted</returns>
        public static DateTime DateAddHours(DateTime date, decimal hours)
        {
            return date.AddHours((double)hours);
        }

        /// <summary>
        /// Add a duration to a date.
        /// </summary>
        /// <param name="date">Date to change</param>
        /// <param name="minutes">Number of minutes</param>
        /// <returns>A DateTime with the specified duration added/subtracted</returns>
        public static DateTime DateAddMinutes(DateTime date, decimal minutes)
        {
            return date.AddMinutes((double)minutes);
        }

        /// <summary>
        /// Add a duration to a date.
        /// </summary>
        /// <param name="date">Date to change</param>
        /// <param name="seconds">Number of seconds</param>
        /// <returns>A DateTime with the specified duration added/subtracted</returns>
        public static DateTime DateAddSeconds(DateTime date, decimal seconds)
        {
            return date.AddSeconds((double)seconds);
        }

        /// <summary>
        /// Get the year of the date.
        /// </summary>
        /// <param name="date">Date to read</param>
        /// <returns>Year</returns>
        public static int DateGetYear(DateTime date)
        {
            return date.Year;
        }

        /// <summary>
        /// Get the month of year from the date.
        /// </summary>
        /// <param name="date">Date to read</param>
        /// <returns>Month of year</returns>
        public static int DateGetMonth(DateTime date)
        {
            return date.Month;
        }

        /// <summary>
        /// Get the day of month from the date.
        /// </summary>
        /// <param name="date">Date to read</param>
        /// <returns>Day of month</returns>
        public static int DateGetDay(DateTime date)
        {
            return date.Day;
        }

        /// <summary>
        /// Get the hour in day from the date.
        /// </summary>
        /// <param name="date">Date to read</param>
        /// <returns>Hour in day</returns>
        public static int DateGetHour(DateTime date)
        {
            return date.Hour;
        }

        /// <summary>
        /// Get the minute in hour from the date.
        /// </summary>
        /// <param name="date">Date to read</param>
        /// <returns>Minute in hour</returns>
        public static int DateGetMinute(DateTime date)
        {
            return date.Minute;
        }

        /// <summary>
        /// Get the second in minute from the date.
        /// </summary>
        /// <param name="date">Date to read</param>
        /// <returns>Second in minute</returns>
        public static int DateGetSecond(DateTime date)
        {
            return date.Second;
        }

        /// <summary>
        /// Get the total days in the duration.
        /// </summary>
        /// <param name="duration">Duration to read</param>
        /// <returns>duration in days</returns>
        public static decimal DurationTotalDays(TimeSpan duration)
        {
            return (decimal)duration.TotalDays;
        }

        /// <summary>
        /// Get the total hours in the duration.
        /// </summary>
        /// <param name="duration">Duration to read</param>
        /// <returns>duration in hours</returns>
        public static decimal DurationTotalHours(TimeSpan duration)
        {
            return (decimal)duration.TotalHours;
        }

        /// <summary>
        /// Get the total minutes in the duration.
        /// </summary>
        /// <param name="duration">Duration to read</param>
        /// <returns>duration in minutes</returns>
        public static decimal DurationTotalMinutes(TimeSpan duration)
        {
            return (decimal)duration.TotalMinutes;
        }

        /// <summary>
        /// Get the total seconds in the duration.
        /// </summary>
        /// <param name="duration">Duration to read</param>
        /// <returns>duration in seconds</returns>
        public static decimal DurationTotalSeconds(TimeSpan duration)
        {
            return (decimal)duration.TotalSeconds;
        }

        /*****/

        /// <summary>
        /// Truncates the time part of a datetime value.
        /// </summary>
        /// <param name="date">The source date</param>
        /// <returns>A modified date with only the date part of the original datetime</returns>
        public static DateTime DateFloorDay(DateTime date)
        {
            return new DateTime(date.Year, date.Month, date.Day, 0, 0, 0, date.Kind);
        }

        /// <summary>
        /// Função que dados dois numéricos devolve o máximo
        /// </summary>
        /// <param name="obj1">numérico a comparar</param>
        /// <param name="obj2">outro numérico a comparar</param>
        /// <returns>o máximo entre os dois</returns>
        public static decimal maxN(decimal obj1, decimal obj2)
        {
            return (obj1 > obj2 ? obj1 : obj2);
        }

        /// <summary>
        /// Função que dados dois numéricos devolve o mínimo
        /// </summary>
        /// <param name="obj1">numérico a comparar</param>
        /// <param name="obj2">outro numérico a comparar</param>
        /// <returns>o mínimo entre os dois</returns>
        public static decimal minN(decimal obj1, decimal obj2)
        {
            return (obj1 < obj2 ? obj1 : obj2);
        }

        /// <summary>
        /// Função que dadas duas datas devolve a data máxima
        /// </summary>
        /// <param name="obj1">data a comparar</param>
        /// <param name="obj2">outra data a comparar</param>
        /// <returns>a maior data</returns>
        public static DateTime maxD(DateTime obj1, DateTime obj2)
        {
            return (obj1 > obj2 ? obj1 : obj2);
        }

        /// <summary>
        /// Função que dadas duas datas devolve a data mínima
        /// </summary>
        /// <param name="obj1">data a comparar</param>
        /// <param name="obj2">outra data a comparar</param>
        /// <returns>a menor data</returns>
        public static DateTime minD(DateTime obj1, DateTime obj2)
        {
            return (obj1 < obj2 ? obj1 : obj2);
        }

        /// <summary>
        /// Função que obtem o day actual
        /// </summary>
        /// <returns>DateTime com o day actual</returns>
        public static DateTime GetCurrentDay()
        {
            return DateTime.Today;
        }

        /// <summary>
        /// Função que obtem o mês actual
        /// </summary>
        /// <returns>int com o mês actual</returns>
        public static int GetCurrentMonth()
        {
            return Month(DateTime.Today);
        }

        /// <summary>
        /// Função que obtem o Qyear actual
        /// </summary>
        /// <returns>int com o Qyear actual</returns>
        public static int GetCurrentYear()
        {
            return Year(DateTime.Today);
        }

        /// <summary>
        /// Função que permite obter o nº de characters desejado à esquerda de uma string
        /// </summary>
        /// <param name="arg">string</param>
        /// <param name="nrElem">nº de characters</param>
        /// <returns>nº de characters da string a count da esquerda</returns>
        public static string LEFT(string arg,int nrElem)
        {
            if (arg == null)
                return "";
            if (nrElem < 0)
                return "";
            if (nrElem > arg.Length)
                return arg;

            return arg.Substring(0,nrElem);
        }

        /// <summary>
        /// Função que permite obter o nº de characters desejado à direita de uma string
        /// </summary>
        /// <param name="arg">string</param>
        /// <param name="nrElem">nº de characters</param>
        /// <returns>nº de characters da string a count da direita</returns>
        public static string RIGHT(string arg,int nrElem)
        {
            if (arg == null)
                return "";
            if (nrElem < 0)
                return "";
            if (nrElem > arg.Length)
                return arg;
            return arg.Substring(arg.Length-nrElem,nrElem);
        }

        /// <summary>
        /// Função que dada uma string permite obter o nº de elementos a count de uma posição
        /// </summary>
        /// <param name="arg">string</param>
        /// <param name="inicio">posição apartir da qual se querem obter os characters</param>
        /// <param name="nrElem">nº de characters desejados</param>
        /// <returns>characters da string</returns>
        public static string SubString(string arg,int start,int nrElem)
        {
            if (arg == null)
                return "";
            if (nrElem < 0)
                return "";
            if (start < 0)
                return "";
            if (start > arg.Length)
                return "";
            if (nrElem > arg.Length - start)
                nrElem = arg.Length - start;
            return arg.Substring(start,nrElem);
        }

        /// <summary>
        /// Returns the zero-based index of the first occurrence of the specified substring within the given string.
        /// If the substring is not found or either input string is null or empty, returns -1.
        /// </summary>
        /// <param name="str">The string to search in.</param>
        /// <param name="substr">The substring to search for.</param>
        /// <returns>The zero-based index of the first occurrence of the specified substring, or -1 if not found.</returns>
        public static int IndexOf(string str, string substr)
        {
            if (string.IsNullOrEmpty(str) || string.IsNullOrEmpty(substr))
                return -1;
            return str.IndexOf(substr);
        }

        /// <summary>
        /// Função que permite arredondar um numérico com o número de digits decimais definido
        /// </summary>
        /// <param name="num">número a ser arredondamento</param>
        /// <param name="casas">número de digits decimais</param>
        /// <returns>o número arredondado</returns>
        public static decimal Round(decimal num, int digits)
        {
            //HAP - Added casts due to diferences when field in Genio is from decimal type.
			//Discussed with Rodrigo Serafim and Joao Ferro (2024/02/28) this solutions and it works with decimal and double/float
			return System.Math.Round(num, digits, MidpointRounding.AwayFromZero);
        }

        /// <summary>
        /// Função que permite obter o módulo de um número
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public static decimal abs(decimal num)
        {
            return System.Math.Abs(num);
        }

        /// <summary>
        /// Validate if a date is valid and between an acceptable range
        /// </summary>
        /// <param name="date">date</param>
        /// <returns>1 if its valid, 0 otherwise</returns>
        [Obsolete("Use !emptyD(Datetime date) instead")]
        public static int IsValid(DateTime date)
        {
            // SqlDateTime must be between 1/1/1753 12:00:00 AM and 12/31/9999 11:59:59 PM
            if (date.Equals(DateTime.MinValue) || date < new DateTime(1753, 1, 1))
                return 0;
            else
                return 1;
        }

        /// <summary>
        /// Função que compara duas datas
        /// </summary>
        /// <param name="data1">date1</param>
        /// <param name="data2">date2</param>
        /// <returns>0 se sao iguais >0 se a 1ª é maior e <0 se a 1ª é menor </returns>
        public static int CompareDates(DateTime date1,DateTime date2)
        {
            return DateTime.Compare(date1,date2);
        }

        /// <summary>
        /// Função que retorna o size de uma string
        /// </summary>
        /// <param name="a">string</param>
        /// <returns>inteiro que corresponde ao size de uma string</returns>
        public static int LengthString(string a)
        {
            return a.Length;
        }

        /// <summary>
        /// Funcao que calcula o Qvalue da Incidencia
        /// </summary>
        /// <param name="valoruni">Qvalue unitário</param>
        /// <param name="quantida">quantidade</param>
        /// <param name="pdescont">percentagem de desconto</param>
        /// <param name="prec">digits decimais de precisão</param>
        /// <returns>o Qvalue da incidencia</returns>
        public static decimal Incidenc(decimal unitValue, decimal amount, decimal pdiscount, int prec)
        {
            decimal valorart = RoundQG(unitValue * amount, prec);
            return valorart - RoundQG(pdiscount / 100.0m * valorart, prec);
        }

        /// <summary>
        /// Função que calcula o Qvalue do IVA
        /// A incidencia pode entrar com iva ou sem iva sendo discriminada pelo parametro vatprice
        /// </summary>
        /// <param name="incidenc">O Qvalue com iva ou sem iva</param>
        /// <param name="taxa_iva">taxa de IVA</param>
        /// <param name="preciva">1 caso o incidenc seja o preço com iva, 0 caso seja o preço sem iva</param>
        /// <param name="prec">precisão</param>
        /// <returns>Qvalue do IVA</returns>
        public static decimal VATValue(decimal incidenc, decimal rate_iva, int vatprice, int prec)
        {
            return RoundQG(vatprice==1
                ? incidenc / (1.0m + rate_iva / 100.0m) * (rate_iva / 100.0m)
                : incidenc * (rate_iva / 100.0m), prec);
        }

        /// <summary>
        /// Função que faz o arredondamento
        /// O arredondamento é feito com uma folga de 0.001 o que significa que por exemplo:
        /// RoundQG(0.499, 0) = 1.0 sendo que apenas 0.49899999 é que arredonda to baixo
        /// </summary>
        /// <param name="x">number a arrendondar</param>
        /// <param name="c">number de digits</param>
        /// <returns>Qvalue arredondado</returns>
        public static decimal RoundQG(decimal x, int c)
        {
            //(RS 2010.11.03) Reimplementei to dar os mesmos resultados que no BO e no SQL
            if (c < 0) c = 0;
            decimal folga = (decimal)(0.001 * Math.Pow(0.1, c) * Math.Sign(x));
            return Math.Round(x + folga, c, MidpointRounding.AwayFromZero);
        }

        /// <summary>
        /// Função que devolve um bool se a string corresponder à expressão regular
        /// </summary>
        /// <param name="expression">Expressão a ser avaliada</param>
        /// <param name="pattern">Padrão em expressão regular</param>
        /// <returns>true se a expressão é validate, false caso contrário</returns>
        public static bool RegExpr(string expression, string pattern)
        {
            Regex re = new Regex(pattern);
            if (re.IsMatch(expression))
                return (true);
            else
                return (false);
        }

        /// <summary>
        /// Função que devolve um bool se a string corresponder ao wildcard
        /// </summary>
        /// <param name="expression">Expressão a ser avaliada</param>
        /// <param name="pattern">Padrão wildcard</param>
        /// <returns>true se a expressão é validate, false caso contrário</returns>
        public static bool RegExprWild(string expression, string pattern)
        {
            pattern = WildcardToRegExpr(pattern);
            return RegExpr(expression, pattern);
        }

        /// <summary>
        /// Função que converte wildcards to Regex
        /// </summary>
        /// <param name="pattern">Padrão wildcard</param>
        /// <returns>Padrão em expressão regular</returns>
        public static string WildcardToRegExpr(string pattern)
        {
            return "^" + Regex.Escape(pattern).
              Replace("\\*", ".*").
              Replace("\\?", ".") + "$";
        }

        /// <summary>
        /// Função to enviar um e-mail
        /// </summary>
        /// <param name="smtp">cliente smtp</param>
        /// <param name="de">endereço de source</param>
        /// <param name="para">endereço de target</param>
        /// <param name="assunto">subject</param>
        /// <param name="corpo">body do email</param>
        public void sendEmail(string smtp, string de, string to, string subject, string body,User user)
        {
            SmtpClient smtpCliente = new SmtpClient();
            System.Net.Mail.MailMessage mensagem = new System.Net.Mail.MailMessage();
            try
            {
                //host smtp
                smtpCliente.Host = smtp;

                //endereço source
                mensagem.From = new MailAddress(de);

                // endereço target
                mensagem.To.Add(to);
                mensagem.Subject = subject;

                //body da mensagem
                mensagem.Body = body ;

                // Send SMTP mail
                smtpCliente.Send(mensagem);

            }
            catch (Exception ex)
            {
                Log.Error("Erro a enviar email: " + ex.Message);
            }
        }

        /// <summary>
        /// Implementação do método floor
        /// </summary>
        /// <param name="numero"></param>
        /// <returns>floor de número</returns>
        public static decimal Floor(decimal number)
        {
            return Math.Floor(number);
        }

        /// <summary>
        /// Método que permite enviar um email
        /// </summary>
        /// <param name="origem">email de quem envia o email</param>
        /// <param name="assunto">subject do email</param>
        /// <param name="destino">email de quem recebe o email</param>
        /// <param name="corpoEmail">body do email</param>
        public static void sendEmail(string source,string subject,string target,string emailBody)
        {
            try
            {
                SmtpClient smtpClient = new SmtpClient(Configuration.PP_Email);
                System.Net.Mail.MailMessage message = new System.Net.Mail.MailMessage(source, target);
                message.Subject = subject;
                message.Body = emailBody;
                smtpClient.Send(message);
            }
            catch (Exception ex)
            {
                throw new BusinessException("Não foi possível enviar o email.", "GlobalFunctions.enviarEmail", "Couldn't send the email: " + ex.Message, ex);
            }
        }

        /// <summary>
        /// FS >2008-03-26
        /// Permite calcular a diferença entre duas datas (DateTime).
        ///Retorna um Qvalue (numerico) da diferença entre as duas datas.
        /// </summary>
        /// <param name="dt_inicio">data de início</param>
        /// <param name="dt_fim">data de fim</param>
        /// <param name="escala">em D(Dias), H(Horas), M(Minutos) ou S(Segundos)</param>
        /// <returns>a diferença entre as datas na scale escolhida</returns>
        public static decimal Diferenca_entre_Datas(DateTime dt_start, DateTime dt_end, string scale)
        {
            // RS (2010.11.04) Acertei a função to ficar com a mesma semantica que no backoffice
            decimal diferenca_valor;

            if (emptyD(dt_start) == 1 || emptyD(dt_end) == 1)
                return 0;

            TimeSpan diferenca_tempo = dt_end - dt_start;
            switch (scale.ToString().ToUpper())
            {
                case "D":
                    {
                        diferenca_valor = (decimal)Math.Floor(diferenca_tempo.TotalDays);
                        break;
                    }
                case "H":
                    {
                        diferenca_valor = (decimal)Math.Floor(diferenca_tempo.TotalHours);
                        break;
                    }
                case "M":
                    {
                        diferenca_valor = (decimal)Math.Floor(diferenca_tempo.TotalMinutes);
                        break;
                    }
                case "S":
                    {
                        diferenca_valor = (decimal)Math.Floor(diferenca_tempo.TotalSeconds);
                        break;
                    }
                default:
                    {
                        diferenca_valor = 0;
                        break;
                    }
            }
            return diferenca_valor;
        }

        /// <summary>
        /// Created by [SF] at [2017.03.23]
        /// Query to retirar todos os dados necessÃ¡rios to enviar pelo querystring
        /// </summary>
        /// <param name="id">parametro do id da table associada ao docums</param>
        /// <returns>Dados do documento</returns>
        public DataMatrix ReturnValueSignPdf(string id, string table, string Qfield, string keyPK)
        {
            DataMatrix result = null;
            try
            {
                SelectQuery query = new SelectQuery()
                    .Select(CSGenioAdocums.FldNome)
                    .Select(CSGenioAdocums.FldDocument)
                    .Select(CSGenioAdocums.FldDocpath)
                    .Select(CSGenioAdocums.FldCoddocums)
                    .Select(CSGenioAdocums.FldVersao)
                    .From(table)
                    .Join("docums", "DOCUMS")
                    .On(CriteriaSet.And().Equal(table, Qfield+"fk", "DOCUMS", "documid"))
                    .Where(CriteriaSet.And().Equal(table, keyPK, id)
                    .Equal(table, "zzstate", 0)
                    .NotEqual("DOCUMS", "VERSAO", "CHECKOUT"))
                    .OrderBy(CSGenioAdocums.FldDatacria, Quidgest.Persistence.GenericQuery.SortOrder.Descending).Page(1);

                result = sp.Execute(query);
                return result;
            }
            catch
            {
                return result;
            }
        }

        /// <summary>
        /// Created by [SF] at [2017.03.16]
        /// Função to comprimir uma string
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string CompressString(string text)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(text);
            var memoryStream = new MemoryStream();
            using (var gZipStream = new System.IO.Compression.GZipStream(memoryStream, System.IO.Compression.CompressionMode.Compress, true))
            {
                gZipStream.Write(buffer, 0, buffer.Length);
            }

            memoryStream.Position = 0;

            var compressedData = new byte[memoryStream.Length];
            memoryStream.Read(compressedData, 0, compressedData.Length);

            var gZipBuffer = new byte[compressedData.Length + 4];
            Buffer.BlockCopy(compressedData, 0, gZipBuffer, 4, compressedData.Length);
            Buffer.BlockCopy(BitConverter.GetBytes(buffer.Length), 0, gZipBuffer, 0, 4);
            return Convert.ToBase64String(gZipBuffer);
        }

        /// <summary>
        /// Devolve a sigla iso da moeda por omissão definida
        /// </summary>
        /// <returns>A sigla iso da moeda por omissão definida</returns>
        public string GetDefaultCurrency()
        {
            return "EUR";
        }

        /// <summary>
        /// Verifica se uma assinatura é válida
        /// </summary>
        /// <param name="nomePrograma">Name do programa (prefixo das tables)</param>
        /// <param name="nomeTabela">O name da table</param>
        /// <param name="nomeCodigoInterno">O name do Qfield do codigo interno</param>
        /// <param name="codigoInterno">O Código Interno utilizado</param>
        /// <returns>Devolve um array com o codigo do hash usado no momento e o Qvalue dos fields to assinar</returns>
        public string validateSignature(string QtableName, string internalCode)
        {
            String nomeTabelaBD = Configuration.Program + QtableName.ToUpper();
            String nomeChavePrimaria = Area.GetInfoArea(QtableName).PrimaryKeyName;

            String HashRegis = "";
            String query = @"select campos as camposAssinatura, " + nomeTabelaBD + @".*
                from " + nomeTabelaBD + " inner join " + Configuration.Program + "hashcd on " + nomeTabelaBD + @".codhashcd = " + Configuration.Program + @"hashcd.codhashcd
                where " + nomeTabelaBD + "." + nomeChavePrimaria + " = '" + internalCode + "'";

            //Vai buscar a linha
            DataMatrix dataSet = sp.executeQuery(query);
            //Se não houver dados o Qfield não foi assinado...
            if (dataSet.NumRows == 0)
                throw new BusinessException("Erro na validação da assinatura.", "GlobalFunctions.validarAssinatura", "The record with code " + internalCode + " wasn't found.");

            //Verifica se existem fields to assinar
            string[] camposArray = dataSet.GetString(0,"camposAssinatura").Split(',');
            if (!(camposArray.Length > 0))
                throw new BusinessException("Não existem campos para assinar.", "GlobalFunctions.validarAssinatura", "The record with code " + internalCode + " has no fields to sign.");
            else
            {
                Dictionary<string, framework.Field> fields = Area.GetInfoArea(QtableName).DBFields;
                //To todos os fields definidos no array
                foreach (string fieldName in camposArray)
                {
                    framework.Field Qfield;
                    fields.TryGetValue(fieldName, out Qfield);
                    //Se for uma data é preciso ter em atençao a formatting
                    if (Qfield.FieldType.Equals(CSGenio.framework.FieldType.DATA))
                    {
                        String l = dataSet.GetDirect(0,fieldName).ToString();
                        if (l.Equals(""))
                            HashRegis += "__/__/____";
                        else
                            HashRegis += l.Replace("-", "/").Substring(0,10);
                    }
                    else
                    {
                        HashRegis += dataSet.GetDirect(0, fieldName).ToString();
                    }
                }

                try
                {
                    String pTxt = HashRegis.Trim();
                    byte[] plainText = Encoding.Unicode.GetBytes(pTxt);

                    String assinatura = System.Text.Encoding.ASCII.GetString((byte[])dataSet.GetDirect(0, "hashcode"));
                    byte[] encodedMessage = Convert.FromBase64String(assinatura);
                    ContentInfo contentInfo = new ContentInfo(plainText);
                    SignedCms signedCms = new SignedCms(contentInfo, true);
                    signedCms.Decode(encodedMessage);
                    signedCms.CheckSignature(true);
                    //Poderia usar-se o Qcertificate embutido na assinatura to algum proposito...
                    //X509Certificate2 certificate = signedCms.Certificates[0];
                }
                catch (System.Security.Cryptography.CryptographicException e)
                {
                    // So efecuta um update se houver alteração
                    if (!dataSet.GetDirect(0,"hashcode").Equals(""))
                    {
                        query = @"UPDATE " + nomeTabelaBD + @" SET
                            hashcode= ''
                            WHERE " + nomeTabelaBD + "." + nomeChavePrimaria + "='" + internalCode + "'";

                        sp.openConnection();
                        sp.executeNonQuery(query);
                        sp.closeConnection();

                    }
                    //lanca erro
                    throw new BusinessException("Erro na validação da assinatura.", "GlobalFunctions.validarAssinatura", "Error validating signature: " + e.Message, e);
                }
            }
            return "1";
        }

        public string[] returnFieldsSignature(IArea area, string internalCode)
        {
            return returnFieldsSignature(area.QSystem, area.TableName, internalCode);
        }

        /// <summary>
        /// Retorna informação to se poder assinar um documento
        /// </summary>
        /// <param name="nomePrograma">Name do programa (prefixo das tables)</param>
        /// <param name="nomeTabela">O name da table</param>
        /// <param name="nomeCodigoInterno">O name do Qfield do codigo interno</param>
        /// <param name="codigoInterno">O Código Interno utilizado</param>
        /// <returns>Devolve um array com o codigo do hash usado no momento e o Qvalue dos fields to assinar</returns>
        //[Obsolete("User 'string devolverCamposAssinatura(IArea area, string codigoInterno)' instead")]
        public string[] returnFieldsSignature(string QtableName, string internalCode)
        {
            return returnFieldsSignature(null, QtableName, internalCode);
        }

        private string[] returnFieldsSignature(string schema, string QtableName, string internalCode)
        {
            String nomeTabelaBD = Configuration.Program + QtableName.ToUpper();
            String nomeChavePrimaria = Area.GetInfoArea(QtableName).PrimaryKeyName;

            String HashRegis = "";

            // Que fields usar ? //////////////////////////////////////////////////////////////////////
            string tableName = "hashcd";
            SelectQuery qsCampos = new SelectQuery()
                .Select("hashcd", "codhashcd")
                .Select("hashcd", "campos")
                .From(tableName, "hashcd")
                .Where(CriteriaSet.And()
                    .Equal(SqlFunctions.Upper(new ColumnReference("hashcd", "tabela")), nomeTabelaBD.ToUpper()))
                .OrderBy("hashcd", "datacria", Quidgest.Persistence.GenericQuery.SortOrder.Descending)
                .PageSize(1);

            //Vai buscar os fields
            DataMatrix registoHash = sp.Execute(qsCampos);
            String codhashcd = registoHash.GetString(0, "hashcd.codhashcd");
            String fields = registoHash.GetString(0, "hashcd.campos");

            // Se não forem definidos fields to a table retorna uma excepção
            if (!(fields.Length > 0))
            {
                throw new BusinessException("Não foram definidos campos para a assinatura.", "GlobalFunctions.devolverCamposAssinatura", "The table " + nomeTabelaBD + " has no fields.");
            }

            // Quais sao os Qvalues desses fields ? ///////////////////////////////////////////////////
            //Constroi uma query to ir buscar os Qvalues desses fields
            string[] camposArray = fields.Split(',');

            SelectQuery qs = new SelectQuery();
            foreach (string Qfield in camposArray)
            {
                qs.Select(nomeTabelaBD, Qfield);
            }
            qs.From(schema, nomeTabelaBD, nomeTabelaBD);
            qs.Where(CriteriaSet.And()
                .Equal(nomeTabelaBD, nomeChavePrimaria, internalCode));

            //Vai buscar os Qvalues
            DataMatrix fieldsvalues = sp.Execute(qs);

            /* Esta verificação deve ser feita do lado do cliente
            //Por vezes usa-se o @ no Qfield opercria outra vezes não
            String userAt = "@" + this.user.Name;
            String user = this.user.Name;

            String criadorDocumento = fieldsvalues["opercria"].ToString();
            if (!(user.ToUpper().Equals(criadorDocumento.ToUpper()) || userAt.ToUpper().Equals(criadorDocumento.ToUpper()))){
                throw new BusinessException("Não pode assinar um documento que não foi criado por si.", "GlobalFunctions.devolverCamposAssinatura", "The user " + user + " is trying to sign a document created by " + criadorDocumento + ".");
            }
            */

            Dictionary<string, framework.Field> camps = Area.GetInfoArea(QtableName).DBFields;
            //To todos os fields definidos no array
            foreach (string fieldName in camposArray)
            {
                framework.Field Qfield;
                camps.TryGetValue(fieldName, out Qfield);
                //Se for uma data é preciso ter em atençao a formatting
                if (Qfield.FieldType.Equals(CSGenio.framework.FieldType.DATA) || Qfield.FieldType.Equals(CSGenio.framework.FieldType.DATAHORA))
                {
                    String l = fieldsvalues.GetDirect(0, nomeTabelaBD + "." + fieldName).ToString();
                    if (l.Equals(""))
                        HashRegis += "__/__/____";
                    else
                        HashRegis += l.Replace("-", "/").Substring(0, 10);
                }
                else
                {
                        if (Qfield.FieldType.Formatting == FieldFormatting.GUID && fieldsvalues.GetDirect(0, nomeTabelaBD + "." + fieldName).ToString().Length != 0)
                            HashRegis += fieldsvalues.GetDirect(0, nomeTabelaBD + "." + fieldName).ToString().ToUpper();
                        else
                            HashRegis += fieldsvalues.GetDirect(0, nomeTabelaBD + "." + fieldName).ToString();
                }
            }

            // Create um "array" com o codigo da hash que foi utilizada e os Qvalues
            string[] result = new string[2];
            result[0] = codhashcd;
            result[1] = HashRegis.Trim();
            return result;
        }

        /// <summary>
        /// Verifica e escreve a assinatura na base de dados
        /// </summary>
        /// <param name="nomePrograma">Name do programa (prefixo das tables)</param>
        /// <param name="nomeTabela">O name da table</param>
        /// <param name="nomeCodigoInterno">O name do Qfield do codigo interno</param>
        /// <param name="codigoInterno">O codigo interno utilizado</param>
        /// <param name="codhashcd">O codigo da hash utilizada</param>
        /// <param name="texto">O text que foi assinado (to se poder verificar com a assinatura)</param>
        /// <param name="assinatura">A assinatura</param>
        /// <returns>Devolve 1 se a assinatura for bem sucedida ou 0 no caso contrário.</returns>
        public string writeSignature(string QtableName, string internalCode, string signatureInfo)
        {
            // O name da table na base de dados é o name do module concatenado com a table
            String nomeTabelaBD = Configuration.Program + QtableName.ToUpper();
            String nomeChavePrimaria = Area.GetInfoArea(QtableName).PrimaryKeyName;

            string[] assinaturaCampos = signatureInfo.Split(';');
            String codhashcd = assinaturaCampos[0];
            String text = assinaturaCampos[1];
            String assinatura = assinaturaCampos[2];

            try
            {
                //Verificar a assinatura antes de escrever
                byte[] plainText = Encoding.Unicode.GetBytes(text);
                byte[] encodedMessage = Convert.FromBase64String(assinatura);
                ContentInfo contentInfo = new ContentInfo(plainText);
                SignedCms signedCms = new SignedCms(contentInfo, true);
                signedCms.Decode(encodedMessage);
                signedCms.CheckSignature(true);

                //Se a assinatura for validate entao escreve na BD
                String query = @"UPDATE " + nomeTabelaBD + @" SET
                            hashcode = '" + assinatura + @"' ,
                            codhashcd= '" + codhashcd + @"'
                            WHERE " + nomeTabelaBD + "." + nomeChavePrimaria + "='" + internalCode + "'";

                sp.openConnection();
// USE /[MANUAL PRO VALIDAASSINA]/
                int linhas = sp.executeNonQuery(query);
                if (linhas != 1)
                    throw new BusinessException("Ocorreu um erro ao assinar.", "GlobalFunctions.escreverAssinatura", "There were " + linhas + " records updated.");
// USE /[MANUAL PRO ONASSINA]/

                sp.closeConnection();
                return "1";
            }
            catch (System.Security.Cryptography.CryptographicException e)
            {
                throw new BusinessException("Assinatura invalida, o documento não foi assinado.", "GlobalFunctions.escreverAssinatura", "Error saving the signature: " + e.Message, e);
            }
        }

        /// <summary>
        /// Regista o number de serie do Qcertificate na db
        /// </summary>
        /// <param name="numerodeserie"></param>
        /// <returns></returns>
        public int registerCertificateSerialNumber(String serialNumber)
        {
            UpdateQuery queryUp = new UpdateQuery()
                .Update("USERLOGIN")
                .Set("certsn", serialNumber)
                .Where(CriteriaSet.And()
                    .Equal("USERLOGIN", "codpsw", user.Codpsw));

            int n = sp.Execute(queryUp);

            return n;
        }

        /// <summary>
        /// Created by [FA] at [2012.10.29]
        /// Updated by [CJP] at [2014.10.27]
        /// Gera o documento através de um template
        /// </summary>
        /// <param name="res">Hashtable com o historial</param>
        /// <param name="area">Name da Área dos templates</param>
        /// <param name="campo">Name do Qfield que contém a key primária da table dos templates</param>
        /// <param name="valor">Value da key primária do template to geração</param>
        /// <returns>FileInfo do documento gerado</returns>
        /// <remarks>Retorna null se não existirem queries manuais</remarks>
        public FileInfo CreateDocum(Hashtable kV, string area, string Qfield, string Qvalue)
        {
            Area areaObj = Area.createArea(area, user, user.CurrentModule);
            string[] fields = new string[] { area + ".path", area + ".outname", area + ".tpdoc" };
            areaObj.insertNamesFields(fields);
            areaObj.selectOne(CriteriaSet.And().Equal(area, Qfield, Qvalue), null, "", sp);
            sp.closeConnection();

            String path = Conversion.internalString2InternalValidString(areaObj.returnValueField(area + ".path"));
            String outname = Conversion.internalString2InternalValidString(areaObj.returnValueField(area + ".outname"));
            String tpdoc = Conversion.internalString2InternalValidString(areaObj.returnValueField(area + ".tpdoc"));

            var engine = new GenioServer.business.DocumentEngine(this.sp, this.user, kV);
            String output = engine.GenerateDocument(path, outname, tpdoc);
            FileInfo info = new FileInfo(output);

            return info;
        }

        /// <summary>
        /// Created by [CJP] at [2014.10.27]
        /// Gera o documento através de um template
        /// </summary>
        /// <param name="res">string com o historial</param>
        /// <param name="area">Name da Área dos templates</param>
        /// <param name="campo">Name do Qfield que contém a key primária da table dos templates</param>
        /// <param name="valor">Value da key primária do template to geração</param>
        /// <returns>string com o Qresult do documento gerado</returns>
        /// <remarks>Implementação QWeb</remarks>
        public string CreateDocumQweb(string res, string area, string Qfield, string Qvalue)
        {
            String[] r = res.Split('|');
            Hashtable kV = new Hashtable();
            for (int i = 0; i < r.Length - 1; i += 2)
                kV[r[i]] = r[i + 1];

            String codlig = kV["key"].ToString();

            FileInfo documentoGerado = CreateDocum(kV, area, Qfield, Qvalue);

            if (documentoGerado != null)
            {
                Resource recfich = new ResourceFile(documentoGerado.Name, documentoGerado.FullName);
                string recSer = QResources.CreateTicketEncryptedBase64(user.Name, user.Location, recfich);
                string linkRec = System.Web.HttpUtility.UrlEncode(recSer);
                //to apanhar os casos em que chega a null
                return linkRec + "[" + "Documento criado com sucesso!" + "[" + documentoGerado.Name;
            }
            else
                return "";
        }



        /// <summary>
        /// Created by [ARR] at [2015-05-25]
        /// Função que identifica o perfil do user
        /// </summary>
        /// <returns></returns>
        public List<object> GetUserProfile()
        {
            List<object> res = new List<object>();
            string name = user.Name;
            ResourceQuery foto = null;

            List<Profile> profiles = Profile.GetProfiles();

            //apenas construimos a query se tivermos perfis definidos
            if (profiles.Count > 0)
            {
                //vamos juntar tudo numa so query
                //porque so vamos retornar dados se existir apenas um perfil válido
                SelectQuery sqlProfile = null;
                foreach (Profile p in profiles)
                {
                    SelectQuery sql = new SelectQuery();

                    //adicionar a key primária da table profile
                    sql.Select(p.Key, "chave");

                    //indicar a table do profile numa das colunas to que seja genérico a leitura final
                    sql.Select(new SqlValue(p.ProfileArea.Alias), "tabela");
                    //definir a source dos dados
                    sql.From(p.ProfileArea);

                    foreach(Relation rel in p.Relations)
                    {
                        sql.Join(rel.SourceTable, rel.AliasSourceTab)
                            .On(CriteriaSet.And().Equal(rel.AliasSourceTab, rel.SourceRelField, rel.AliasTargetTab, rel.TargetRelField));
                    }

                    //aplicar o limite do user
                    sql.Where(CriteriaSet.And().Equal(CSGenioApsw.FldCodpsw, User.Codpsw));

                    //se a query principal ainda não tiver definida então passa a ser esta
                    //se já tiver definida então adicionamos um union
                    if (sqlProfile == null)
                        sqlProfile = sql;
                    else
                        sqlProfile.Union(sql, false); //com false to não dar os registos repetidos
                }

                DataMatrix mat = sp.Execute(sqlProfile);
                //só vamos buscar os Qvalues se tivermos apenas um registo de pessoa associado ao user
                if (mat.NumRows == 1)
                {
                    string cod = mat.GetKey(0, "chave");
                    string table = mat.GetString(0, "tabela");
                    Area profileArea = Area.createArea(table, User, User.CurrentModule);

                    Profile profile = profiles.Find(x => x.ProfileArea.Alias == table);

                    //se encontrarmos o profile que identificado na query
                    //e conseguirmos criar uma area tendo como referencia a area base do profile
                    //então vamos buscar o registo
                    if (profile != null && profileArea != null )
                    {
                        //vamos buscar os fields do profile
                        if (sp.getRecord(profileArea, cod, new string[]{ profile.Name.Field, profile.Photo.Field }))
                        {
                            name = DBConversion.ToString(profileArea.returnValueField(profile.Name.FullName));
                            name = GetShortName(name, 30); //reduzir o size do name

                            //apenas cria um resource se existir mesmo uma foto na BD to o registo indicado
                            Byte[] fotoByte = DBConversion.ToBinary(profileArea.returnValueField(profile.Photo.FullName));
                            if (fotoByte.Length != 0)
                                foto = new ResourceQuery(profileArea, profile.Photo.Field, cod);
                        }
                    }
                }
            }

            //o name adiciona sempre seja o do registo ou o do username do login
            res.Add(name);
            //mesmo que a foto vá a null a comunicação consegue traduzir
            res.Add(foto);
            return res;
        }

        /// <summary>
        /// Created by [ARR] at [2015-05-25]
        /// Função que reduz o size do name tendo em conta um maximo indicado
        /// </summary>
        /// <returns></returns>
        public string GetShortName(string name, int size)
        {
            name = name.Trim();

            if (GlobalFunctions.emptyC(name) == 1)
                return "";

            //vamos fazer split do name pelos espaços to depois avaliarmos
            string[] nomes = name.Split(' ');

            //se o name estiver dentro do size pertendido então retornamos
            //se apenas tiver um name também retornamos independentemente do size máximo (o interface irá cortar o excesso)
            if (name.Length <= size || nomes.Length == 1)
                return name;
            else if (nomes.Length > 1) //apenas se tiver mais do que um name
            {
                string primeiro = nomes[0];
                string ultimo = nomes[nomes.Length - 1];

                //se o primeiro mais o ultimo mais o espaço entre eles
                //couber no size definido então retornamos o primeiro e o ultimo name
                if ((primeiro.Length + ultimo.Length + 1) <= size)
                {
                    //retorna o primeiro e o ultimo
                    return string.Join(" ", new string[] { primeiro, ultimo });
                }
                else if ((primeiro.Length + 3) <= size) //primeiro mais espaço mais redução do ultimo name (ex: Pedro S.)
                {
                    //vamos reduzir o ultimo name à 1ª letra do name mais um ponto (ex: Santos = S.)
                    //retorna o primeiro e o ultimo reduzido
                    return string.Join(" ", new string[] { primeiro, ultimo[0] + "." });
                }
                else
                    return primeiro; //retorna apenas o primeiro independentemente do size máximo (o interface irá cortar o excesso)
            }

            return "";
        }

        /// <summary>
        /// Hidrate the list of scripts (Adding reindex functions delegates).
        /// This funtions must be called before "upgradeSchema()"
        /// </summary>
        /// <param name="scripts">List of scripts</param>
        /// <param name="versionReader">A reader for the database version</param>
        /// <param name="zero">Full reindexation</param>
        /// <returns></returns>
        public List<ExecuteQueryCore.RdxScript> HidrateScripts(List<ExecuteQueryCore.RdxScript> scripts, IVersionReader versionReader, bool zero = false)
        {
            int upgrindx;
            decimal dbVersion;
            
            try 
            {
                upgrindx = versionReader.GetDbUpgradeVersion();
                dbVersion = versionReader.GetDbVersion();
            }
            catch (Exception)
            {
                upgrindx = Configuration.VersionUpgrIndxGen;
                dbVersion = 0;
            }


            /*
            * In the database there is a field that stores the version of the last upgrade routine that was ran, its called upgrindx.
            * Despite looking like it on the surface, the script do not run in a sequencial order (like version 1, 2, 3 ...), instead
            * they are sorted from lowest version to highest AND FROM BEFORE THE SCHEMA TO AFTER. This order is does NOT change, unless
            * the user edits something ofc.
            *
            * Example:
            *
            * Scritps: 1 - Before schema | 2 - After schema | 3 - After schema | 4 - Before schema
            *
            * Instead of running them sequencially -> 1, 2, 3, 4
            * We sort them like explained above -> 1, 4, 2, 3
            *
            * In this example, if every script ran fine, the number that is stored on the database will 3, since its the version that was
            * ran last.
            *
            * Here what we do is we fetch the last ran version and we clear all the scripts that were ran beforehand
            */
            ReindexFunctions rdxfunc = new ReindexFunctions(sp, user, zero);
            for (int i = scripts.Count - 1; i >= 0; i--)
            {
                //Check if script needs to be run by looking at the specified Min and Max DB versions
                if ((!String.IsNullOrEmpty(scripts[i].MinDbVersion) && Convert.ToInt32(scripts[i].MinDbVersion) != 0 && Convert.ToInt32(scripts[i].MinDbVersion) > dbVersion)
                        || (!String.IsNullOrEmpty(scripts[i].MaxDbVersion) && Convert.ToInt32(scripts[i].MaxDbVersion) != 0 && Convert.ToInt32(scripts[i].MaxDbVersion) < dbVersion))
                {
                    scripts.RemoveAt(i);
                    continue;
                }

                if (scripts[i].Script == "UpgradeClient" || scripts[i].Script == "UpgradeClient.sql")
                    //Since the scripts are not sequencially executed, we compare their indexes and not the versions themselfs to know if
                    //they need to be removed or not
                    if (i < scripts.IndexOf(scripts.Find(ord => ord.Version == upgrindx.ToString())))
                    {
                        scripts.RemoveAt(i);
                        continue;
                    }

                if (scripts[i].Type != null && scripts[i].Type.ToUpper() == "CS")
                {
                    if (String.IsNullOrEmpty(scripts[i].Version))
                        scripts[i].Execute = (Action<System.Threading.CancellationToken>)Delegate.CreateDelegate(typeof(Action<System.Threading.CancellationToken>), rdxfunc, rdxfunc.GetType().GetMethod(scripts[i].Script));
                    else
                        scripts[i].Execute = (Action<System.Threading.CancellationToken>)Delegate.CreateDelegate(typeof(Action<System.Threading.CancellationToken>), rdxfunc, rdxfunc.GetType().GetMethod(scripts[i].Script + scripts[i].Version));
                }
            }

            //We remove the last ran index last because if we do it before the others, we wont know what index to compare the records to
            int idx = scripts.IndexOf(scripts.Find(ord => ord.Version == upgrindx.ToString()));
            if (idx != -1)
                scripts.RemoveAt(idx);

            return scripts;
        }




        /// <summary>
        /// Access to value of the certain EPH.
        /// </summary>
        /// <param name="usr">Current user</param>
        /// <param name="ephID">EPH Identifier</param>
        /// <returns>EPH (first) Value</returns>
        public static string GetEph(User user, string ephID)
        {
            var values = UserFactory.GetEPH(user, ephID);
            if (values != null && values.Length > 0)
                return values[0];

            return null;
        }

        /// <summary>
        /// Check if a user as access to a certain role
        /// </summary>
        /// <param name="usr">User we want to check</param>
        /// <param name="roleId">Role Identifier</param>
        /// <returns>true if the user has access to a certain role</returns>
        public static bool HasRole(User user, string roleId)
        {
            var role = Role.GetRole(roleId);
            return user.VerifyAccess(role);
        }

        /// <summary>
        /// Checks if a given feature is active in this application for this client.
        /// </summary>
        /// <param name="feature">The feature name</param>
        /// <returns>True if the feature is active</returns>
        public static bool IsFeatureActive(string feature)
        {
            switch (feature)
            {
                default :
                    return false;
            }
        }

        /// <summary>
        /// Converts the given Genio language id to it's correspondent platform language id
        /// </summary>
        /// <param name="languageId">The language id to convert</param>
        /// <returns>A string with the language id, or null if the specified id doesn't exist</returns>
        public static string GetClientLang(string languageId)
        {
            switch (languageId)
            {
                case "por":
                    return "PTPT";
            }

            return null;
        }

        /// <summary>
        /// My application theme variables
        /// </summary>
        private static readonly Dictionary<string, string> MYAPP_THEME_VARIABLES = new Dictionary<string, string>()
        {
            { "$footer-bg", "transparent" },
            { "$menu-sidebar-width", "16rem" },
            { "$menu-behaviour", "partial_collapse" },
            { "$compactheader", "false" },
            { "$save-icon", "floppy-disk" },
            { "$compactstyle", "true" },
            { "$border-radius", "0.25rem" },
            { "$table-striped", "false" },
            { "$table-head-inverse", "false" },
            { "$table-vertical-border", "true" },
            { "$enable-table-wrap", "true" },
            { "$font-size-base", "0.9rem" },
            { "$font-family-sans-serif", "\"Lato\", Roboto, \"Helvetica Neue\", Arial, sans-serif, \"Apple Color Emoji\", \"Segoe UI Emoji\", \"Segoe UI Symbol\", \"Noto Color Emoji\"" },
            { "$font-headings", "$font-family-sans-serif" },
            { "$primary", "#008ad2" },
            { "$secondary", "#001d31" },
            { "$highlight", "#ff8241" },
            { "$action-focus-width", "2px" },
            { "$action-focus-style", "solid" },
            { "$action-focus-color", "#201060" },
            { "$input-focus-color", "rgba(0, 169, 206, 0.35)" },
            { "$button-focus-color", "rgba(238, 96, 2, 0.5)" },
            { "$body-bg", "$white" },
            { "$body-color", "#202428" },
            { "$input-btn-padding-y", "0.26rem" },
            { "$input-btn-padding-x", "0.25rem" },
            { "$enable-switch-single-label", "false" },
            { "$wizard-steps", "circle" },
            { "$wizard-content", "standard" },
            { "$btn-align-right", "false" },
            { "$menu-multi-level", "true" },
            { "$primary-light", "#cde5ff" },
            { "$primary-dark", "#006398" },
            { "$success", "#28a745" },
            { "$danger", "#b71c1c" },
            { "$light", "#EAEBEC" },
            { "$red", "#b71c1c" },
            { "$info", "#17a2b8" },
            { "$warning", "#ffa900" },
            { "$gray", "#7C858D" },
            { "$gray-light", "#C4C5CA" },
            { "$gray-dark", "#40474F" },
            { "$navbar-font-size", "0.9rem" },
            { "$navbar-font-weight", "400" },
            { "$custom-tab-navigation", "false" },
            { "$group-border-top", "none" },
            { "$group-border-bottom", "none" },
            { "$hover-item", "rgb($primary-light-rgb / 0.5)" },
            { "$header-bg", "$background" },
            { "$menu-multi-level-border", "false" }
        };

        /// <summary>
        /// Access to value of the certain Variable in current app.
        /// </summary>
        /// <param name="variable">variable name</param>
        /// <returns>theme variable Value</returns>
        public static string GetAppThemeVariable(string variable)
        {
            return MYAPP_THEME_VARIABLES[variable];
        }

        /// <summary>
        /// Access to value of the certain Variable from a specific app.
        /// </summary>
        /// <param name="appID">apps acronym</param>
        /// <param name="variable">variable name</param>
        /// <returns>theme variable Value</returns>
        public static string GetThemeVariable(string appID, string variable)
        {
            if ("MYAPP" == appID)
                return MYAPP_THEME_VARIABLES[variable];
            return "";
        }

        /// <summary>
        /// Splits the provided geometry collection into a list of geometries/polygons
        /// </summary>
        /// <param name="geometry">The geometry</param>
        /// <returns>A collection of geometries/polygons</returns>
        public static ICollection<CSGenio.framework.Geography.GeographicShape> SplitGeometry(CSGenio.framework.Geography.GeographicData geometry)
        {
            return CSGenio.framework.Geography.GeographicData.SplitGeometry(geometry);
        }

        /// <summary>
        /// Transforms a list of geometries/polygons into a geometry collection
        /// </summary>
        /// <param name="geometries">The list</param>
        /// <returns>A geometry collection with all the geometries in the list</returns>
        public static CSGenio.framework.Geography.GeographicData JoinGeometries(ICollection<CSGenio.framework.Geography.GeographicShape> geometries)
        {
            return CSGenio.framework.Geography.GeographicData.JoinGeometries(geometries);
        }

        /// <summary>
        /// Converts a given string to a QR code representation
        /// </summary>
        /// <param name="text">The string to convert</param>
        /// <returns>A byte array representing the result of the convertion</returns>
        public static byte[] StringToQRcode(string text)
        {
            if (String.IsNullOrEmpty(text))
                return null;

            QRCoder.QRCodeGenerator qrGenerator = new QRCoder.QRCodeGenerator();
            // Error correction: Level Q (Quartile) - 25% of data bytes can be restored.
            QRCoder.QRCodeData qrCodeData = qrGenerator.CreateQrCode(text, QRCoder.QRCodeGenerator.ECCLevel.Q);
            QRCoder.QRCode qrCode = new QRCoder.QRCode(qrCodeData);
            System.Drawing.Bitmap qrCodeImage = qrCode.GetGraphic(20);

            using (var stream = new MemoryStream())
            {
                qrCodeImage.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                return stream.ToArray();
            }
        }

        /// <summary>
        /// Returns the access level associated with the provided role.
        /// </summary>
        /// <param name="level">The level.</param>
        /// <param name="roleId">The role identifier.</param>
        /// <returns></returns>
        public static decimal GetLevelFromRole(decimal level, string roleId)
        {
            if (string.IsNullOrEmpty(roleId))
                return level;

            Role role = Role.GetRole(roleId);

            if (role != null && (role.Type == RoleType.LEVEL || role.Type == RoleType.SYSTEM))
                return role.GetLevelInt();
            return 0;
        }

        /// <summary>
        /// Import the users from a domain active directory is not already in PSW table
        /// </summary>
        /// <param name="domain">Domain from AD</param>
        /// <returns>Status of the import</returns>
        public StatusMessage ImportUsersFromAD(string domain)
        {
            if (!CSGenio.framework.Configuration.Security.IdentityProviders.Exists(p => p.Type.Equals("GenioServer.security.LdapIdentityProvider")))
                return StatusMessage.Error(Translations.Get("O tipo de login não permite a importação a partir de Active directory.", user.Language));

            int usersCreated = 0;
            try
            {
                sp.openConnection();

                List<string> userList = new List<string>();

                using (var context = new System.DirectoryServices.AccountManagement.PrincipalContext(System.DirectoryServices.AccountManagement.ContextType.Domain, domain))
                {
                    using (var searcher = new System.DirectoryServices.AccountManagement.PrincipalSearcher(new System.DirectoryServices.AccountManagement.UserPrincipal(context)))
                    {
                        foreach (var result in searcher.FindAll())
                        {
                            DirectoryEntry de = result.GetUnderlyingObject() as DirectoryEntry;
                            string usercontrol = de.Properties["userAccountControl"].Value.ToString();

                            //Disable acounts code 514 = NORMAL_ACCOUNT (512) + ACCOUNTDISABLE (2)
                            if (!usercontrol.Equals("514"))
                                userList.Add(de.Properties["samAccountName"].Value.ToString());
                        }
                    }
                }

                //Checks for each user if is not already in the database
                foreach (string usr in userList)
                {
                    SelectQuery selQuery = new SelectQuery()
                        .Select(CSGenioApsw.FldCodpsw)
                        .From(Area.AreaPSW)
                        .Where(CriteriaSet.And()
                            .Equal(CSGenioApsw.FldNome, usr)
                            .Equal(CSGenioApsw.FldZzstate, 0)
                        )
                        .PageSize(1);

                    var userExist = sp.Execute(selQuery);

                    //If the user doesn't existe , create
                    if (userExist.NumRows == 0)
                    {
                        CSGenioApsw userPsw = new CSGenioApsw(User)
                        {
                            ValNome = usr
                        };
                        userPsw.insert(sp);
                        usersCreated++;
                    }
                }
                sp.closeConnection();
            }
            catch (Exception ex)
            {
                return StatusMessage.Error(ex.Message);
            }
            return StatusMessage.OK(string.Format(Translations.Get("Importação concluída com sucesso. Foram importados {0} utilizadores", user.Language), usersCreated));
        }

        private static string GetApiProperty(string property, bool isRequired = true)
        {
            bool exists = Configuration.ExistsProperty(property);

            if (!exists)
            {
                if (isRequired)
                    CSGenio.framework.Log.Error(string.Format("Error while trying to obtain external service token: property \"{0}\" not found!", property));

                return null;
            }

            return Configuration.GetProperty(property) ?? "";
        }

        /// <summary>
        /// Tries to obtain an access token for an external service, using the data in the specified configuration
        ///
        /// The configuration must have the following properties:
        /// - [configId]_BASE_URL: The base url for the service
        /// - [configId]_TOKEN_PATH: The path to get the token, relative to the base url
        /// - [configId]_USERNAME: The username to access the service
        /// - [configId]_PASSWORD: The password to access the service
        /// - [configId]_SERVICE_TYPE: The type of the service (ex: ArcGis)
        ///
        /// Additional properties can eventually be added, when necessary, for specific implementations
        /// </summary>
        /// <param name="configId">The id (prefix) of the configuration (in WebAdmin)</param>
        /// <returns>A token to access the external service, or null if some error prevents it's obtainment</returns>
        public static string GetExternalServiceToken(string configId)
        {
            if (string.IsNullOrWhiteSpace(configId))
            {
                CSGenio.framework.Log.Error("Error while trying to obtain external service token: the config id can't be empty!");
                return null;
            }

            string serviceType = GetApiProperty(configId + "_SERVICE_TYPE");

            if (string.IsNullOrWhiteSpace(serviceType))
                return null;

            string baseUrl = GetApiProperty(configId + "_BASE_URL"),
                tokenPath = GetApiProperty(configId + "_TOKEN_PATH");

            // Takes care of trailing slashes in the url.
            if (baseUrl.EndsWith(@"/"))
                baseUrl = baseUrl.Remove(baseUrl.Length - 1);
            if (!tokenPath.StartsWith(@"/"))
                tokenPath = "/" + tokenPath;

            var parameters = new Dictionary<string, string>()
            {
                { "baseUrl", baseUrl },
                { "tokenPath", tokenPath },
                { "username", GetApiProperty(configId + "_USERNAME") },
                { "password", GetApiProperty(configId + "_PASSWORD") }
            };

            // All required parameters must be filled.
            if (parameters.Values.Any(v => string.IsNullOrWhiteSpace(v)))
                return null;

            // Add optional parameters.
            parameters["expiration"] = GetApiProperty(configId + "_EXPIRATION", false);

            Type type = Type.GetType(string.Format("CSGenio.business.{0}", serviceType));
            IMapServiceProvider provider = (IMapServiceProvider) Activator.CreateInstance(type);
            return provider.GetToken(parameters);
        }

        /// <summary>
        /// Tries to obtain the glob record
        /// </summary>
        /// <returns>The glob record, if it exists, or null if it doesn't</returns>
        public DbArea GetGlob()
        {
            return null;
        }
    }
}
