using System;
using CSGenio.framework;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using CSGenio.persistence;
using System.Text;
using Quidgest.Persistence;
using Quidgest.Persistence.GenericQuery;

namespace CSGenio.business
{
	/// <summary>
	/// Summary description for CSArea.
	/// </summary>
	public class CSGenioAtblcfgsel : DbArea
	{
	    /// <summary>
		/// Meta-informação sobre esta àrea
		/// </summary>
		protected static AreaInfo informacao = InicializaAreaInfo();

		public CSGenioAtblcfgsel(User user,string module)
		{
			fields = new Hashtable();
            this.user = user;
            this.module = module;
		}
	
		public CSGenioAtblcfgsel(User user) : this(user, user.CurrentModule)
		{
		}
	
		private static AreaInfo InicializaAreaInfo()
		{
			AreaInfo info = new AreaInfo();
			
			/*Information das areas*/
			info.TableName = "PROtblcfgsel";
			info.ShadowTabName = "";
			info.PrimaryKeyName = "codtblcfgsel";
            info.HumanKeyName = "codtblcfgsel";
			info.ShadowTabKeyName = "";
			info.Alias = "tblcfgsel";
			info.IsDomain =  false;
			info.AreaDesignation = "User table configuration";
			info.AreaPluralDesignation = "User table configurations";
			info.DescriptionCav = "User table configuration";
			
			//sincronização
			info.SyncIncrementalDateStart = TimeSpan.FromHours(9.0);
			info.SyncIncrementalDateEnd = TimeSpan.FromHours(23.0);
			info.SyncCompleteHour = TimeSpan.FromHours(1.0);
			info.SyncIncrementalPeriod = TimeSpan.FromHours(1);
			info.BatchSync = 100;
			info.SyncType = SyncType.Central;
					
			info.RegisterFieldDB(new Field("codtblcfgsel", FieldType.CHAVE_PRIMARIA));
			info.DBFields["codtblcfgsel"].FieldSize = 6;
			info.RegisterFieldDB(new Field("codpsw", FieldType.CHAVE_ESTRANGEIRA));
			info.DBFields["codpsw"].FieldSize = 8;
			info.KeyType = CodeType.INT_KEY;
			info.RegisterFieldDB(new Field("uuid", FieldType.TEXTO){NotDup = true, PrefNDup = "codpsw"});
			info.RegisterFieldDB(new Field("codtblcfg", FieldType.CHAVE_ESTRANGEIRA));
			info.DBFields["codtblcfg"].FieldSize = 6;
            info.RegisterFieldDB(new Field("date", FieldType.DATACRIA));
            info.RegisterFieldDB(new Field("zzstate", FieldType.INTEIRO));

            // Carimbos automáticos na BD
            //------------------------------
   			info.StampFieldsIns = new string[] {
             "date"
			};

			// Relações Filhas
			//------------------------------

			// Relações Mãe
			//------------------------------

			// Pathways
			//------------------------------

			// Levels de acesso
			//------------------------------
			info.QLevel = new QLevel();
			info.QLevel.Query = Role.UNAUTHORIZED;
			info.QLevel.Create = Role.UNAUTHORIZED;
			info.QLevel.AlterAlways = Role.UNAUTHORIZED;
			info.QLevel.RemoveAlways = Role.UNAUTHORIZED;

			// Automatic audit stamps in BD
            //------------------------------


			return info;
		}
		
		/// <summary>
		/// Meta-informação sobre esta àrea
		/// </summary>
		public override AreaInfo Information
		{
			get { return informacao; }
		}
		/// <summary>
		/// Meta-informação sobre esta àrea
		/// </summary>		
		public static AreaInfo GetInformation()
		{
			return informacao;
		}

		// USE /[MANUAL PRO TABAUX tblcfgsel]/

		
        public string FormMode { get; set; }
        public string ResultMsg { get; set; }

