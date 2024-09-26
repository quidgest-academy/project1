

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Text;
using CSGenio.framework;
using CSGenio.persistence;
using Quidgest.Persistence;
using Quidgest.Persistence.GenericQuery;
using System.Linq;

namespace CSGenio.business
{
	/// <summary>
	/// Agente imobiliário
	/// </summary>
	public class CSGenioAagent : DbArea	{
		/// <summary>
		/// Meta-information on this area
		/// </summary>
		protected readonly static AreaInfo informacao = InicializaAreaInfo();

		public CSGenioAagent(User user, string module)
		{
			fields = new Hashtable();
            this.user = user;
            this.module = module;
			this.KeyType = CodeType.INT_KEY;
			// USE /[MANUAL PRO CONSTRUTOR AGENT]/
		}

		public CSGenioAagent(User user) : this(user, user.CurrentModule)
		{
		}

		/// <summary>
		/// Initializes the metadata relative to the fields of this area
		/// </summary>
		private static void InicializaCampos(AreaInfo info)
		{
			Field Qfield = null;
#pragma warning disable CS0168, S1481 // Variable is declared but never used
			List<ByAreaArguments> argumentsListByArea;
#pragma warning restore CS0168, S1481 // Variable is declared but never used
			//- - - - - - - - - - - - - - - - - - -
			Qfield = new Field("codagent", FieldType.CHAVE_PRIMARIA);
			Qfield.FieldDescription = "";
			Qfield.FieldSize =  8;
			Qfield.Alias = info.Alias;
			Qfield.MQueue = false;
			Qfield.CavDesignation = "";

			Qfield.Dupmsg = "";
			info.RegisterFieldDB(Qfield);

			//- - - - - - - - - - - - - - - - - - -
			Qfield = new Field("foto", FieldType.IMAGEM_JPEG);
			Qfield.FieldDescription = "Fotografia";
			Qfield.FieldSize =  3;
			Qfield.Alias = info.Alias;
			Qfield.MQueue = false;
			Qfield.CavDesignation = "FOTOGRAFIA36807";

			Qfield.Dupmsg = "";
			info.RegisterFieldDB(Qfield);

			//- - - - - - - - - - - - - - - - - - -
			Qfield = new Field("nome", FieldType.TEXTO);
			Qfield.FieldDescription = "Nome";
			Qfield.FieldSize =  80;
			Qfield.Alias = info.Alias;
			Qfield.MQueue = false;
			Qfield.CavDesignation = "NOME47814";

            Qfield.NotNull = true;
			Qfield.Dupmsg = "";
			info.RegisterFieldDB(Qfield);

			//- - - - - - - - - - - - - - - - - - -
			Qfield = new Field("dnascime", FieldType.DATA);
			Qfield.FieldDescription = "Data de nascimento";
			Qfield.FieldSize =  8;
			Qfield.Alias = info.Alias;
			Qfield.MQueue = false;
			Qfield.CavDesignation = "DATA_DE_NASCIMENTO13938";

			Qfield.Dupmsg = "";
			info.RegisterFieldDB(Qfield);

			//- - - - - - - - - - - - - - - - - - -
			Qfield = new Field("email", FieldType.TEXTO);
			Qfield.FieldDescription = "E-mail";
			Qfield.FieldSize =  80;
			Qfield.Alias = info.Alias;
			Qfield.MQueue = false;
			Qfield.CavDesignation = "E_MAIL42251";

            Qfield.NotNull = true;
			Qfield.Dupmsg = "";
            Qfield.NotDup = true;
			info.RegisterFieldDB(Qfield);

			//- - - - - - - - - - - - - - - - - - -
			Qfield = new Field("telefone", FieldType.TEXTO);
			Qfield.FieldDescription = "Telefone";
			Qfield.FieldSize =  14;
			Qfield.Alias = info.Alias;
			Qfield.MQueue = false;
			Qfield.CavDesignation = "TELEFONE37757";

			Qfield.Dupmsg = "";
			Qfield.FillingRule = (rule) =>
			{
				string mask = "+000 000000000";
				string validation = "+000 000000000";
				return Validation.validateMP(rule, mask, validation);
			};
			info.RegisterFieldDB(Qfield);

			//- - - - - - - - - - - - - - - - - - -
			Qfield = new Field("codpmora", FieldType.CHAVE_ESTRANGEIRA);
			Qfield.FieldDescription = "";
			Qfield.FieldSize =  8;
			Qfield.Alias = info.Alias;
			Qfield.MQueue = false;
			Qfield.CavDesignation = "";

			Qfield.Dupmsg = "";
			info.RegisterFieldDB(Qfield);

			//- - - - - - - - - - - - - - - - - - -
			Qfield = new Field("codpnasc", FieldType.CHAVE_ESTRANGEIRA);
			Qfield.FieldDescription = "";
			Qfield.FieldSize =  8;
			Qfield.Alias = info.Alias;
			Qfield.MQueue = false;
			Qfield.CavDesignation = "";

			Qfield.Dupmsg = "";
			info.RegisterFieldDB(Qfield);

			//- - - - - - - - - - - - - - - - - - -
			Qfield = new Field("perelucr", FieldType.NUMERO);
			Qfield.FieldDescription = "% lucro";
			Qfield.FieldSize =  4;
			Qfield.Alias = info.Alias;
			Qfield.MQueue = false;
			Qfield.Decimals = 2;
			Qfield.CavDesignation = "__LUCRO26851";

			Qfield.Dupmsg = "";
			info.RegisterFieldDB(Qfield);

			//- - - - - - - - - - - - - - - - - - -
			Qfield = new Field("lucro", FieldType.VALOR);
			Qfield.FieldDescription = "Lucro";
			Qfield.FieldSize =  10;
			Qfield.Alias = info.Alias;
			Qfield.MQueue = false;
			Qfield.Decimals = 4;
			Qfield.CavDesignation = "LUCRO53291";

			Qfield.Dupmsg = "";
			info.RegisterFieldDB(Qfield);

			//- - - - - - - - - - - - - - - - - - -
			Qfield = new Field("zzstate", FieldType.INTEIRO);
			Qfield.FieldDescription = "Estado da ficha";
			Qfield.Alias = info.Alias;
			info.RegisterFieldDB(Qfield);

		}

