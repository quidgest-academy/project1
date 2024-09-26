using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using CSGenio.framework;
using CSGenio.persistence;
using Quidgest.Persistence;
using Quidgest.Persistence.GenericQuery;
using System.Collections.Concurrent;

namespace CSGenio.business
{
    /// <summary>
    /// Classe genérica que representa uma área, todas as áreas criadas deverão extender esta
    /// classe e implementar os métodos abstractos
    /// Os atributos:
    /// - fields é uma hash cujo identifier é (alias+fieldName), cada entrada aponta to um Field
    /// todasAreas hashtable que permite obter todas as áreas
    /// </summary>
    public abstract class Area : IArea
    {
        public static AreaRef AreaCONTC { get { return m_AreaCONTC; } }
        private static AreaRef m_AreaCONTC = new AreaRef("PRO", "procontc", "contc");
        public static AreaRef AreaPAIS { get { return m_AreaPAIS; } }
        private static AreaRef m_AreaPAIS = new AreaRef("PRO", "propais", "pais");
        public static AreaRef AreaCIDAD { get { return m_AreaCIDAD; } }
        private static AreaRef m_AreaCIDAD = new AreaRef("PRO", "procidad", "cidad");
        public static AreaRef AreaS_PAX { get { return m_AreaS_PAX; } }
        private static AreaRef m_AreaS_PAX = new AreaRef("PRO", "asyncprocessattachments", "s_pax");
        public static AreaRef AreaPSW { get { return m_AreaPSW; } }
        private static AreaRef m_AreaPSW = new AreaRef("PRO", "userlogin", "psw");
        public static AreaRef AreaS_APR { get { return m_AreaS_APR; } }
        private static AreaRef m_AreaS_APR = new AreaRef("PRO", "asyncprocess", "s_apr");
        public static AreaRef AreaPROPR { get { return m_AreaPROPR; } }
        private static AreaRef m_AreaPROPR = new AreaRef("PRO", "propropr", "propr");
        public static AreaRef AreaALBUM { get { return m_AreaALBUM; } }
        private static AreaRef m_AreaALBUM = new AreaRef("PRO", "proalbum", "album");
        public static AreaRef AreaPNASC { get { return m_AreaPNASC; } }
        private static AreaRef m_AreaPNASC = new AreaRef("PRO", "propais", "pnasc");
        public static AreaRef AreaAGENT { get { return m_AreaAGENT; } }
        private static AreaRef m_AreaAGENT = new AreaRef("PRO", "proagente_imobiliario", "agent");
        public static AreaRef AreaMEM { get { return m_AreaMEM; } }
        private static AreaRef m_AreaMEM = new AreaRef("PRO", "promem", "mem");
        public static AreaRef AreaPMORA { get { return m_AreaPMORA; } }
        private static AreaRef m_AreaPMORA = new AreaRef("PRO", "propais", "pmora");
        public static AreaRef AreaS_NES { get { return m_AreaS_NES; } }
        private static AreaRef m_AreaS_NES = new AreaRef("PRO", "notificationemailsignature", "s_nes");
        public static AreaRef AreaS_ARG { get { return m_AreaS_ARG; } }
        private static AreaRef m_AreaS_ARG = new AreaRef("PRO", "asyncprocessargument", "s_arg");
        public static AreaRef AreaS_UA { get { return m_AreaS_UA; } }
        private static AreaRef m_AreaS_UA = new AreaRef("PRO", "userauthorization", "s_ua");
        public static AreaRef AreaS_NM { get { return m_AreaS_NM; } }
        private static AreaRef m_AreaS_NM = new AreaRef("PRO", "notificationmessage", "s_nm");
        //areas hardcoded
        public static AreaRef AreaDELEGA { get { return m_AreaDELEGA; } }
        private static AreaRef m_AreaDELEGA = new AreaRef("PROdelega", "delega");
        public static AreaRef AreaPSWUP { get { return m_AreaPSWUP; } }
        private static AreaRef m_AreaPSWUP = new AreaRef("UserLogin", "pswup");
        public static AreaRef AreaMQQUEUES { get { return m_AreaMQQUEUES; } }
        private static AreaRef m_AreaMQQUEUES = new AreaRef("PROmqqueues", "mqqueues");
        public static AreaRef AreaUSERAUTHORIZATION { get { return m_AreaUSERAUTHORIZATION; } }
        private static AreaRef m_AreaUSERAUTHORIZATION = new AreaRef("userauthorization", "userauthorization");
        public static AreaRef AreaNOTIFICATIONEMAILSIGNATURE { get { return m_AreaNOTIFICATIONEMAILSIGNATURE; } }
        private static AreaRef m_AreaNOTIFICATIONEMAILSIGNATURE = new AreaRef("notificationemailsignature", "notificationemailsignature");
        public static AreaRef AreaNOTIFICATIONMESSAGE { get { return m_AreaNOTIFICATIONMESSAGE; } }
        private static AreaRef m_AreaNOTIFICATIONMESSAGE = new AreaRef("notificationmessage", "notificationmessage");
		//FOR: USER_TABLE_CONFIG (VueJS)
		//BEGIN: User table configuration
        public static AreaRef AreaTBLCFG { get { return m_AreaTBLCFG; } }
        private static AreaRef m_AreaTBLCFG = new AreaRef("PROtblcfg", "tblcfg");
        public static AreaRef AreaTBLCFGSEL { get { return m_AreaTBLCFGSEL; } }
        private static AreaRef m_AreaTBLCFGSEL = new AreaRef("PROtblcfgsel", "tblcfgsel");
		//END: User table configuration
        public static AreaRef AreaLSTUSR { get { return m_AreaLSTUSR; } }
        private static AreaRef m_AreaLSTUSR = new AreaRef("PROlstusr", "lstusr");
        public static AreaRef AreaLSTCOL { get { return m_AreaLSTCOL; } }
        private static AreaRef m_AreaLSTCOL = new AreaRef("PROlstcol", "lstcol");
        public static AreaRef AreaUSRCFG { get { return m_AreaUSRCFG; } }
        private static AreaRef m_AreaUSRCFG = new AreaRef("PROusrcfg", "usrcfg");
        public static AreaRef AreaLSTREN { get { return m_AreaLSTREN; } }
        private static AreaRef m_AreaLSTREN = new AreaRef("PROlstren", "lstren");
        public static AreaRef AreaUSRWID { get { return m_AreaUSRWID; } }
        private static AreaRef m_AreaUSRWID = new AreaRef("PROusrwid", "usrwid");

        /// <summary>
        /// Lista de areas
        /// </summary>
        public static readonly System.Collections.ObjectModel.ReadOnlyCollection<string> ListaAreas = new System.Collections.ObjectModel.ReadOnlyCollection<string>(
            new List<string>() {
            "contc",
            "pais",
            "cidad",
            "s_pax",
            "psw",
            "s_apr",
            "propr",
            "album",
            "pnasc",
            "agent",
            "mem",
            "pmora",
            "s_nes",
            "s_arg",
            "s_ua",
            "s_nm",
        });

        /// <summary>
        /// User
        /// </summary>
        protected User user;
        /// <summary>
        /// Module
        /// </summary>
        protected string module;
        /// <summary>
        /// fields da table
        /// </summary>
        protected Hashtable fields;

        /// <summary>
        /// Type de Key Primária
        /// </summary>
        protected CodeType KeyType;

        //Static class accessed a lot during startup, must have concurrency concerns
        private static ConcurrentDictionary<string, Type> m_areaRegistry = new ConcurrentDictionary<string, Type>();
        
        /// <summary>
        /// Função que dado o identifier da area devolve um objecto da mesma
        /// </summary>
        /// <param name="nome">name da Area</param>
        /// <param name="utilizador">O contexto de user</param>
        /// <returns>Area correspondente</returns>
        public static Area createArea(string name, User user, string module)
        {
            if (name == null)
                throw new BusinessException(null, "Area.criarArea", "Argument [nome] is null.");

            //RS 21.03.2011 Apaguei as listas de areas e passei a usar reflection to obter a informação de uma area
            // Isto permite reduzir o time de inicialização na primeira chamada ao servidor de forma substancial
            // uma vez que existiam problemas em 64 bits (podia demorar 2 minutes)

            Type areaType = GetTypeArea(name);

            return createArea(areaType, user, module);
        }

        /// <summary>
        /// Returns the type of the area from the area name. Also caches the type in memory
        /// </summary>
        /// <param name="name">Area Id</param>
        /// <returns></returns>
        public static Type GetTypeArea(string name)
        {
            if (name == null)
                throw new BusinessException(null, "Area.criarArea", "Argument [nome] is null.");

            if(!m_areaRegistry.ContainsKey(name)) {
                m_areaRegistry[name] = LoadType(name);
            }
            return  m_areaRegistry[name];
        }


        private static Type LoadType(string name) {
            const string classPrefix = "CSGenio.business.CSGenioA";
            //We need to pass an hint for the assembly, or it will only search in CSGenio.core
            string areaName = classPrefix + name + ", GenioServer";
            var type = Type.GetType(areaName);
            //Since there are much more assemblies in GenioServer, we search for CSGenio.core only after not finding in GenioServer. 
            if(type == null)
            {
                areaName = classPrefix + name + ", CSGenio.core";
                type = Type.GetType(areaName);
            }
            return type;
        }

