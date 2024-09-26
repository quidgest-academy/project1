

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
	/// Contacto de cliente
	/// </summary>
	public class CSGenioAcontc : DbArea	{
		/// <summary>
		/// Meta-information on this area
		/// </summary>
		protected readonly static AreaInfo informacao = InicializaAreaInfo();

		public CSGenioAcontc(User user, string module)
		{
			fields = new Hashtable();
            this.user = user;
            this.module = module;
			this.KeyType = CodeType.INT_KEY;
			// USE /[MANUAL PRO CONSTRUTOR CONTC]/
		}

		public CSGenioAcontc(User user) : this(user, user.CurrentModule)
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
			Qfield = new Field("codcontc", FieldType.CHAVE_PRIMARIA);
			Qfield.FieldDescription = "";
			Qfield.FieldSize =  8;
			Qfield.Alias = info.Alias;
			Qfield.MQueue = false;
			Qfield.CavDesignation = "";

			Qfield.Dupmsg = "";
			info.RegisterFieldDB(Qfield);

			//- - - - - - - - - - - - - - - - - - -
			Qfield = new Field("dtcontat", FieldType.DATA);
			Qfield.FieldDescription = "Data do contacto";
			Qfield.FieldSize =  8;
			Qfield.Alias = info.Alias;
			Qfield.MQueue = false;
			Qfield.CavDesignation = "DATA_DO_CONTACTO52251";

			Qfield.Dupmsg = "";
			info.RegisterFieldDB(Qfield);

			//- - - - - - - - - - - - - - - - - - -
			Qfield = new Field("codpropr", FieldType.CHAVE_ESTRANGEIRA);
			Qfield.FieldDescription = "";
			Qfield.FieldSize =  8;
			Qfield.Alias = info.Alias;
			Qfield.MQueue = false;
			Qfield.CavDesignation = "";

			Qfield.Dupmsg = "";
			info.RegisterFieldDB(Qfield);

			//- - - - - - - - - - - - - - - - - - -
			Qfield = new Field("cltname", FieldType.TEXTO);
			Qfield.FieldDescription = "Nome do cliente";
			Qfield.FieldSize =  50;
			Qfield.Alias = info.Alias;
			Qfield.MQueue = false;
			Qfield.CavDesignation = "NOME_DO_CLIENTE38483";

            Qfield.NotNull = true;
			Qfield.Dupmsg = "";
			info.RegisterFieldDB(Qfield);

			//- - - - - - - - - - - - - - - - - - -
			Qfield = new Field("cltemail", FieldType.TEXTO);
			Qfield.FieldDescription = "Email do cliente";
			Qfield.FieldSize =  80;
			Qfield.Alias = info.Alias;
			Qfield.MQueue = false;
			Qfield.CavDesignation = "EMAIL_DO_CLIENTE30111";

            Qfield.NotNull = true;
			Qfield.Dupmsg = "";
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
			Qfield = new Field("descriic", FieldType.MEMO);
			Qfield.FieldDescription = "Descrição";
			Qfield.FieldSize =  80;
			Qfield.Alias = info.Alias;
			Qfield.MQueue = false;
			Qfield.Decimals = 5;
			Qfield.CavDesignation = "DESCRICAO07528";

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

			// Mother Relations
			//------------------------------
			info.ParentTables = new Dictionary<string, Relation>();
			info.ParentTables.Add("propr", new Relation("PRO", "procontc", "contc", "codcontc", "codpropr", "PRO", "propropr", "propr", "codpropr", "codpropr"));
		}

		/// <summary>
		/// Initializes metadata for indirect paths to other areas
		/// </summary>
		private static void InicializaCaminhos(AreaInfo info)
		{
			// Pathways
			//------------------------------
			info.Pathways = new Dictionary<string, string>(6);
			info.Pathways.Add("propr","propr");
			info.Pathways.Add("cidad","propr");
			info.Pathways.Add("agent","propr");
			info.Pathways.Add("pais","propr");
			info.Pathways.Add("pmora","propr");
			info.Pathways.Add("pnasc","propr");
		}

		/// <summary>
		/// Initializes metadata for triggers and formula arguments
		/// </summary>
		private static void InicializaFormulas(AreaInfo info)
		{
			// Formulas
			//------------------------------








			//Write conditions
			List<ConditionFormula> conditions = new List<ConditionFormula>();
			info.WriteConditions = conditions.Where(c=> c.IsWriteCondition()).ToList();
			info.CrudConditions = conditions.Where(c=> c.IsCrudCondition()).ToList();

		}

		/// <summary>
		/// static CSGenioAcontc()
		/// </summary>
		private static AreaInfo InicializaAreaInfo()
		{
			AreaInfo info = new AreaInfo();

			// Area meta-information
			info.QSystem="PRO";
			info.TableName="procontc";
			info.ShadowTabName="";
			info.ShadowTabKeyName="";

			info.PrimaryKeyName="codcontc";
			info.HumanKeyName="cltemail,cltname,".TrimEnd(',');
			info.Alias="contc";
			info.IsDomain = true;
			info.PersistenceType = PersistenceType.Database;
			info.AreaDesignation="Contacto de cliente";
			info.AreaPluralDesignation="Contactos de clientes";
			info.DescriptionCav="CONTACTO_DE_CLIENTE62085";

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
		public static FieldRef FldCodcontc { get { return m_fldCodcontc; } }
		private static FieldRef m_fldCodcontc = new FieldRef("contc", "codcontc");

		/// <summary>Field : "" Tipo: "+" Formula:  ""</summary>
		public string ValCodcontc
		{
			get { return (string)returnValueField(FldCodcontc); }
			set { insertNameValueField(FldCodcontc, value); }
		}


		/// <summary>Field : "Data do contacto" Tipo: "D" Formula:  ""</summary>
		public static FieldRef FldDtcontat { get { return m_fldDtcontat; } }
		private static FieldRef m_fldDtcontat = new FieldRef("contc", "dtcontat");

		/// <summary>Field : "Data do contacto" Tipo: "D" Formula:  ""</summary>
		public DateTime ValDtcontat
		{
			get { return (DateTime)returnValueField(FldDtcontat); }
			set { insertNameValueField(FldDtcontat, value); }
		}


		/// <summary>Field : "" Tipo: "CE" Formula:  ""</summary>
		public static FieldRef FldCodpropr { get { return m_fldCodpropr; } }
		private static FieldRef m_fldCodpropr = new FieldRef("contc", "codpropr");

		/// <summary>Field : "" Tipo: "CE" Formula:  ""</summary>
		public string ValCodpropr
		{
			get { return (string)returnValueField(FldCodpropr); }
			set { insertNameValueField(FldCodpropr, value); }
		}


		/// <summary>Field : "Nome do cliente" Tipo: "C" Formula:  ""</summary>
		public static FieldRef FldCltname { get { return m_fldCltname; } }
		private static FieldRef m_fldCltname = new FieldRef("contc", "cltname");

		/// <summary>Field : "Nome do cliente" Tipo: "C" Formula:  ""</summary>
		public string ValCltname
		{
			get { return (string)returnValueField(FldCltname); }
			set { insertNameValueField(FldCltname, value); }
		}


		/// <summary>Field : "Email do cliente" Tipo: "C" Formula:  ""</summary>
		public static FieldRef FldCltemail { get { return m_fldCltemail; } }
		private static FieldRef m_fldCltemail = new FieldRef("contc", "cltemail");

		/// <summary>Field : "Email do cliente" Tipo: "C" Formula:  ""</summary>
		public string ValCltemail
		{
			get { return (string)returnValueField(FldCltemail); }
			set { insertNameValueField(FldCltemail, value); }
		}


		/// <summary>Field : "Telefone" Tipo: "C" Formula:  ""</summary>
		public static FieldRef FldTelefone { get { return m_fldTelefone; } }
		private static FieldRef m_fldTelefone = new FieldRef("contc", "telefone");

		/// <summary>Field : "Telefone" Tipo: "C" Formula:  ""</summary>
		public string ValTelefone
		{
			get { return (string)returnValueField(FldTelefone); }
			set { insertNameValueField(FldTelefone, value); }
		}


		/// <summary>Field : "Descrição" Tipo: "MO" Formula:  ""</summary>
		public static FieldRef FldDescriic { get { return m_fldDescriic; } }
		private static FieldRef m_fldDescriic = new FieldRef("contc", "descriic");

		/// <summary>Field : "Descrição" Tipo: "MO" Formula:  ""</summary>
		public string ValDescriic
		{
			get { return (string)returnValueField(FldDescriic); }
			set { insertNameValueField(FldDescriic, value); }
		}


		/// <summary>Field : "ZZSTATE" Type: "INT" Formula:  ""</summary>
		public static FieldRef FldZzstate { get { return m_fldZzstate; } }
		private static FieldRef m_fldZzstate = new FieldRef("contc", "zzstate");



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
        public static CSGenioAcontc search(PersistentSupport sp, string key, User user, string[] fields = null)
        {
			if (string.IsNullOrEmpty(key))
				return null;

		    CSGenioAcontc area = new CSGenioAcontc(user, user.CurrentModule);

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
        [Obsolete("Use List<CSGenioAcontc> searchList(PersistentSupport sp, User user, CriteriaSet where, string []fields) instead")]
        public static List<CSGenioAcontc> searchList(PersistentSupport sp, User user, string where, string []fields = null)
        {
            return sp.searchListWhere<CSGenioAcontc>(where, user, fields);
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
        public static List<CSGenioAcontc> searchList(PersistentSupport sp, User user, CriteriaSet where, string[] fields = null, bool distinct = false, bool noLock = false)
        {
				return sp.searchListWhere<CSGenioAcontc>(where, user, fields, distinct, noLock);
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
        public static void searchListAdvancedWhere(PersistentSupport sp, User user, CriteriaSet where, ListingMVC<CSGenioAcontc> listing)
        {
			sp.searchListAdvancedWhere<CSGenioAcontc>(where, listing);
        }




		/// <summary>
		/// Check if a record exist
		/// </summary>
		/// <param name="key">Record key</param>
		/// <param name="sp">DB conecntion</param>
		/// <returns>True if the record exist</returns>
		public static bool RecordExist(string key, PersistentSupport sp) => DbArea.RecordExist(key, informacao, sp);







		// USE /[MANUAL PRO TABAUX CONTC]/

 

        

	}
}