		/// <summary>
		/// Initializes metadata for paths direct to other areas
		/// </summary>
		private static void InicializaRelacoes(AreaInfo info)
		{
			// Daughters Relations
			//------------------------------
			info.ChildTable = new ChildRelation[1];
			info.ChildTable[0]= new ChildRelation("propr", new String[] {"codagent"}, DeleteProc.NA);

			// Mother Relations
			//------------------------------
			info.ParentTables = new Dictionary<string, Relation>();
			info.ParentTables.Add("pmora", new Relation("PRO", "proagente_imobiliario", "agent", "codagent", "codpmora", "PRO", "propais", "pmora", "codpais", "codpais"));
			info.ParentTables.Add("pnasc", new Relation("PRO", "proagente_imobiliario", "agent", "codagent", "codpnasc", "PRO", "propais", "pnasc", "codpais", "codpais"));
		}

		/// <summary>
		/// Initializes metadata for indirect paths to other areas
		/// </summary>
		private static void InicializaCaminhos(AreaInfo info)
		{
			// Pathways
			//------------------------------
			info.Pathways = new Dictionary<string, string>(2);
			info.Pathways.Add("pmora","pmora");
			info.Pathways.Add("pnasc","pnasc");
		}

		/// <summary>
		/// Initializes metadata for triggers and formula arguments
		/// </summary>
		private static void InicializaFormulas(AreaInfo info)
		{
			// Formulas
			//------------------------------




			info.RelatedSumFields = new string[] {
			 "lucro"
			};





			//Write conditions
			List<ConditionFormula> conditions = new List<ConditionFormula>();
			info.WriteConditions = conditions.Where(c=> c.IsWriteCondition()).ToList();
			info.CrudConditions = conditions.Where(c=> c.IsCrudCondition()).ToList();

		}