        /// <summary>
        /// Função que dado o tipo da area devolve um objecto da mesma
        /// </summary>
        /// <typeparam name="TArea">O tipo da area</typeparam>
        /// <param name="utilizador">O contexto de user</param>
        /// <returns>Area correspondente</returns>
        public static TArea createArea<TArea>(User user, string module) where TArea : Area
        {
            return (TArea)createArea(typeof(TArea), user, module);
        }

        /// <summary>
        /// Função que dado o identifier da area devolve um objecto da mesma
        /// </summary>
        /// <param name="nome">name da Area</param>
        /// <param name="utilizador">O contexto de user</param>
        /// <returns>Area correspondente</returns>
        private static Area createArea(Type areaType, User user, string module)
        {
            if (areaType == null)
                throw new BusinessException(null, "Area.criarArea", "Argument [areaType] is null.");

            Area result = System.Activator.CreateInstance(areaType, user, module) as Area;
            if (result == null)
                throw new BusinessException(null, "Area.criarArea", "CreateInstance returned null.");

            return result;
        }

        private bool m_isFichaUtilizador = true;
        /// <summary>
        /// True se a ficha deve autenticar e carimbar o user, false caso seja o negócio
        /// </summary>
        /// <remarks>
        /// Os métodos desta classe vão considerar a ficha como sendo gravada pelo user
        ///  e vão autorizar e carimbar-la como tal. Sempre que esse comportamente não é desejado
        ///  esta propriadade deve ser explicitamente colocada a false antes dessas operações.
        /// Tipicamente isto é necessário em operações em cascata ou outras rotinas manuais
        ///  automáticas e deve ser feito imediatamente após instanciar a área. É esperado que
        ///  durante a vida dessa instancia este Qvalue não mude mas o servidor não garante nem
        ///  assume que tal possa acontecer.
        /// </remarks>
        public bool UserRecord
        {
            get { return m_isFichaUtilizador; }
            set { m_isFichaUtilizador = value; }
        }

        /// <summary>
        /// True if the record is to be validated, false if not
        /// </summary>
        /// <remarks>
        /// By default, this field will be set as true, to prevent the storage of invalid records. 
        /// However, certain situations require the validations to be delayed or even not occur - in these cases,
        /// it is preferable to alter this property instead of the UserRecord flag, since that is used for several
        /// other cases outside of the validation scope. 
        /// </remarks>
        public bool NeedsValidation { get; set; } = true;

        /// <summary>
        /// True se as validações de valores dos campos se devem forçar a validação se o campo é null ou não
        /// </summary>
        private bool m_validateIfIsNull = false;
        public bool ValidateIfIsNull
        {
            get { return m_validateIfIsNull; }
            set { m_validateIfIsNull = value; }
        }

        /// <summary>
        /// Finds the meta information about a field reference
        /// </summary>
        /// <param name="fieldRef"></param>
        /// <returns>The Field infomation related to the field reference</returns>
        public static Field GetFieldInfo(FieldRef fieldRef)
        {
            return GetInfoArea(fieldRef.Area).DBFields[fieldRef.Field];
        }

        /// <summary>
        /// Obtem a informação sobre uma area dado o seu name
        /// </summary>
        /// <param name="nome"></param>
        /// <returns></returns>
        public static AreaInfo GetInfoArea(string name)
        {
            //RS 21.03.2011 Apaguei as listas de areas e passei a usar reflection to obter a informação de uma area
            // Isto permite reduzir o time de inicialização na primeira chamada ao servidor de forma substancial
            // uma vez que existiam problemas em 64 bits (podia demorar 2 minutes)

            Type areaType = GetTypeArea(name);
            if (areaType == null)
                throw new BusinessException(null, "Area.GetInfoArea", "Argument [areaType] is null.");

            return GetInfoArea(areaType);
        }
		
		/// <summary>
        /// Obtem a informação sobre uma area dado o seu tipo
        /// </summary>
        /// <returns></returns>
		public static AreaInfo GetInfoArea<A>() where A : Area 
        {
            return GetInfoArea(typeof(A));
        }
		
        private static AreaInfo GetInfoArea(Type t)
        {
            return t.InvokeMember("GetInformation"
                , System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.InvokeMethod
                , null
                , null
                , null) as AreaInfo;
        }


        /// <summary>
        /// Função que devolve a lista de todas as queues
        /// </summary>
        /// <returns>Lista de queues</returns>
        public static List<GenioServer.business.QueueGenio>  GetAllQueues()
        {
            List<GenioServer.business.QueueGenio> listaQueues = new List<GenioServer.business.QueueGenio>();
            return listaQueues;
        }

        /// <summary>
        /// Clones the properties of another IArea instance into this instance.
        /// </summary>
        /// <param name="other">The IArea instance to clone from.</param>
        /// <exception cref="InvalidOperationException">Thrown when the alias of the provided Area instance does not match the current instance's alias.</exception>
        public void CloneFrom(IArea other)
        {
            if (Alias != other.Alias)
                throw new InvalidOperationException($"Alias mismatch: Unable to clone from the provided Area instance. Expected alias '{Alias}', but received '{other.Alias}'.");

            foreach (string field in other.Fields.Keys)
                insertNameValueField(field, ((RequestedField)other.Fields[field]).Value);
        }

        /// <summary>
        /// Insere os nomes dos fields na area
        /// Usa o name do Qfield "alias+nomecampo" como key
        /// </summary>
        /// <param name="nomeCamposList">Lista de fields separados por vírgulas</param>
        public void insertNamesFields(string[] fieldNames)
        {
            try
            {
                FieldType fieldType;
                RequestedField campoPedido;
                fields = new Hashtable();//inicializar a variável que tem os fields

                for (int i = 0; i < fieldNames.Length; i++)
                {
                    campoPedido = new RequestedField(fieldNames[i], Alias);

                    if (campoPedido.BelongsArea)//se pertence a esta area
                    {
                        //SO 20060816 nos camposBD não está alias.nomecampo
                        fieldType = ((Field)DBFields[campoPedido.Name]).FieldType;
                        campoPedido.FieldType = fieldType;
                    }
                    else
                    {
                        //----------------------------------------------------------------
                        //Este código só é executado na sequancia de um pedido GET1.
                        //TODO: Convinha tentar fazer de outra forma. Não faz muito sentido
                        //  uma area guardar fields de outra area
                        //System.Diagnostics.Debug.Assert(false);

                        if (!campoPedido.WithoutArea)//se tem área mas é outra
                        {
                            //SO 2007.05.29
                            Area areaAux = Area.createArea(campoPedido.Area, User, Module);
                            fieldType = ((Field)areaAux.DBFields[campoPedido.Name]).FieldType;
                            campoPedido.FieldType = fieldType;
                        }

                        //----------------------------------------------------------------
                    }

                    if (!fields.ContainsKey(fieldNames[i]))
                        fields.Add(fieldNames[i], campoPedido);

                }
            }
            catch (GenioException ex)
            {
                throw new BusinessException(ex.UserMessage, "Area.inserirNomesCampos", "Error inserting field names: " + ex.Message, ex);
            }
            catch (Exception ex)
            {
                throw new BusinessException(null, "Area.inserirNomesCampos", "Error inserting field names: " + ex.Message, ex);
            }
        }

