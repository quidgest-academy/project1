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
	public class CSGenioApswuserauthlevels : DbArea
	{
	    /// <summary>
		/// Meta-informa��o sobre esta �rea
		/// </summary>
		protected static AreaInfo informacao = InicializaAreaInfo();

		public CSGenioApswuserauthlevels(User user,string module)
		{
			fields = new Hashtable();
            this.user = user;
            this.module = module;
		}
	
		public CSGenioApswuserauthlevels(User user) : this(user, user.CurrentModule)
		{
		}
	
		private static AreaInfo InicializaAreaInfo()
		{
			AreaInfo info = new AreaInfo();
			
			/*Information das areas*/
			info.TableName = "UserAuthorization";
			info.ShadowTabName = "";
			info.PrimaryKeyName = "codua";
            info.HumanKeyName = "codua";
			info.ShadowTabKeyName = "";
			info.Alias = "pswuserauthlevels";
			info.IsDomain =  false;
			info.AreaDesignation = "Autorização";
			info.AreaPluralDesignation = "Autorizações";
			info.DescriptionCav = "Autorização";
			
			//sincroniza��o
			info.SyncIncrementalDateStart = TimeSpan.FromHours(9.0);
			info.SyncIncrementalDateEnd = TimeSpan.FromHours(23.0);
			info.SyncCompleteHour = TimeSpan.FromHours(1.0);
			info.SyncIncrementalPeriod = TimeSpan.FromHours(1);
			info.BatchSync = 100;
			info.SyncType = SyncType.Central;
					
			info.RegisterFieldDB(new Field("codua", FieldType.CHAVE_PRIMARIA));
			info.DBFields["codua"].FieldSize = 6;
			info.KeyType = CodeType.INT_KEY;
			info.RegisterFieldDB(new Field("codpsw", FieldType.CHAVE_ESTRANGEIRA));
			info.DBFields["codpsw"].FieldSize = 8;
			info.RegisterFieldDB(new Field("sistema", FieldType.TEXTO));
			info.RegisterFieldDB(new Field("modulo", FieldType.TEXTO));
			info.RegisterFieldDB(new Field("nivel", FieldType.NUMERO));
            info.RegisterFieldDB(new Field("role", FieldType.TEXTO));            
			info.RegisterFieldDB(new Field("opercria", FieldType.TEXTO));
      info.RegisterFieldDB(new Field("datacria", FieldType.DATA));
      info.RegisterFieldDB(new Field("opermuda", FieldType.TEXTO));
      info.RegisterFieldDB(new Field("datamuda", FieldType.DATA));
			info.RegisterFieldDB(new Field("zzstate", FieldType.INTEIRO));

			// Rela��es Filhas
			//------------------------------

			// Rela��es M�e
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
		/// Meta-informa��o sobre esta �rea
		/// </summary>
		public override AreaInfo Information
		{
			get { return informacao; }
		}
		/// <summary>
		/// Meta-informa��o sobre esta �rea
		/// </summary>		
		public static AreaInfo GetInformation()
		{
			return informacao;
		}

		// USE /[MANUAL PRO TABAUX pswuserauthlevels]/

		
        public static FieldRef FldCodua { get { return m_FldCodua; } }
        private static FieldRef m_FldCodua = new FieldRef("pswuserauthlevels", "codua");

        public string ValCodua
        {
            get { return (string)returnValueField(FldCodua); }
            set { insertNameValueField(FldCodua, value); }
        }

        public static FieldRef FldCodpsw { get { return m_FldCodpsw; } }
        private static FieldRef m_FldCodpsw = new FieldRef("pswuserauthlevels", "codpsw");

        public string ValCodpsw
        {
            get { return (string)returnValueField(FldCodpsw); }
            set { insertNameValueField(FldCodpsw, value); }
        }

        public static FieldRef FldSistema { get { return m_FldSistema; } }
        private static FieldRef m_FldSistema = new FieldRef("pswuserauthlevels", "sistema");

        public string ValSistema
        {
            get { return (string)returnValueField(FldSistema); }
            set { insertNameValueField(FldSistema, value); }
        }

        public static FieldRef FldModulo { get { return m_FldModulo; } }
        private static FieldRef m_FldModulo = new FieldRef("pswuserauthlevels", "modulo");

        public string ValModulo
        {
            get { return (string)returnValueField(FldModulo); }
            set { insertNameValueField(FldModulo, value); }
        }
        
        public static FieldRef FldNivel { get { return m_FldNivel; } }
        private static FieldRef m_FldNivel = new FieldRef("pswuserauthlevels", "nivel");

        public decimal ValNivel
        {
            get { return (decimal)returnValueField(FldNivel); }
            set { insertNameValueField(FldNivel, value); }
        }

        public static FieldRef FldRole { get { return m_FldRole; } }
        private static FieldRef m_FldRole = new FieldRef("pswuserauthlevels", "role");

        public string ValRole
        {
            get { return (string)returnValueField(FldRole); }
            set { insertNameValueField(FldRole, value); }
        }
        
        public static FieldRef FldZzstate { get { return m_FldZzstate; } }
        private static FieldRef m_FldZzstate = new FieldRef("pswuserauthlevels", "zzstate");

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
        public static CSGenioApswuserauthlevels search(PersistentSupport sp, string key, User user, string[] fields = null)
        {
            CSGenioApswuserauthlevels area = new CSGenioApswuserauthlevels(user, user.CurrentModule);
            if (sp.getRecord(area, key, fields))
                return area;
            return null;
        }

        /// <summary>
        /// Procura todos os registos desta area que obedecem a uma condição
        /// </summary>
        /// <param name="sp">O suporte persistente de onde obter a lista</param>
        /// <param name="utilizador">O contexto do user</param>
        /// <param name="where">A condição de procura dos registos. Usar null to obter todos os registos</param>
        /// <returns>Uma lista de registos da areas com todos os fields preenchidos</returns>
        public static List<CSGenioApswuserauthlevels> searchList(PersistentSupport sp, User user, CriteriaSet where)
        {
            return sp.searchListWhere<CSGenioApswuserauthlevels>(where, user, null);
        }
    
        /// <summary>
        /// Procura todos os registos desta area que obedecem a uma condição
        /// </summary>
        /// <param name="sp">O suporte persistente de onde obter a lista</param>
        /// <param name="utilizador">O contexto do user</param>
        /// <param name="where">A condição de procura dos registos. Usar null to obter todos os registos</param>
        /// <param name="campos">Os fields a serem preenchidos na area</param>
        /// <returns>Uma lista de registos da areas com todos os fields preenchidos</returns>
        /// <remarks>Não devem ser utilizadas operações de persistence sobre um registo parcialmente posicionado</remarks>
        public static List<CSGenioApswuserauthlevels> searchList(PersistentSupport sp, User user, CriteriaSet where, string []fields)
        {
            return sp.searchListWhere<CSGenioApswuserauthlevels>(where, user, fields);
        }

	}
}