		/// <summary>
		/// static CSGenioAagent()
		/// </summary>
		private static AreaInfo InicializaAreaInfo()
		{
			AreaInfo info = new AreaInfo();

			// Area meta-information
			info.QSystem="PRO";
			info.TableName="proagente_imobiliario";
			info.ShadowTabName="";
			info.ShadowTabKeyName="";

			info.PrimaryKeyName="codagent";
			info.HumanKeyName="email,nome,".TrimEnd(',');
			info.Alias="agent";
			info.IsDomain = true;
			info.PersistenceType = PersistenceType.Database;
			info.AreaDesignation="Agente imobiliário";
			info.AreaPluralDesignation="Agentes imobiliário";
			info.DescriptionCav="AGENTE_IMOBILIARIO28727";

			info.KeyType = CodeType.INT_KEY;

			//sincronização
			info.SyncIncrementalDateStart = TimeSpan.FromHours(8);
			info.SyncIncrementalDateEnd = TimeSpan.FromHours(23);
			info.SyncCompleteHour = TimeSpan.FromHours(0.5);
			info.SyncIncrementalPeriod = TimeSpan.FromHours(1);
			info.BatchSync = 100;
			info.SyncType = SyncType.Central;
            info.SolrList = new List<string>();
        	info.QueuesList = new List<GenioServer.business.QueueGenio>();





			//RS 22.03.2011 I separated in submetodos due to performance problems with the JIT in 64bits
			// that in very large projects took 2 minutes on the first call.
			// After a Microsoft analysis of the JIT algortimo it was revealed that it has a
			// complexity O(n*m) where n are the lines of code and m the number of variables of a function.
			// Tests have revealed that splitting into subfunctions cuts the JIT time by more than half by 64-bit.
			//------------------------------
			InicializaCampos(info);

			//------------------------------
			InicializaRelacoes(info);

			//------------------------------
			InicializaCaminhos(info);

			//------------------------------
			InicializaFormulas(info);

			// Automatic audit stamps in BD
            //------------------------------

            // Documents in DB
            //------------------------------

            // Historics
            //------------------------------

			// Duplication
			//------------------------------

			// Ephs
			//------------------------------
			info.Ephs=new Hashtable();

			// Table minimum roles and access levels
			//------------------------------
            info.QLevel = new QLevel();
            info.QLevel.Query = Role.AUTHORIZED;
            info.QLevel.Create = Role.AUTHORIZED;
            info.QLevel.AlterAlways = Role.AUTHORIZED;
            info.QLevel.RemoveAlways = Role.AUTHORIZED;

      		return info;
		}

		/// <summary>
		/// Meta-information about this area
		/// </summary>
		public override AreaInfo Information
		{
			get { return informacao; }
		}
		/// <summary>
		/// Meta-information about this area
		/// </summary>
		public static AreaInfo GetInformation()
		{
			return informacao;
		}

		/// <summary>Field : "" Tipo: "+" Formula:  ""</summary>
		public static FieldRef FldCodagent { get { return m_fldCodagent; } }
		private static FieldRef m_fldCodagent = new FieldRef("agent", "codagent");

		/// <summary>Field : "" Tipo: "+" Formula:  ""</summary>
		public string ValCodagent
		{
			get { return (string)returnValueField(FldCodagent); }
			set { insertNameValueField(FldCodagent, value); }
		}


		/// <summary>Field : "Fotografia" Tipo: "IJ" Formula:  ""</summary>
		public static FieldRef FldFoto { get { return m_fldFoto; } }
		private static FieldRef m_fldFoto = new FieldRef("agent", "foto");

		/// <summary>Field : "Fotografia" Tipo: "IJ" Formula:  ""</summary>
		public byte[] ValFoto
		{
			get { return (byte[])returnValueField(FldFoto); }
			set { insertNameValueField(FldFoto, value); }
		}


		/// <summary>Field : "Nome" Tipo: "C" Formula:  ""</summary>
		public static FieldRef FldNome { get { return m_fldNome; } }
		private static FieldRef m_fldNome = new FieldRef("agent", "nome");