        /// <summary>
        /// Metodo auxiliar que Insere ou Altera os nomes e Qvalues dos fields na área
        /// </summary>
        /// <param name="nomeCampos">nomes dos fields</param>
        /// <param name="valoresCampos">Qvalues dos fields que estão em string</param>
        /// <param name="inserir">Bool to indicar se é chamada to introduce</param>
        /// <param name="acrescentar">Bool to indicar se é chamada to acrescentar</param>
        private void AuxNomesValoresCampos(string[] fieldNames, string[] fieldsvalues, bool introduce, bool acrescentar)
        {
            string function;
            if (introduce)
                function = "Area.acrescentarNomesValoresCampos";
            else if (acrescentar)
                function = "Area.inserirNomesValoresCampos";
            else
                throw new BusinessException(null, "Area.AuxNomesValoresCampos", "Arguments [inserir] and [acrescentar] are both false.");
            try
            {
                // se não há fields
                if ((introduce && (fieldNames.Length == 0 || fieldsvalues.Length == 0)) || fieldNames.Length != fieldsvalues.Length)
                    throw new BusinessException(null, "Area.AuxNomesValoresCampos", "Lengths of nomeCampos and valoresCampos don't match or one of these parameters has length 0.");
                else
                {
                    RequestedField campoPedido;
                    if(introduce)
                        fields = new Hashtable(); // inicializar a variável que tem os fields

                    for (int i = 0; i < fieldNames.Length; i++)
                    {
                            campoPedido = new RequestedField(fieldNames[i], Alias);

                            if (campoPedido.BelongsArea)//se pertence a esta area
                            {
                                if (acrescentar && PrimaryKeyName.Equals(campoPedido.Name))
                                    continue;
                                Field Qfield = (Field)DBFields[campoPedido.Name];
                                //SO 20061207 validação dos fields não nulos
                                if (Qfield.NotNull && Qfield.DefaultValue == null)
                                {
                                    if (fieldsvalues[i].Equals(""))
                                        throw new BusinessException("O campo " + Qfield.FieldDescription + " (" + Qfield.Alias + "." + Qfield.Name + ")  é obrigatório mas não está preenchido.", function, "The field " + Qfield.FieldDescription + " (" + Qfield.Alias + "." + Qfield.Name + ")  is mandatory but is not filled.");
                                }

                                if (Qfield.FieldSize < 0 && Qfield.FieldType == FieldType.TEXTO && Qfield.FieldSize < fieldsvalues[i].ToString().Length)
                                    throw new BusinessException("O campo " + Qfield.FieldDescription + " excede a dimensão máxima permitida.", "Area.AuxNomesValoresCampos", "The field " + Qfield.FieldDescription + " exceeds the maximum length allowed.");

                                campoPedido.FieldType = Qfield.FieldType;
                                if (campoPedido.FieldType.Equals(FieldType.IMAGEM_JPEG))
                                    throw new BusinessException("Erro ao gravar a imagem.", "Area.AuxNomesValoresCampos", "The field type JPEG image is not supported by function " + function);
                                else
                                    // RR 01-04-2011 - os fields tipo path e file db não são geridos a este nível, mas sim a um nível superior
                                    campoPedido.Value = Conversion.string2TypeInternal(fieldsvalues[i], Qfield.FieldType.Formatting);
                            }
                            else
                            {
                                //----------------------------------------------------------------
                                //Este código só é executado na sequancia de um pedido GET1.
                                //TODO: Convinha tentar fazer de outra forma. Não faz muito sentido
                                //  uma area guardar fields de outra area
                                //System.Diagnostics.Debug.Assert(false);

                                if (!campoPedido.WithoutArea)//se tem área mas é outra
                                {
                                    //SO 2007.05.29
                                    Area areaAux = Area.createArea(campoPedido.Area, User, Module);
                                    //SO 20060816 na variavel camposBD o name do Qfield não tem o alias antes
                                    Field Qfield = (Field)areaAux.DBFields[campoPedido.Name];
                                    //SO 20061207 validação dos fields não nulos
                                    if (Qfield.NotNull && Qfield.DefaultValue == null)
                                    {
                                        if (fieldsvalues[i].Equals(""))
                                            throw new BusinessException("O campo " + Qfield.FieldDescription + " (" + Qfield.Alias + "." + Qfield.Name + ")  é obrigatório mas não está preenchido.", "Area.AuxNomesValoresCampos", "The field " + Qfield.FieldDescription + " (" + Qfield.Alias + "." + Qfield.Name + ")  is mandatory but is not filled.");
                                    }
                                    campoPedido.FieldType = Qfield.FieldType;
                                    if (Qfield.FieldSize < 0 && Qfield.FieldType == FieldType.TEXTO && Qfield.FieldSize < fieldsvalues[i].ToString().Length)
                                        throw new BusinessException("O campo " + Qfield.FieldDescription + " excede a dimensão máxima permitida.", "Area.AuxNomesValoresCampos", "The field " + Qfield.FieldDescription + " exceeds the maximum length allowed.");
                                    campoPedido.Value = Conversion.string2TypeInternal(fieldsvalues[i], Qfield.FieldType.Formatting);
                                }
                            }
                            if (!fields.ContainsKey(fieldNames[i]))
                                fields.Add(fieldNames[i], campoPedido);
                            else if (acrescentar)
                                fields[fieldNames[i]] = campoPedido;
                    }
                }
            }
            catch (GenioException ex)
            {
                if (ex.ExceptionSite == "Area.AuxNomesValoresCampos")
                    throw;
                throw new BusinessException(ex.UserMessage, "Area.AuxNomesValoresCampos", "Error inserting or changing fields names and values in Area: " + ex.Message, ex);
            }
            catch (Exception ex)
            {
                throw new BusinessException(null, "Area.AuxNomesValoresCampos", "Error inserting or changing fields names and values in Area: " + ex.Message, ex);
            }
        }

        /// <summary>
        /// Insere os nomes e Qvalues dos fields na área
        /// </summary>
        /// <param name="nomeCampos">nomes dos fields</param>
        /// <param name="valoresCampos">Qvalues dos fields que estão em string</param>
        public void insertNamesValuesFields(string[] fieldNames, string[] fieldsvalues)
        {
            AuxNomesValoresCampos(fieldNames, fieldsvalues, true, false);
        }

        /// <summary>
        /// Acrescenta os nomes e Qvalues dos fields na área caso não existam. Actualiza os Qvalues caso contrário
        /// </summary>
        /// <param name="nomeCampos">nomes dos fields</param>
        /// <param name="valoresCampos">Qvalues dos fields que estão em string</param>
        public void addNamesValuesFields(string[] fieldNames, string[] fieldsvalues)
        {
            AuxNomesValoresCampos(fieldNames, fieldsvalues, false, true);
        }

        /// <summary>
        /// Insere na area todos os fields a vazio
        /// </summary>
        public void createEmptyFields()
        {
            Dictionary<string, Field> camposBD = this.DBFields;
            IEnumerator enumCampos = camposBD.Values.GetEnumerator();
            Field Qfield;
            RequestedField campoPedido;

            while (enumCampos.MoveNext())
            {
                Qfield = (Field)enumCampos.Current;
                if (Qfield.FieldType != FieldType.DATACRIA && Qfield.FieldType != FieldType.OPERCRIA && Qfield.FieldType != FieldType.HORACRIA)
                {
                campoPedido = new RequestedField(this.Alias + "." + Qfield.Name, this.Alias);
                campoPedido.FieldType = Qfield.FieldType;
                campoPedido.Value = Qfield.GetValorEmpty();
                if (!this.Fields.ContainsKey(campoPedido.FullName))
                    this.Fields.Add(campoPedido.FullName,campoPedido);
                }
            }
        }

        /// <summary>
        /// Adiciona um Qfield à table
        /// </summary>
        /// <param name="nomeCampo">Name do Qfield</param>
        /// <param name="valorCampo">Value do Qfield</param>
        public void insertNameValueField(string fieldName, object fieldValue)
        {
            try
            {
                RequestedField campoPedido;
                FieldType fieldType;

                if (fields.Contains(fieldName))
                    //SO 20060816
                    campoPedido = (RequestedField)fields[fieldName];
                else
                    campoPedido = new RequestedField(fieldName, Alias);

                if (campoPedido.BelongsArea)//se pertence a esta area
                {
                    //SO 20060816 alteração, os fields da BD só levam o name do Qfield, não o alias
                    fieldType = ((Field)DBFields[campoPedido.Name]).FieldType;
                    campoPedido.FieldType = fieldType;
                    campoPedido.Value = Conversion.internal2InternalValid(fieldValue, fieldType.Formatting);
                    trimPrecision(campoPedido);
                }
                else
                {
                    //----------------------------------------------------------------
                    //Este código só é executado na sequancia de um pedido GET1.
                    //TODO: Convinha tentar fazer de outra forma. Não faz muito sentido
                    //  uma area guardar fields de outra area
                    //System.Diagnostics.Debug.Assert(false);

                    if (!campoPedido.WithoutArea)//se tem área mas é outra
                    {
                        //SO 2007.05.29
                        Area areaAux = Area.createArea(campoPedido.Area, User, Module);
                        fieldType = ((Field)areaAux.DBFields[campoPedido.Name]).FieldType;
                        campoPedido.FieldType = fieldType;
                        campoPedido.Value = Conversion.internal2InternalValid(fieldValue, fieldType.Formatting);
                    }
                    //----------------------------------------------------------------
                }
                if (!fields.ContainsKey(fieldName))
                    fields.Add(fieldName, campoPedido);

            }
            catch (GenioException ex)
            {
                string message = $"Error inserting value in field {fieldName} in area {this.Alias}: {ex.Message}";
                throw new BusinessException(ex.UserMessage, "Area.insertNameValueField", message, ex);

            }
            catch (Exception ex)
            {
                string message = $"Error inserting value in field {fieldName} in area {this.Alias}: {ex.Message}";
                throw new BusinessException(message, "Area.insertNameValueField", message, ex);
            }
        }

        /// <summary>
        /// Trims a field to its declared maximum precision to prevent discrepancies with the database values
        /// </summary>
        /// <param name="field">The requested field to trim</param>
        /// <remarks>
        /// This method assumes the field value was previously normalized to its valid internal type.
        /// This trim is necessary because if you allow temporary values to retain more precision during calculations
        ///  then that extra precision can add up to differences to a calculation done in the SQL fields.
        /// This can happen anywhere but will most commonly happen in SR, for example:
        /// A = [+] (B + C + D) / 3
        /// S = [SR] A
        /// </remarks>
        private void trimPrecision(RequestedField field)
        {
            if(field.FieldType.Formatting == FieldFormatting.FLOAT)
            {
                var dec = DBFields[field.Name].Decimals;
                field.Value = Math.Round((decimal)field.Value, dec);
            }
        }