        ////////////////////////////////////////////////////////////////////////////BD fields CONVERTED TO Areainfo style
        /// <summary>Campo : "PK da tabela tblcfgsel" Tipo: "+" Formula:  ""</summary>
        public static FieldRef FldCodtblcfgsel { get { return m_fldCodtblcfgsel; } }
        private static FieldRef m_fldCodtblcfgsel = new FieldRef("tblcfgsel", "codtblcfgsel");

        /// <summary>Campo : "PK da tabela tblcfgsel" Tipo: "+" Formula:  ""</summary>
        public string ValCodtblcfgsel
        {
            get { return (string)returnValueField(FldCodtblcfgsel); }
            set { insertNameValueField(FldCodtblcfgsel, value); }
        }

        /// <summary>Campo : "PK da tabela psw" Tipo: "+" Formula:  ""</summary>
        public static FieldRef FldCodpsw { get { return m_fldCodpsw; } }
        private static FieldRef m_fldCodpsw = new FieldRef("tblcfgsel", "codpsw");

        /// <summary>Campo : "PK da tabela psw" Tipo: "+" Formula:  ""</summary>
        public string ValCodpsw
        {
            get { return (string)returnValueField(FldCodpsw); }
            set { insertNameValueField(FldCodpsw, value); }
        }

        /// <summary>Campo : "uuid" Tipo: "C" Formula:  ""</summary>
        public static FieldRef FldUuid { get { return m_fldUuid; } }
        private static FieldRef m_fldUuid = new FieldRef("tblcfgsel", "uuid");

        /// <summary>Campo : "uuid" Tipo: "C" Formula:  ""</summary>
        public string ValUuid
        {
            get { return (string)returnValueField(FldUuid); }
            set { insertNameValueField(FldUuid, value); }
        }

        /// <summary>Campo : "PK of selected configuration" Tipo: "C" Formula:  ""</summary>
        public static FieldRef FldCodtblcfg { get { return m_fldCodtblcfg; } }
        private static FieldRef m_fldCodtblcfg = new FieldRef("tblcfgsel", "codtblcfg");

        /// <summary>Campo : "PK of selected configuration" Tipo: "C" Formula:  ""</summary>
        public string ValCodtblcfg
        {
            get { return (string)returnValueField(FldCodtblcfg); }
            set { insertNameValueField(FldCodtblcfg, value); }
        }

        /// <summary>Campo : "Criação: Date" Tipo: "OD" Formula:  ""</summary>
        public static FieldRef FldDate { get { return m_fldDate; } }
        private static FieldRef m_fldDate = new FieldRef("tblcfgsel", "date");

        /// <summary>Campo : "Criação: Date" Tipo: "OD" Formula:  ""</summary>
        public DateTime ValDate
        {
            get { return (DateTime)returnValueField(FldDate); }
            set { insertNameValueField(FldDate, value); }
        }

        /// <summary>Campo : "ZZSTATE" Tipo: "INT" Formula:  ""</summary>
        public static FieldRef FldZzstate { get { return m_fldZzstate; } }
        private static FieldRef m_fldZzstate = new FieldRef("tblcfgsel", "zzstate");

        /// <summary>Campo : "ZZSTATE" Tipo: "INT"</summary>
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
		public static CSGenioAtblcfgsel search(PersistentSupport sp, string key, User user, string[] fields = null)
        {
            if (string.IsNullOrEmpty(key)) //para proteger chamadas "cegas"
                return null;
            CSGenioAtblcfgsel area = new CSGenioAtblcfgsel(user, user.CurrentModule);
            if (sp.getRecord(area, key, fields))
                return area;
            return null;
        }

        /// <summary>
        /// Procura todos os registos desta area que obedecem a uma condição
        /// </summary>
        /// <param name="sp">O suporte persistente de onde obter a lista</param>
        /// <param name="User">O contexto do User</param>
        /// <param name="where">A condição de procura dos registos. Usar null para obter todos os registos</param>
        /// <returns>Uma lista de registos da areas com todos os campos preenchidos</returns>
        public static List<CSGenioAtblcfgsel> searchList(PersistentSupport sp, User User, CriteriaSet where)
        {
            return searchList(sp, User, where, null);
        }
   