		/// <summary>Field : "Nome" Tipo: "C" Formula:  ""</summary>
		public string ValNome
		{
			get { return (string)returnValueField(FldNome); }
			set { insertNameValueField(FldNome, value); }
		}


		/// <summary>Field : "Data de nascimento" Tipo: "D" Formula:  ""</summary>
		public static FieldRef FldDnascime { get { return m_fldDnascime; } }
		private static FieldRef m_fldDnascime = new FieldRef("agent", "dnascime");

		/// <summary>Field : "Data de nascimento" Tipo: "D" Formula:  ""</summary>
		public DateTime ValDnascime
		{
			get { return (DateTime)returnValueField(FldDnascime); }
			set { insertNameValueField(FldDnascime, value); }
		}


		/// <summary>Field : "E-mail" Tipo: "C" Formula:  ""</summary>
		public static FieldRef FldEmail { get { return m_fldEmail; } }
		private static FieldRef m_fldEmail = new FieldRef("agent", "email");

		/// <summary>Field : "E-mail" Tipo: "C" Formula:  ""</summary>
		public string ValEmail
		{
			get { return (string)returnValueField(FldEmail); }
			set { insertNameValueField(FldEmail, value); }
		}


		/// <summary>Field : "Telefone" Tipo: "C" Formula:  ""</summary>
		public static FieldRef FldTelefone { get { return m_fldTelefone; } }
		private static FieldRef m_fldTelefone = new FieldRef("agent", "telefone");

		/// <summary>Field : "Telefone" Tipo: "C" Formula:  ""</summary>
		public string ValTelefone
		{
			get { return (string)returnValueField(FldTelefone); }
			set { insertNameValueField(FldTelefone, value); }
		}


		/// <summary>Field : "" Tipo: "CE" Formula:  ""</summary>
		public static FieldRef FldCodpmora { get { return m_fldCodpmora; } }
		private static FieldRef m_fldCodpmora = new FieldRef("agent", "codpmora");

		/// <summary>Field : "" Tipo: "CE" Formula:  ""</summary>
		public string ValCodpmora
		{
			get { return (string)returnValueField(FldCodpmora); }
			set { insertNameValueField(FldCodpmora, value); }
		}


		/// <summary>Field : "" Tipo: "CE" Formula:  ""</summary>
		public static FieldRef FldCodpnasc { get { return m_fldCodpnasc; } }
		private static FieldRef m_fldCodpnasc = new FieldRef("agent", "codpnasc");

		/// <summary>Field : "" Tipo: "CE" Formula:  ""</summary>
		public string ValCodpnasc
		{
			get { return (string)returnValueField(FldCodpnasc); }
			set { insertNameValueField(FldCodpnasc, value); }
		}


		/// <summary>Field : "% lucro" Tipo: "N" Formula:  ""</summary>
		public static FieldRef FldPerelucr { get { return m_fldPerelucr; } }
		private static FieldRef m_fldPerelucr = new FieldRef("agent", "perelucr");

		/// <summary>Field : "% lucro" Tipo: "N" Formula:  ""</summary>
		public decimal ValPerelucr
		{
			get { return (decimal)returnValueField(FldPerelucr); }
			set { insertNameValueField(FldPerelucr, value); }
		}


		/// <summary>Field : "Lucro" Tipo: "$" Formula: SR "[PROPR->LUCRO]"</summary>
		public static FieldRef FldLucro { get { return m_fldLucro; } }
		private static FieldRef m_fldLucro = new FieldRef("agent", "lucro");

		/// <summary>Field : "Lucro" Tipo: "$" Formula: SR "[PROPR->LUCRO]"</summary>
		public decimal ValLucro
		{
			get { return (decimal)returnValueField(FldLucro); }
			set { insertNameValueField(FldLucro, value); }
		}


		/// <summary>Field : "ZZSTATE" Type: "INT" Formula:  ""</summary>
		public static FieldRef FldZzstate { get { return m_fldZzstate; } }
		private static FieldRef m_fldZzstate = new FieldRef("agent", "zzstate");