        /// <summary>
        /// Devolve o Qvalue do Qfield através do name do Qfield
        /// </summary>
        /// <param name="nomeCampo">Name do Qfield</param>
        /// <returns>Value do Qfield</returns>
        public object returnValueField(string fieldName)
        {
            try
            {
                if (fields[fieldName] == null)
                    return DBFields[fieldName.Split('.')[1]].GetValorEmpty();
                else
                    return ((RequestedField)fields[fieldName]).Value;
            }
            catch (GenioException ex)
            {
                throw new BusinessException(ex.UserMessage, "Area.devolverValorCampo", "Error returning the field's value: " + ex.Message, ex);
            }
            catch (Exception ex)
            {
                throw new BusinessException(null, "Area.devolverValorCampo", "Error returning the field's value: " + ex.Message, ex);
            }

        }

        /// <summary>
        /// Devolve o Qvalue do Qfield através do name do Qfield e formatação do Qfield
        /// </summary>
        /// <param name="nomeCampo">Name do Qfield</param>
        /// <param name="formatacao">Formatação do Qfield</param>
        /// <returns>Qvalue do Qfield</returns>
        public object returnValueField(string fieldName, FieldFormatting formatting)
        {
            try
            {
                if (fields[fieldName] == null)
                    return Field.GetValorEmpty(formatting);
                else
                {
                    return ((RequestedField)fields[fieldName]).Value;
                }
            }
            catch (GenioException ex)
            {
                throw new BusinessException(ex.UserMessage, "Area.devolverValorCampo", "Error returning the value of the field " + fieldName + ": " + ex.Message, ex);
            }
        }

        /// <summary>
        /// Returns the decrypted value of the field by name.
        /// </summary>
        /// <param name="fieldName">Field name</param>
        /// <returns>Field decrypted value</returns>
        public object ReturnDecryptedValueField(string fieldName)
        {
            try
            {
                var dbField = DBFields[fieldName.Split('.')[1]];

                if (fields[fieldName] == null)
                    return dbField.GetValorEmpty();
                else
                {
                    var encData = (EncryptedDataType)((RequestedField)fields[fieldName]).Value;
                    if(encData?.IsEmpty() ?? true)
                        return dbField.GetValorEmpty();

                    // TODO: Decrypt the value if necessary.

                    return encData.DecryptedValue;
                } 
            }
            catch (GenioException ex)
            {
                throw new BusinessException(ex.UserMessage, "Area.ReturnDecryptedValueField", "Error returning the field's value: " + ex.Message, ex);
            }
            catch (Exception ex)
            {
                throw new BusinessException(null, "Area.ReturnDecryptedValueField", "Error returning the field's value: " + ex.Message, ex);
            }
        }

        /// <summary>
        /// Função que permite remover um Qfield da área
        /// </summary>
        /// <param name="nomeCampo">name do Qfield a remover</param>
        /// <returns>True se foi removido false caso contrário</returns>
        public bool removeFieldValue(string fieldName)
        {
            if (fields.Contains(fieldName))
            {
                fields.Remove(fieldName);
                return true;
            }
            else
                return false;
        }

        /// <summary>
        /// Função que permite remover da área os fields que são doutras áreas
        /// </summary>
        public void removeFieldsOtherAreas()
        {
            IEnumerator camposPreenchidos = this.Fields.Values.GetEnumerator();
            String[] fieldsList = new string[fields.Count];
            int i = 0;
            while (camposPreenchidos.MoveNext())
            {
                RequestedField campoPedido = (RequestedField)camposPreenchidos.Current;

                if (!campoPedido.BelongsArea)
                {
                    fieldsList[i] = campoPedido.FullName;
                    i++;
                }
            };
            for (int j = 0; j < i; j++)
            {
                this.removeFieldValue(fieldsList[j]);
            }

        }

        /// <summary>
        /// Set decrypted value to encrypted field.
        /// TODO: In the future it will have to automatically include the type of encryption to use when writing the value to the database.
        /// </summary>
        /// <param name="fieldName">Field name</param>
        /// <param name="fieldValue">Decrypted value of the field</param>
        public virtual void InsertNameDecryptedValueField(string fieldName, object fieldValue)
        {
            throw new BusinessException(null, "Area.InsertNameDecryptedValueField", "Function not implemented.");
        }

        /// <summary>
        /// Function that allows removing Password-type fields from the area.
        /// </summary>
        /// <param name="removeOnlyEmpty">Remove only those taht have an empty value</param>
        public void RemovePasswordFields(bool removeOnlyEmpty = false)
        {
            foreach (var passwordField in this.PasswordFields ?? Enumerable.Empty<string>())
            {
                var fullFieldName = Alias + "." + passwordField;
                if (removeOnlyEmpty)
                {
                    var currentValue = this.returnValueField(fullFieldName);
                    var field = this.DBFields[passwordField];
                    var isEmpty = field.isEmptyValue(currentValue);

                    if(!isEmpty)
                        continue;
                }
                this.removeFieldValue(fullFieldName);
            }
        }

        /// <summary>
        /// Função que permite remover da área os fields que são calculados automaticamente
        /// Sempre que não confiamos nos Qvalues que estão a ser dados do exterior devemos
        ///  invocar este método depois de preencher os fields de uma área e antes de invocar
        ///  funções de recálculo.
        /// </summary>
        public void removeCalculatedFields()
        {
            string[] camposSR = this.RelatedSumFields;
            if (camposSR != null)
            {
                for (int i = 0; i < camposSR.Length; i++)
                {
                    this.removeFieldValue(Alias + "." + camposSR[i]);
                }
            }

            string[] camposU1 = this.LastValueFields;
            if (camposU1 != null)
            {
                for (int i = 0; i < camposU1.Length; i++)
                {
                    this.removeFieldValue(Alias + "." + camposU1[i]);
                }
            }

            // Fields with Concatenate rows formulas should not be overwritten by external inputs. 
            // This type of formula is propagated from bottom to top.
            if (AggregateListFields != null)
            {
                foreach (var field in AggregateListFields)
                {
                    removeFieldValue($"{Alias}.{field}");
                }
            }

            // MH (23/06/2017) - Apagar os fields do carimbo da ficha.
            // O metodo do Carimbar a ficha está e deve estar só depois deste metodo.
            if (this.StampFieldsIns != null)
            {
                foreach (string campoCarimbo in this.StampFieldsIns)
                    this.removeFieldValue(Alias + "." + campoCarimbo);
            }
            if (this.StampFieldsAlt != null)
            {
                foreach (string campoCarimbo in this.StampFieldsAlt)
                    this.removeFieldValue(Alias + "." + campoCarimbo);
            }

            // MH (23/06/2017) - Apagar os fields com formulas + e +H
            if (this.InternalOperationFields != null)
            {
                foreach (string Qfield in this.InternalOperationFields)
                    this.removeFieldValue(Alias + "." + Qfield);
            }

            // MH (23/06/2017) - Apagar os fields com formula do tipo Replica
            if (this.ReplicaFields != null)
            {
                foreach (string Qfield in this.ReplicaFields)
                    this.removeFieldValue(Alias + "." + Qfield);
            }

            // MH (23/06/2017) - Apagar os fields com formula do tipo Fim Periodo
            if (this.EndofPeriodFields != null)
            {
                foreach (string Qfield in this.EndofPeriodFields)
                    this.removeFieldValue(Alias + "." + Qfield);
            }
        }

        /// <summary>
        /// Função que permite obter os Qvalues da lista de fields
        /// </summary>
        ///
        public void getFields(string[] fieldsList, PersistentSupport sp)
        {
            if (fieldsList != null && fieldsList.Length!=0)
            {
                string[] listaValores = new string[fieldsList.Length];
                string[] listaCamposCompletos = new string[fieldsList.Length];

                SelectField[] listaCamposQuery = new SelectField[fieldsList.Length];
                for (int i = 0; i < fieldsList.Length; i++)
                    listaCamposQuery[i] = new SelectField(new ColumnReference(Alias, fieldsList[i]), fieldsList[i]);

                ArrayList Qvalues = sp.returnFields(this, listaCamposQuery, PrimaryKeyName, this.QPrimaryKey);

                for (int i = 0; i < fieldsList.Length; i++)
                {
                    if (Qvalues != null)
                    {
                            listaValores[i] = Conversion.internal2String(Qvalues[i],this.DBFields[fieldsList[i]].FieldType);
                            listaCamposCompletos[i] = Alias + "." + fieldsList[i];
                    }
                }
                this.addNamesValuesFields(listaCamposCompletos, listaValores);
            }
        }

        /// <summary>
        /// Método to devolver a formatação de um Qfield da Base de dados
        /// </summary>
        /// <param name="nomeCampoBD">name do Qfield</param>
        /// <returns>a formatação do Qfield se existir na BD, null caso contrário</returns>
        public FieldFormatting returnFormattingDBField(string dbFieldName)
        {
            return DBFields[dbFieldName].FieldFormat;
            //RS(2008.06.09) Os enumerados não são nullable, o Qfield devia existir sempre senão é barracada da aplicação e deve
            // ser corrigida (pelo que tem de crashar to se perceber de onde vem o erro).
            //if (DBFields.ContainsKey(dbFieldName))
            //    return ((Field)DBFields[dbFieldName]).FieldFormat;
            //else
            //    return null;
        }