        /// <summary>
        /// Procura todos os registos desta area que obedecem a uma condição
        /// </summary>
        /// <param name="sp">O suporte persistente de onde obter a lista</param>
        /// <param name="User">O contexto do User</param>
        /// <param name="where">A condição de procura dos registos. Usar null para obter todos os registos</param>
        /// <param name="campos">Os campos a serem preenchidos na area</param>
        /// <returns>Uma lista de registos da areas com todos os campos preenchidos</returns>
        /// <remarks>Não devem ser utilizadas operações de persistence sobre um registo parcialmente posicionado</remarks>
        public static List<CSGenioAtblcfgsel> searchList(PersistentSupport sp, User User, CriteriaSet where, string[] campos)
        {
            return sp.searchListWhere<CSGenioAtblcfgsel>(where, User, campos);
        }
		
        /// <summary>
        /// Procura todos os registos desta area que obedecem a uma condição
        /// </summary>
        /// <param name="sp">O suporte persistente de onde obter a lista</param>
        /// <param name="User">O contexto do User</param>
        /// <param name="where">A condição de procura dos registos. Usar null para obter todos os registos</param>
        /// <param name="campos">Os campos a serem preenchidos na area</param>
        /// <param name="distinct">Obter distinct de campos</param>
        /// <param name="noLock">NOLOCK</param>
        /// <returns>Uma lista de registos da areas com todos os campos preenchidos</returns>
        /// <remarks>Não devem ser utilizadas operações de persistence sobre um registo parcialmente posicionado</remarks>
		public static void searchListAdvancedWhere(PersistentSupport sp, User User, CriteriaSet where, ListingMVC<CSGenioAtblcfgsel> listing)
        {
            sp.searchListAdvancedWhere<CSGenioAtblcfgsel>(where, listing);
        }
		
        /// <summary>
        /// Procura todos os registos desta area que obedecem a uma condição
        /// </summary>
        /// <param name="sp">O suporte persistente de onde obter a lista</param>
        /// <param name="User">O contexto do User</param>
        /// <param name="where">A condição de procura dos registos. Usar null para obter todos os registos</param>
        /// <param name="campos">Os campos a serem preenchidos na area</param>
        /// <param name="distinct">Obter distinct de campos</param>
        /// <param name="noLock">NOLOCK</param>
        /// <returns>Uma lista de registos da areas com todos os campos preenchidos</returns>
        /// <remarks>Não devem ser utilizadas operações de persistence sobre um registo parcialmente posicionado</remarks>
        public static List<CSGenioAtblcfgsel> searchList(PersistentSupport sp, User User, CriteriaSet where, string[] campos, bool distinct, bool noLock = false)
        {
            return sp.searchListWhere<CSGenioAtblcfgsel>(where, User, campos, distinct, noLock);
        }

        /// <summary>
        /// Procura todos os registos desta area que obedecem a uma condição
        /// </summary>
        /// <param name="sp">O suporte persistente de onde obter a lista</param>
        /// <param name="User">O contexto do User</param>
        /// <param name="where">A condição de procura dos registos. Usar null para obter todos os registos</param>
        /// <param name="campos">Os campos a serem preenchidos na area</param>
        /// <param name="distinct">Obter distinct de campos</param>
        /// <returns>Uma lista de registos da areas com todos os campos preenchidos</returns>
        /// <remarks>Não devem ser utilizadas operações de persistence sobre um registo parcialmente posicionado</remarks>
		public static List<CSGenioAtblcfgsel> searchList(PersistentSupport sp, User User, CriteriaSet where, bool distinct, bool noLock = false)
        {
            return searchList(sp, User, where, null, distinct, noLock);
        }

	}
}