		/// <summary>Field : "ZZSTATE" Type: "INT"</summary>
		public int ValZzstate
		{
			get { return (int)returnValueField(FldZzstate); }
			set { insertNameValueField(FldZzstate, value); }
		}

        /// <summary>
        /// Obtains a partially populated area with the record corresponding to a primary key
        /// </summary>
        /// <param name="sp">Persistent support from where to get the registration</param>
        /// <param name="key">The value of the primary key</param>
        /// <param name="user">The context of the user</param>
        /// <param name="fields">The fields to be filled in the area</param>
        /// <returns>An area with the fields requests of the record read or null if the key does not exist</returns>
        /// <remarks>Persistence operations should not be used on a partially positioned register</remarks>
        public static CSGenioAagent search(PersistentSupport sp, string key, User user, string[] fields = null)
        {
			if (string.IsNullOrEmpty(key))
				return null;

		    CSGenioAagent area = new CSGenioAagent(user, user.CurrentModule);

            if (sp.getRecord(area, key, fields))
                return area;
			return null;
        }


		public static string GetkeyFromControlledRecord(PersistentSupport sp, string ID, User user)
		{
			if (informacao.ControlledRecords != null)
				return informacao.ControlledRecords.GetPrimaryKeyFromControlledRecord(sp, user, ID);
			return String.Empty;
		}



        /// <summary>
        /// Search for all records of this area that comply with a condition
        /// </summary>
        /// <param name="sp">Persistent support from where to get the list</param>
        /// <param name="user">The context of the user</param>
        /// <param name="where">The search condition for the records. Use null to get all records</param>
        /// <param name="fields">The fields to be filled in the area</param>
        /// <returns>A list of area records with all fields populated</returns>
        /// <remarks>Persistence operations should not be used on a partially positioned register</remarks>
        [Obsolete("Use List<CSGenioAagent> searchList(PersistentSupport sp, User user, CriteriaSet where, string []fields) instead")]
        public static List<CSGenioAagent> searchList(PersistentSupport sp, User user, string where, string []fields = null)
        {
            return sp.searchListWhere<CSGenioAagent>(where, user, fields);
        }


        /// <summary>
        /// Search for all records of this area that comply with a condition
        /// </summary>
        /// <param name="sp">Persistent support from where to get the list</param>
        /// <param name="user">The context of the user</param>
        /// <param name="where">The search condition for the records. Use null to get all records</param>
        /// <param name="fields">The fields to be filled in the area</param>
        /// <param name="distinct">Get distinct from fields</param>
        /// <param name="noLock">NOLOCK</param>
        /// <returns>A list of area records with all fields populated</returns>
        /// <remarks>Persistence operations should not be used on a partially positioned register</remarks>
        public static List<CSGenioAagent> searchList(PersistentSupport sp, User user, CriteriaSet where, string[] fields = null, bool distinct = false, bool noLock = false)
        {
				return sp.searchListWhere<CSGenioAagent>(where, user, fields, distinct, noLock);
        }



       	/// <summary>
        /// Search for all records of this area that comply with a condition
        /// </summary>
        /// <param name="sp">Persistent support from where to get the list</param>
        /// <param name="user">The context of the user</param>
        /// <param name="where">The search condition for the records. Use null to get all records</param>
        /// <param name="listing">List configuration</param>
        /// <returns>A list of area records with all fields populated</returns>
        /// <remarks>Persistence operations should not be used on a partially positioned register</remarks>
        public static void searchListAdvancedWhere(PersistentSupport sp, User user, CriteriaSet where, ListingMVC<CSGenioAagent> listing)
        {
			sp.searchListAdvancedWhere<CSGenioAagent>(where, listing);
        }




		/// <summary>
		/// Check if a record exist
		/// </summary>
		/// <param name="key">Record key</param>
		/// <param name="sp">DB conecntion</param>
		/// <returns>True if the record exist</returns>
		public static bool RecordExist(string key, PersistentSupport sp) => DbArea.RecordExist(key, informacao, sp);







		// USE /[MANUAL PRO TABAUX AGENT]/

 

           

	}
}