        /****************************CARIMBAR REGISTOS***************************************/

        /// <summary>
        /// Método to preencher os fields relativos aos dados da pessoa que inseriu o registo e à altura em que o fez
        /// </summary>
        public void fillStampInsert()
        {
            try
            {
                if (this.StampFieldsIns != null && this.StampFieldsIns.Length != 0)
                {
                    string[] camposCarimbo = Information.StampFieldsIns;
                    for (int i = 0; i < camposCarimbo.Length; i++)
                    {
                        Field campoCarimbo = (Field)DBFields[camposCarimbo[i]];
                        DateTime dataHoje = DateTime.Now;
                        if (campoCarimbo.FieldType == FieldType.DATACRIA || campoCarimbo.FieldType == FieldType.INSTANTECRIA)//preenche o datacria se existir
                            insertNameValueField(this.Alias +"." +campoCarimbo.Name, dataHoje);
                        if (campoCarimbo.FieldType == FieldType.OPERCRIA)//preenche o opercria se existir
                            insertNameValueField(this.Alias +"." +campoCarimbo.Name, user.Name);
                        if (campoCarimbo.FieldType == FieldType.HORACRIA)//preenche o horacria se existir
                            insertNameValueField(this.Alias + "." + campoCarimbo.Name, string.Format("{0:00}:{1:00}", dataHoje.Hour, dataHoje.Minute));
                    }
                }
            }
            catch (GenioException ex)
            {
                throw new BusinessException(ex.UserMessage, "Area.preencherCarimboIns", "Error filling stamp fields for insertion: " + ex.Message, ex);
            }
            catch (Exception ex)
            {
                throw new BusinessException(null, "Area.preencherCarimboIns", "Error filling stamp fields for insertion: " + ex.Message, ex);
            }
        }

        /// <summary>
        /// Método to preencher os fields relativos aos dados da pessoa que alterou o registo e à altura em que o fez
        /// </summary>
        public void fillStampChange()
        {
            try
            {
                if (StampFieldsAlt != null && StampFieldsAlt.Length != 0)
                {
                    string[] camposCarimbo = StampFieldsAlt;
                    for (int i = 0; i < camposCarimbo.Length; i++)
                    {
                        Field campoCarimbo = (Field)DBFields[camposCarimbo[i]];
                        DateTime dataHoje = DateTime.Now;
                        if (campoCarimbo.FieldType == FieldType.DATAMUDA)//preenche o datamuda se existir
                            insertNameValueField(this.Alias +"." +campoCarimbo.Name, dataHoje);
                        if (campoCarimbo.FieldType == FieldType.OPERMUDA)//preenche o operChange se existir
                            insertNameValueField(this.Alias +"." +campoCarimbo.Name, user.Name);
                        if (campoCarimbo.FieldType == FieldType.HORAMUDA)//preenche o horamuda se existir
                            insertNameValueField(this.Alias + "." + campoCarimbo.Name, string.Format("{0}:{1}", dataHoje.Hour, dataHoje.Minute));
                    }
                }
            }
            catch (GenioException ex)
            {
                throw new BusinessException(ex.UserMessage, "Area.preencherCarimboAlt", "Error filling stamp fields for update: " + ex.Message, ex);
            }
            catch (Exception ex)
            {
                throw new BusinessException(null, "Area.preencherCarimboAlt", "Error filling stamp fields for update: " + ex.Message, ex);
            }
        }

        /// <summary>
        /// A partir de uma lista de historial filtra quais são os Qvalues aplicáveis a uma area.
        /// Tipicamente estes Qvalues vão ser usados to construir condições
        /// </summary>
        /// <param name="valoresEph">Um dicionario de condições a filtrar. A key é o name do eph, o Qvalue é um array de object.</param>
        /// <param name="identificador">Um identifier de override to a filtragem. Null caso não se queira considerar</param>
        /// <param name="unico">True caso o filtro só deva ser aplicado quando existir um e só um Qvalue to o eph</param>
        /// <returns>Uma lista de condicões em que cada uma é representada por um EPHOfArea</returns>
        /// <remarks>
        /// RS(2010.10.27) Refactorizei o codigo dos EPH em toda a frameword de forma a evitar duplicações
        /// Retirado de Listing (unico=false)
        /// *Bate certo com o da Area (unico=true, identifier=null)
        /// *Bate quase certo com o do Cart (unico=true, identifier=null) -> o unico é aplicado apenas a relations acima
        /// *Bate certo com o da Comunicacao (unico=false)
        /// *Bate certo com o do ReportCrystal (unico=false, identifier=null)
        /// </remarks>
        public List<EPHOfArea> CalculateAreaEphs(Hashtable ephValues, string identifier, bool unico)
        {
            List<EPHOfArea> res = new List<EPHOfArea>();

            //AV 20091229 Refiz condições to permitir EPH em árvore, com múltiplos Qvalues e
            //aplicadas a fields diferentes de chaves
            if (ephValues == null)
                return res;
            if (Ephs == null)
                return res;

            Hashtable condEph = new Hashtable(ephValues);
            // obtém as EPHs definidas to a área, module e nível de user

            //JGF 2020.09.09 Changed to get EPHs from a list of roles.
            var roleList = User.GetModuleRoles(module);
            List<EPHField> ephsArea = new List<EPHField>();
            foreach (var role in roleList)
            {
                var roleEphs = (EPHField[]) Ephs[new Par(Module, role.Id)];
                if(roleEphs != null)
                {
                    ephsArea.AddRange(roleEphs);
                }
            }

            foreach(var ephArea in ephsArea)
            {
                //considerar os identificadores nao sujeitos a EPH
                EPH eph = EPH.getEPH(module);//FFS (05-05-2011) a variável identifier vinha a null em alguns casos
                // RR 29-08-2012 - é preciso verificar também pela a EPH, faltava a última parte da condição
                if (eph.MenusNotSubjectEPH != null && identifier != null && eph.MenusNotSubjectEPH.ContainsKey(identifier) && eph.MenusNotSubjectEPH[identifier].Contains(ephArea.Name))
                    continue;

                //esta lista se exists eph nunca está vazia
                string[] listaValores = (string[])condEph[module + "_" + ephArea.Name];
                if (listaValores == null)
                    continue;

                //Se for unico entao a lista de Qvalues so pode ter um Qvalue
                if (unico && listaValores.Length != 1)
                {
                    condEph.Remove(module + "_" + ephArea);
                    continue;
                }

                EPHOfArea ephRes = new EPHOfArea(ephArea, listaValores);

                // Check if EPH is related to parent areas.
                // If found, these relations will be used to create query conditions.
                // Note: current area can be the same as the EPH area.
                // No need to add a relation if the current area is the EPH area itself.
                if (ParentTables != null)
                {
                    AreaInfo tabelaAtual = Area.GetInfoArea(Alias);

                    if (!String.IsNullOrEmpty(ephArea.Table) && Alias != ephArea.Table)
                    {
                        Relation myrelacao = null;
                        if (ParentTables.TryGetValue(ephArea.Table.ToLower(), out myrelacao))
                        {
                            ephRes.Relation = myrelacao;
                        }
                        else if (ephArea.Propagate)
                        {
                            List<Relation> relations = tabelaAtual.GetRelations(ephArea.Table);

                            if (relations != null && relations.Count > 0)
                                ephRes.Relation = relations.Last();
                        }
                    }

                    // Repeat the process if the EPH has a second condition (EPH2)
                    if (!String.IsNullOrEmpty(ephArea.Table2) && Alias != ephArea.Table2)
                    {
                        Relation myrelacao = null;

                        if (ParentTables.TryGetValue(ephArea.Table2.ToLower(), out myrelacao))
                        {
                            ephRes.Relation2 = myrelacao;
                        }
                        else if (ephArea.Propagate)
                        {
                            List<Relation> relations = tabelaAtual.GetRelations(ephArea.Table2);

                            if (relations != null && relations.Count > 0)
                                ephRes.Relation2 = relations.Last();
                        }
                    }
                }
                res.Add(ephRes);

                condEph.Remove(module + "_" + ephArea);
                if (condEph.Count == 0)
                    break;
            }

            return res;
        }

        /// <summary>
        /// Checks if the current user has access rights to query a record in this area.
        /// </summary>
        /// <returns></returns>
        public bool AccessRightsToConsult()
        {
            return AccessRightsToConsult(User);
        }

        /// <summary>
        /// Checks if a user has access rights to query a record in this area.
        /// </summary>
        /// <returns></returns>
        public bool AccessRightsToConsult(User user)
        {
            return user.GetModuleRoles(module).Any(role => QLevel.CanConsult(role));
        }


        /// <summary>
        /// Testa se o user tem direitos de Acesso to apagar
        /// </summary>
        /// <returns>true se o user pode apagar, false caso contrário</returns>
        public bool AccessRightsToDelete(User user)
        {
            return user.GetModuleRoles(module).Any(role => QLevel.CanDelete(role));
        }

        /// <summary>
        /// Função que verifica se um registo pode ser alterado
        /// </summary>
        /// <param name="sp">Suporte Persistente</param>
        /// <returns>true se pode change, false caso contrário</returns>
        public bool AccessRightsToChange(User user)
        {
            return user.GetModuleRoles(module).Any(role => QLevel.CanChange(role));
        }

        /// <summary>
        /// Checks if a user has access rights to create a record in this area.
        /// </summary>
        /// <returns></returns>
        public bool AccessRightsToCreate(User user)
        {
            return user.GetModuleRoles(module).Any(role => QLevel.CanCreate(role));
        }

        /// <summary>
        /// função que preenche as eph quando exists um único Qvalue
        /// </summary>
        /// <param name="User">user em sessão</param>
        public void fillEPH(User user, PersistentSupport sp, string identifier)
        {
            List<EPHOfArea> ephsDaArea = CalculateAreaEphs(user.Ephs, identifier, true);
            foreach (EPHOfArea v in ephsDaArea)
            {
                if (v.Relation == null && v.Relation2 == null) {
                    AuxAdicionaCondicaoMesmaArea(v.Eph, v.ValuesList);
                    continue;
                }

                // Add field from EPH first condition
                if (v.Relation != null)
                {
                    AuxAdicionaCondicaoOutraArea(sp, v.Eph, v.ValuesList, v.Relation);
                }
                // Add field from EPH second condition
                // Caveat: EPHs with multiple conditions will only add one field to the requested fields
                else if (v.Relation2 != null)
                {
                    // In order to reuse code, we create a second EPH field from data in area's EPH
                    EPHField EPH2 = new EPHField(v.Eph.Name, v.Eph.Table2, v.Eph.Field2, v.Eph.Operator2, v.Eph.Propagate);
                    AuxAdicionaCondicaoOutraArea(sp, EPH2, v.ValuesList, v.Relation2);
                }
            }
        }

        private RequestedField AuxAdicionaCondicaoOutraArea(PersistentSupport sp, EPHField ephArea, string[] listaValores, Relation myrelacao)
        {
            AreaInfo tabelaEPH = Area.GetInfoArea(ephArea.Table);
            RequestedField campoPedido;
            string crorigem = myrelacao.SourceRelField;
            string crorigem_full = Alias + "." + crorigem;

            if (ephArea.Propagate)
            {
                crorigem = myrelacao.TargetRelField;
                crorigem_full = tabelaEPH.Alias + "." + crorigem;
            }

            campoPedido = new RequestedField(crorigem_full, Alias);
            Field QPrimaryKeyField = (Field)tabelaEPH.DBFields[tabelaEPH.PrimaryKeyName];
            campoPedido.FieldType = QPrimaryKeyField.FieldType;

            object Qvalue = null;

            // If the EPH is set on a primary-key column, we can directly use the supplied value
            // Otherwise, we retrieve the primary-key value of the record that validates the EPH condition
            if (ephArea.Field.ToLower().Equals(myrelacao.TargetRelField))
            {
                Qvalue = listaValores[0];
            }
            else
            {
                CriteriaSet where = CriteriaSet.And();

                switch (ephArea.Operator)
                {
                    case "=" :
                        where.Equal(tabelaEPH.TableName, ephArea.Field, listaValores[0]);
                        break;
                    case "<" :
                        where.Lesser(tabelaEPH.TableName, ephArea.Field, listaValores[0]);
                        break;
                    case ">" :
                        where.Greater(tabelaEPH.TableName, ephArea.Field, listaValores[0]);
                        break;
                    case "<=" :
                        where.LesserOrEqual(tabelaEPH.TableName, ephArea.Field, listaValores[0]);
                        break;
                    case ">=" :
                        where.GreaterOrEqual(tabelaEPH.TableName, ephArea.Field, listaValores[0]);
                        break;
                    case "!=" :
                        where.NotEqual(tabelaEPH.TableName, ephArea.Field, listaValores[0]);
                        break;
                    case "L" :
                        where.Like(tabelaEPH.TableName, ephArea.Field, listaValores[0] + "%"); // TODO: Use LEFT. BackOffice: (LEFT(%s,%d)=
                        break;
                    case "LN" :
                        // MH - Eph em árvore ou NULL
                        CriteriaSet auxWhere = CriteriaSet.Or();
                        Field campoLN = tabelaEPH.DBFields[ephArea.Field];
                        if (campoLN.isKey())
                        {
                            auxWhere.Equal(new ColumnReference(tabelaEPH.TableName, ephArea.Field), null);
                        }
                        else
                        {
                            FieldFormatting cFormat = campoLN.FieldFormat;
                            string funcaoSQL = FieldType.getEPHFunction(cFormat);
                            auxWhere.Equal(SqlFunctions.Custom(funcaoSQL, new ColumnReference(tabelaEPH.TableName, ephArea.Field)), 1);
                        }
                        auxWhere.Like(tabelaEPH.TableName, ephArea.Field, listaValores[0] + "%"); // TODO: Use LEFT. BackOffice: (LEFT(%s,%d)=
                        where.SubSet(auxWhere);
                        break;
                    case "NULL" :
                        where.Equal(tabelaEPH.TableName, ephArea.Field, null);
                        break;
                    case "EN":
						Field Qfield = (Field)tabelaEPH.DBFields[ephArea.Field];
                        CriteriaSet lim = new CriteriaSet(CriteriaSetOperator.Or);
                        if(Qfield.isKey())
                        {
                            lim.Equal(new ColumnReference(tabelaEPH.TableName, ephArea.Field), null);
                        }
                        else
                        {
                            FieldFormatting cFormat = Qfield.FieldFormat;
                            string funcaoSQL = FieldType.getEPHFunction(cFormat);
                            lim.Equal(SqlFunctions.Custom(funcaoSQL, new ColumnReference(tabelaEPH.TableName, ephArea.Field)), 1);
                        }
                        lim.Equal(tabelaEPH.TableName, ephArea.Field, listaValores[0]);
                        where.SubSet(lim);
                        break;
                    default:
                        throw new BusinessException(null, "Area.AuxAdicionaCondicaoOutraArea", "The eph operator '" + ephArea.Operator + "' is not known.");
                }

                SelectQuery query = new SelectQuery()
                    .Select(tabelaEPH.TableName, tabelaEPH.PrimaryKeyName)
                    .From(tabelaEPH.QSystem, tabelaEPH.TableName, tabelaEPH.TableName)
                    .Where(where);

                var result = sp.Execute(query);
                if (result.NumRows == 1)
                    Qvalue = DBConversion.ToInternal(result.GetDirect(0, 0), QPrimaryKeyField.FieldFormat);
            }

            if (QPrimaryKeyField.isEmptyValue(Qvalue))
                Qvalue = QPrimaryKeyField.GetValorEmpty();

            campoPedido.Value = Qvalue;

            // Do not add a repeated field to the query
            // The request already includes the requested field needed for the EPH
            if (fields.ContainsKey(Alias + "." + crorigem))
                return campoPedido;

            if (!fields.ContainsKey(crorigem_full))
                fields.Add(crorigem_full, campoPedido);
            else
                fields[crorigem_full] = campoPedido;
            return campoPedido;
        }


        private RequestedField AuxAdicionaCondicaoMesmaArea(EPHField ephArea, string[] listaValores)
        {
            RequestedField campoPedido;
            campoPedido = new RequestedField(Alias + "." + ephArea.Field, Alias);
            Field Qfield = (Field)DBFields[campoPedido.Name];
            campoPedido.FieldType = Qfield.FieldType;
            campoPedido.Value = listaValores[0];

            if (!fields.ContainsKey(Alias + "." + ephArea.Field))
                fields.Add(Alias + "." + ephArea.Field, campoPedido);
            else
                fields[Alias + "." + ephArea.Field] = campoPedido;
            return campoPedido;
        }

        public int Zzstate
        {
            get { return (int)returnValueField(Alias + "." + "zzstate"); }
            set { insertNameValueField(Alias + "." + "zzstate", value); }
        }

        public string QPrimaryKey
        {
            get { return (string)returnValueField(Alias + "." + PrimaryKeyName); }
            set { insertNameValueField(Alias + "." + PrimaryKeyName, value); }
        }

        /// <summary>
        /// Selecciona o Qresult da query quando é único
        /// </summary>
        /// <param name="condicao">Condição WHERE</param>
        /// <param name="identificador">Identificado do controlo</param>
        /// <param name="sp">Suporte persistente</param>
        /// <returns></returns>
        public void selectSingle(CriteriaSet condition, string identifier, PersistentSupport sp)
        {
            try
            {
                sp.selectSingle(condition, this, identifier);
            }
            catch (GenioException ex)
            {
                throw new BusinessException(ex.UserMessage, "Area.seleccionarUnico", "Error selecting a single query result: " + ex.Message, ex);
            }
        }

        [Obsolete("Use void seleccionarUm(CriteriaSet condicao, IList<ColumnSort> ordenacao, string identificador) instead")]
        public void selectOne(string condition, string sorting, string identifier)
        {
            try
            {
                PersistentSupport sp = PersistentSupport.getPersistentSupport(User.Year, User.Name);
                sp.selectOne(condition, sorting, this, identifier);
            }
            catch (GenioException ex)
            {
                throw new BusinessException(ex.UserMessage, "Area.seleccionarUm", "Error selecting first query result: " + ex.Message, ex);
            }
        }

        [Obsolete("Use void seleccionarUm(CriteriaSet condicao, IList<ColumnSort> ordenacao, string identificador, PersistentSupport sp) instead")]
        public void selectOne(CriteriaSet condition, IList<ColumnSort> sorting, string identifier)
        {
            try
            {
                PersistentSupport sp = PersistentSupport.getPersistentSupport(User.Year);
                sp.selectOne(condition, sorting, this, identifier);
            }
            catch (GenioException ex)
            {
                throw new BusinessException(ex.UserMessage, "Area.seleccionarUm", "Error selecting first query result: " + ex.Message, ex);
            }
        }

        /// <summary>
        /// Selecciona o primeiro Qresult da query
        /// </summary>
        /// <param name="condicao">Condição WHERE</param>
        /// <param name="ordenacao">Ordenação do Qresult</param>
        /// <param name="identificador">Identificado do controlo</param>
        /// <returns></returns>
        [Obsolete("Use void seleccionarUm(CriteriaSet condicao, IList<ColumnSort> ordenacao, string identificador, PersistentSupport sp) instead")]
        public void selectOne(string condition, string sorting, string identifier, PersistentSupport sp)
        {
            try
            {
                sp.selectOne(condition, sorting, this, identifier);
            }
            catch (GenioException ex)
            {
                throw new BusinessException(ex.UserMessage, "Area.seleccionarUm", "Error selecting first query result: " + ex.Message, ex);
            }
        }

        public void selectOne(CriteriaSet condition, IList<ColumnSort> sorting, string identifier, PersistentSupport sp, int pageSize = 1)
        {
            try
            {
                Type funcObj = typeof(GenioServer.framework.OverrideQuery);
                MethodInfo funcOver = funcObj.GetMethod(identifier);
                if (funcOver != null)
                {
                    //void (CriteriaSet condition, string tokenAux, PersistentSupport sp, IList<ColumnSort> sorting)

                    object[] parameters = new object[5];
                    parameters[0] = condition;
                    parameters[1] = user;
                    parameters[2] = sp;
                    parameters[3] = sorting;
                    parameters[4] = this;

                    funcOver.Invoke(this, parameters); //TODO : os paramentros adicionais e tratamentos dos mesmo
                }
                else
                {
                    if (Information.PersistenceType == PersistenceType.Database || Information.PersistenceType == PersistenceType.View)
                    {
                        sp.selectOne(condition, sorting, this, identifier, pageSize);
                    }
                    else
                    {
                        string[] fieldsRequested = new string[Fields.Keys.Count];
                        Fields.Keys.CopyTo(fieldsRequested, 0);

                        Type areaType = this.GetType();
                        MethodInfo listMethod = areaType.GetMethod("search",
                            BindingFlags.Static | BindingFlags.Public,
                            null,
                            new Type[] { typeof(PersistentSupport), typeof(string), typeof(User), typeof(string[]) },
                            null
                        );
                        IArea res = listMethod.Invoke(null, new object[] {
                            sp,
                            condition.FindCriteria(this.Alias, this.PrimaryKeyName, CriteriaOperator.Equal, CriteriaSet.FindVariable.Any).RightTerm as string,
                            user,
                            fieldsRequested
                        }) as IArea;
                        CloneFrom(res);
                    }
                }
            }
            catch (GenioException ex)
            {
                throw new BusinessException(ex.UserMessage, "Area.seleccionarUm", "Error selecting first query result: " + ex.Message, ex);
            }
            catch (TargetInvocationException ex)
            {
                throw new BusinessException(null, "Area.seleccionarUm", "Error selecting first query result: " + ex.Message, ex);
                /*if (ex.InnerException is FrameworkException)
                    throw (FrameworkException)ex.InnerException;
                else if (ex.InnerException is BusinessException)
                    throw (BusinessException)ex.InnerException;
                else if (ex.InnerException is PersistenceException)
                    throw (PersistenceException)ex.InnerException;
                else
                    throw ex.InnerException;*/
            }
        }

        /// <summary>
        /// Search for all records of this area that comply with a condition
        /// </summary>
        /// <param name="area">The area.</param>
        /// <param name="sp">The sp.</param>
        /// <param name="user">The user.</param>
        /// <param name="where">The where.</param>
        /// <param name="fields">The fields.</param>
        /// <param name="distinct">if set to <c>true</c> [distinct].</param>
        /// <param name="noLock">if set to <c>true</c> [no lock].</param>
        /// <returns></returns>
        public static List<Area> searchList(string area, PersistentSupport sp, User user, CriteriaSet where, string[] fields = null, bool distinct = false, bool noLock = false)
        {
            // Find the generic method in Persistent Support (searchListWhere<T>)
            var mInfo = typeof(PersistentSupport).GetMethod("genericSearchListWhere",
                BindingFlags.Public | BindingFlags.Instance,
                null,
                CallingConventions.Any,
                new Type[] { typeof(CriteriaSet), typeof(User), typeof(string[]), typeof(bool), typeof(bool) },
                null);

            // Apply concrete type to method type parameter (searchListWhere<CSGenioA_____>)
            Type type = GetTypeArea(area);
            MethodInfo generic = mInfo.MakeGenericMethod(type);
            
            // Invoke
            object[] args = { where, user, fields, distinct, noLock };
            return ((List<Area>)generic.Invoke(sp, args));
        }

        /// <summary>
        /// Delete a record from the database. Requires an opened connection
        /// </summary>
        /// <param name="sp">PersistentSupport</param>
        /// <returns></returns>
        public virtual StatusMessage eliminate(PersistentSupport sp)
        {
            try
            {
                if (Log.IsDebugEnabled) Log.Debug(string.Format("Area.eliminar [area] {0}", Alias));
                object valorCodigoObj = QPrimaryKey;
                if (valorCodigoObj == null)
                    throw new BusinessException(null, "Area.eliminar", "ChavePrimaria is null.");

                sp.deleteRecord(this, valorCodigoObj.ToString());
                return StatusMessage.OK("Eliminação bem sucedida.");
            }
            catch (GenioException ex)
            {
                if (ex.ExceptionSite == "Area.eliminar")
                    throw;
                throw new BusinessException(ex.UserMessage, "Area.eliminar", "Error deleting record from Area: " + ex.Message, ex);
            }
            catch (Exception ex)
            {
                throw new BusinessException(null, "Area.eliminar", "Error deleting record from Area: " + ex.Message, ex);
            }

        }


        /// <summary>
        /// Eliminate a record due to the deletion of another record. The root record must be known for various checks
        /// </summary>
        /// <param name="rootRecord">Root record which originated this deletion</param>
        /// <returns></returns>
        public virtual StatusMessage eliminateDependent(PersistentSupport sp, Area rootRecord)
        {
            return eliminate(sp);
        }

        public virtual StatusMessage change(PersistentSupport sp, CriteriaSet condition)
        {
            try
            {
                if (Log.IsDebugEnabled) Log.Debug(string.Format("Area.alterar [area] {0}", Alias));
                sp.change(this);
            }
            catch (GenioException ex)
            {
                throw new BusinessException(ex.UserMessage, "Area.alterar", "Error changing record from Area: " + ex.Message, ex);
            }
            catch (Exception ex)
            {
                throw new BusinessException(null, "Area.alterar", "Error changing record from Area: " + ex.Message, ex);
            }

            return StatusMessage.OK("Alteração bem sucedida.");
        }

        public virtual void apply(PersistentSupport sp, bool isGoingBack = false)
        {
            try
            {
                if (Log.IsDebugEnabled) Log.Debug(string.Format("Area.apply [area] {0}", Alias));
                sp.change(this);
            }
            catch (GenioException ex)
            {
                throw new BusinessException(ex.UserMessage, "Area.apply", "Error changing record from Area: " + ex.Message, ex);
            }
            catch (Exception ex)
            {
                throw new BusinessException(null, "Area.apply", "Error changing record from Area: " + ex.Message, ex);
            }
        }

        /// <summary>
        /// Método to introduce um registo que fica imediatamente disponivel (zztate=0)
        /// </summary>
        /// <param name="modulo">módulo</param>
        /// <returns>o status e a mensagem resposta da inserção</returns>
        public virtual StatusMessage inserir_WS(PersistentSupport sp)
        {
            try
            {
                insertPseud(sp);

                //Aqui queremos sempre garantir que o zzstate passa a 0
                Zzstate = 0;

                return change(sp, (CriteriaSet)null);
            }
            catch (GenioException ex)
            {
                throw new BusinessException(ex.UserMessage, "Area.inserir_WS", "Error inserting record in Area: " + ex.Message, ex);
            }
        }

        /// <summary>
        /// Método que permite introduce um registo na base de dados,
        /// pressupoe uma ligação aberta
        /// </summary>
        /// <param name="sp">Suporte Persistente</param>
        /// <returns></returns>
        public virtual Area insertPseud(PersistentSupport sp)
        {
            return insertPseud(sp,  new string[] { }, new string[] { });
        }

        /// <summary>
        /// Método que permite introduce um registo na base de dados como uma ficha pseudo-nova (zzstate = 1)
        /// </summary>
        /// <param name="sp">Suporte Persistente</param>
        /// <param name="condicao">Condição de seleção dos registos</param>
        /// <returns></returns>
        public virtual Area insertPseud(PersistentSupport sp, string[] fieldNames, string[] fieldsvalues)
        {
            try
            {
                if (Log.IsDebugEnabled) Log.Debug(string.Format("Area.inserir [area] {0}", Alias));

                sp.insertPseud(this);
            }
            catch (GenioException ex)
            {
                throw new BusinessException(ex.UserMessage, "Area.inserir", "Error inserting record in Area: " + ex.Message, ex);
            }
            catch (Exception ex)
            {
                throw new BusinessException(null, "Area.inserir", "Error inserting record in Area: " + ex.Message, ex);
            }

            return this;
        }

        public virtual Area duplicate(PersistentSupport sp, CriteriaSet condition)
        {
            throw new BusinessException(null, "Area.duplicar", "Function not implemented.");
        }

        public virtual StatusMessage beforeUpdate(PersistentSupport sp, Area oldvalues)
        {
            return StatusMessage.OK();
        }

        public virtual StatusMessage afterUpdate(PersistentSupport sp, Area oldvalues)
        {
            return StatusMessage.OK();
        }

        public virtual StatusMessage beforeInsert(PersistentSupport sp)
        {
            return StatusMessage.OK();
        }

        public virtual StatusMessage afterInsert(PersistentSupport sp)
        {
            return StatusMessage.OK();
        }

        public virtual StatusMessage beforeEliminate(PersistentSupport sp)
        {
            return StatusMessage.OK();
        }

        public virtual StatusMessage afterEliminate(PersistentSupport sp)
        {
            return StatusMessage.OK();
        }

        public virtual StatusMessage beforeDuplicate(PersistentSupport sp)
        {
            return StatusMessage.OK();
        }

        public virtual StatusMessage afterDuplicate(PersistentSupport sp)
        {
            return StatusMessage.OK();
        }

        /// <summary>
        /// Gets the primary key and all the requested fields of a given related area.
        /// Returns an area filled with these informations.
        /// </summary>
        /// <param name="relatedAreaName">The related area</param>
        /// <param name="requestedFields">The requested fields</param>
        /// <returns>Area field with the requested fields and primary key</returns>
        public virtual Area fillRelatedArea(string relatedAreaName, string[] requestedFields) {
            Area relatedArea = Area.createArea(relatedAreaName, user, Module);
            // adds the restriction, that will limit the upper areas
            CriteriaSet cs = CriteriaSet.And();
            cs.Equal(new FieldRef(Alias,PrimaryKeyName), QPrimaryKey);

            List<string> fieldsList = new List<string>(requestedFields);
            string pkFullFieldName = relatedArea.Alias + "." + relatedArea.PrimaryKeyName;
            if(!fieldsList.Contains(pkFullFieldName))
                fieldsList.Add(pkFullFieldName);

            PersistentSupport sp = PersistentSupport.getPersistentSupport(user.Year);
            sp.fillInfoForForeignKey(relatedArea, this, cs, fieldsList);

            return relatedArea;
        }

        public Hashtable Fields
        {
            get { return fields; }
            set { fields = value; }
        }

        public abstract AreaInfo Information
        {
            get;
        }

        public string QSystem
        {
            get { return Information.QSystem; }
        }

        public string Alias
        {
            get { return Information.Alias; }
        }

        public Dictionary<string, Relation> ParentTables
        {
            get { return Information.ParentTables; }
        }

        public Relation[] DuplicationRelations
        {
            get { return Information.DuplicationRelations; }
        }

        public string PrimaryKeyName
        {
            get { return Information.PrimaryKeyName; }
        }

        public string ShadowTabKeyName
        {
            get { return Information.ShadowTabKeyName; }
        }

        public string TableName
        {
            get { return Information.TableName; }
        }

        public string ShadowTabName
        {
            get { return Information.ShadowTabName; }
        }

        public Dictionary<string, Field> DBFields
        {
            get { return Information.DBFields; }
        }

        public ChildRelation[] ChildTable
        {
            get { return Information.ChildTable; }
        }

        public Hashtable Ephs
        {
            get { return Information.Ephs; }
        }

        public ArrayList ShadowTabLevels
        {
            get { return Information.ShadowTabLevels; }
        }

        public string[] DefaultValues
        {
            get { return Information.DefaultValues; }
        }

        public string[] SequentialDefaultValues
        {
            get { return Information.SequentialDefaultValues; }
        }

        public string[] ReplicaFields
        {
            get { return Information.ReplicaFields; }
        }

        public string[] CheckTableFields
        {
            get { return Information.CheckTableFields; }
        }

        //SO 20060616
        public string[] EndofPeriodFields
        {
            get { return Information.EndofPeriodFields; }
        }

        public string[] InternalOperationFields
        {
            get { return Information.InternalOperationFields; }
        }

        public string[] InternalOperationSequentialFields
        {
            get { return Information.InternalOperationSequentialFields; }
        }

        public string[] RelatedSumFields
        {
            get { return Information.RelatedSumFields; }
        }

        public List<RelatedSumArgument> RelatedSumArgs
        {
            get { return Information.RelatedSumArgs; }
        }

        //AJAGENIO
        public string[] AggregateListFields
        {
            get { return Information.AggregateListFields; }
        }
        public List<ListAggregateArgument> ArgsListAggregate
        {
            get { return Information.ArgsListAggregate; }
        }

        public List<History> HistoryList
        {
            get { return Information.HistoryList; }
        }
        //AV 20090206
        public string[] LastValueFields
        {
            get { return Information.LastValueFields; }
        }

        public string[] StampFieldsIns
        {
            get { return Information.StampFieldsIns; }
        }

        public string[] StampFieldsAlt
        {
            get { return Information.StampFieldsAlt; }
        }

        //SO 20060810
        public List<LastValueArgument> LastValueArgs
        {
            get { return Information.LastValueArgs; }
        }

        //SO 20060818 propriedade to retirar no QLevel
        public QLevel QLevel
        {
            get { return Information.QLevel; }
        }

        public string AreaPluralDesignation
        {
            get { return Information.AreaPluralDesignation; }
        }

        public string AreaDesignation
        {
            get { return Information.AreaDesignation; }
        }

        public User User
        {
            get { return user; }
        }
        public string Module
        {
            get { return module; }
        }

        /// <summary>
        /// Write conditions for this area
        /// </summary>
        private List<ConditionFormula> WriteConditions
        {
            get => Information.WriteConditions;
        }

        /// <summary>
        /// CRUD conditions for this area
        /// </summary>
        private List<ConditionFormula> CrudConditions
        {
            get => Information.CrudConditions;
        }

        public string[] PasswordFields
        {
            get => Information.PasswordFields;
        }


        /// <summary>
        /// Validate all area level conditions
        /// </summary>
        /// <param name="sp">The persistent support to get values from</param>
        /// <param name="isApply">If this is an apply operation. Some conditions can execute in apply and other not</param>
        /// <returns>A status message with the aggregated result of all conditions evaluation</returns>
        public StatusMessage ValidateConditions(PersistentSupport sp, bool isApply)
        {
            StatusMessage result = StatusMessage.OK();
            //Area conditions
            foreach (ConditionFormula condition in WriteConditions)
            {
                if (isApply && !condition.Validate)
                    continue;

                bool conditionResult = condition.ExecuteCondition(this, sp, FunctionType.ALT);
                StatusMessage status = StatusMessage.OK();
                if (!conditionResult)
                {
                    if (condition.Type == ConditionType.ERROR)
                    {
                        status = StatusMessage.Error(Translations.Get(condition.ErrorWarning, user.Language));
                    }
                    else if (condition.Type == ConditionType.WARNING)
                    {
                        status = StatusMessage.Warning(Translations.Get(condition.ErrorWarning, user.Language));
                    }
                    else if (condition.Type == ConditionType.SAVE) {
                        status = StatusMessage.OK(Translations.Get(condition.ErrorWarning, user.Language));
                        result.MergeStatusMessage(status); //If this is the right condition result for SAVE, merge the message
                    }
                }
                else if (condition.Type == ConditionType.MANDATORY)
                {
                    var fieldName = condition.Field.Alias + "." + condition.Field.Name;
                    var value = ((RequestedField)Fields[fieldName]).Value;
                    if (condition.Field.isEmptyValue(value))
                    {
                        status = StatusMessage.Error(Translations.Get(condition.ErrorWarning, user.Language));
                    }
                }  

                if(status.Status != Status.OK)
                    result.MergeStatusMessage(status);
            }
            return result;
        }
    }
}
