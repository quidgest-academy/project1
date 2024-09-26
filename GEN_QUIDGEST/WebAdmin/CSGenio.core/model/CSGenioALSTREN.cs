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
	public class CSGenioAlstren : DbArea
	{
	    /// <summary>
		/// Meta-informa��o sobre esta �rea
		/// </summary>
		protected static AreaInfo informacao = InicializaAreaInfo();

		public CSGenioAlstren(User user,string module)
		{
			fields = new Hashtable();
            this.user = user;
            this.module = module;
		}
	
		public CSGenioAlstren(User user) : this(user, user.CurrentModule)
		{
		}
	
		private static AreaInfo InicializaAreaInfo()
		{
			AreaInfo info = new AreaInfo();
			
			/*Information das areas*/
			info.TableName = "PROlstren";
			info.ShadowTabName = "";
			info.PrimaryKeyName = "codlstren";
            info.HumanKeyName = "renderizacao";
			info.ShadowTabKeyName = "";
			info.Alias = "lstren";
			info.IsDomain =  false;
			info.AreaDesignation = "Configuração de renderização";
			info.AreaPluralDesignation = "Configuração de renderizações";
			info.DescriptionCav = "Configuração de renderização";
			
			//sincroniza��o
			info.SyncIncrementalDateStart = TimeSpan.FromHours(9.0);
			info.SyncIncrementalDateEnd = TimeSpan.FromHours(23.0);
			info.SyncCompleteHour = TimeSpan.FromHours(1.0);
			info.SyncIncrementalPeriod = TimeSpan.FromHours(1);
			info.BatchSync = 100;
			info.SyncType = SyncType.Central;
					
			info.RegisterFieldDB(new Field("codlstren", FieldType.CHAVE_PRIMARIA));
			info.DBFields["codlstren"].FieldSize = 6;
			info.KeyType = CodeType.INT_KEY;
			info.RegisterFieldDB(new Field("codlstusr", FieldType.CHAVE_ESTRANGEIRA));
			info.DBFields["codlstusr"].FieldSize = 6;

            info.RegisterFieldDB(new Field("renderizacao", FieldType.TEXTO));
            info.RegisterFieldDB(new Field("visivel", FieldType.LOGICO));
            info.RegisterFieldDB(new Field("posicao", FieldType.NUMERO));
            info.RegisterFieldDB(new Field("operacao", FieldType.NUMERO));
            info.RegisterFieldDB(new Field("tipo", FieldType.NUMERO));
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

		// USE /[MANUAL PRO TABAUX lstren]/

		
        public string FormMode { get; set; }
        public string ResultMsg { get; set; }

        public static FieldRef FldCodlstren { get { return m_fldCodlstren; } }
        private static FieldRef m_fldCodlstren = new FieldRef("lstren", "codlstren");

        /// <summary>Field : "" Tipo: "+" Formula:  ""</summary>
        public string ValCodlstren
        {
            get { return (string)returnValueField(FldCodlstren); }
            set { insertNameValueField(FldCodlstren, value); }
        }


        /// <summary>Field : "" Tipo: "CE" Formula:  ""</summary>
        public static FieldRef FldCodlstusr { get { return m_fldCodlstusr; } }
        private static FieldRef m_fldCodlstusr = new FieldRef("lstren", "codlstusr");

        /// <summary>Field : "" Tipo: "CE" Formula:  ""</summary>
        public string ValCodlstusr
        {
            get { return (string)returnValueField(FldCodlstusr); }
            set { insertNameValueField(FldCodlstusr, value); }
        }


        /// <summary>Field : "Renderizacao" Tipo: "C" Formula:  ""</summary>
        public static FieldRef FldRenderizacao { get { return m_fldRenderizacao; } }
        private static FieldRef m_fldRenderizacao = new FieldRef("lstren", "renderizacao");

        /// <summary>Field : "Renderizacao" Tipo: "C" Formula:  ""</summary>
        public string ValRenderizacao
        {
            get { return (string)returnValueField(FldRenderizacao); }
            set { insertNameValueField(FldRenderizacao, value); }
        }


        /// <summary>Field : "Vis�vel" Tipo: "L" Formula:  ""</summary>
        public static FieldRef FldVisivel { get { return m_fldVisivel; } }
        private static FieldRef m_fldVisivel = new FieldRef("lstren", "visivel");

        /// <summary>Field : "Vis�vel" Tipo: "L" Formula:  ""</summary>
        public int ValVisivel
        {
            get { return (int)returnValueField(FldVisivel); }
            set { insertNameValueField(FldVisivel, value); }
        }


        /// <summary>Field : "Posi��o" Tipo: "N" Formula:  ""</summary>
        public static FieldRef FldPosicao { get { return m_fldPosicao; } }
        private static FieldRef m_fldPosicao = new FieldRef("lstren", "posicao");

        /// <summary>Field : "Posi��o" Tipo: "N" Formula:  ""</summary>
        public decimal ValPosicao
        {
            get { return (decimal)returnValueField(FldPosicao); }
            set { insertNameValueField(FldPosicao, value); }
        }


        /// <summary>Field : "Opera��o" Tipo: "N" Formula:  ""</summary>
        public static FieldRef FldOperacao { get { return m_fldOperacao; } }
        private static FieldRef m_fldOperacao = new FieldRef("lstren", "operacao");

        /// <summary>Field : "Opera��o" Tipo: "N" Formula:  ""</summary>
        public decimal ValOperacao
        {
            get { return (decimal)returnValueField(FldOperacao); }
            set { insertNameValueField(FldOperacao, value); }
        }


        /// <summary>Field : "Tipo" Tipo: "N" Formula:  ""</summary>
        public static FieldRef FldTipo { get { return m_fldTipo; } }
        private static FieldRef m_fldTipo = new FieldRef("lstren", "tipo");

        /// <summary>Field : "Tipo" Tipo: "N" Formula:  ""</summary>
        public decimal ValTipo
        {
            get { return (decimal)returnValueField(FldTipo); }
            set { insertNameValueField(FldTipo, value); }
        }


        /// <summary>Field : "ZZSTATE" Type: "INT" Formula:  ""</summary>
        public static FieldRef FldZzstate { get { return m_fldZzstate; } }
        private static FieldRef m_fldZzstate = new FieldRef("lstren", "zzstate");



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
        public static CSGenioAlstren search(PersistentSupport sp, string key, User user, string[] fields = null)
        {
			if (string.IsNullOrEmpty(key)) //to proteger chamadas "cegas"
				return null;
		    CSGenioAlstren area = new CSGenioAlstren(user, user.CurrentModule);
            if(sp.getRecord(area, key, fields))
                return area;
			return null;
        }

        /// <summary>
        /// Procura todos os registos desta area que obedecem a uma condi��o
        /// </summary>
        /// <param name="sp">O suporte persistente de onde obter a lista</param>
        /// <param name="utilizador">O contexto do user</param>
        /// <param name="where">A condi��o de procura dos registos. Usar null to obter todos os registos</param>
        /// <returns>Uma lista de registos da areas com todos os fields preenchidos</returns>
        [Obsolete("Use List<CSGenioAlstren> searchList(PersistentSupport sp, User utilizador, CriteriaSet where) instead")]
        public static List<CSGenioAlstren> searchList(PersistentSupport sp, User user, string where)
        {
            return sp.searchListWhere<CSGenioAlstren>(where, user, null);
        }

        /// <summary>
        /// Procura todos os registos desta area que obedecem a uma condi��o
        /// </summary>
        /// <param name="sp">O suporte persistente de onde obter a lista</param>
        /// <param name="utilizador">O contexto do user</param>
        /// <param name="where">A condi��o de procura dos registos. Usar null to obter todos os registos</param>
        /// <returns>Uma lista de registos da areas com todos os fields preenchidos</returns>
        public static List<CSGenioAlstren> searchList(PersistentSupport sp, User user, CriteriaSet where)
        {
            return searchList(sp, user, where, null);
        }
       
        /// <summary>
        /// Procura todos os registos desta area que obedecem a uma condi��o
        /// </summary>
        /// <param name="sp">O suporte persistente de onde obter a lista</param>
        /// <param name="utilizador">O contexto do user</param>
        /// <param name="where">A condi��o de procura dos registos. Usar null to obter todos os registos</param>
        /// <param name="campos">Os fields a serem preenchidos na area</param>
        /// <returns>Uma lista de registos da areas com todos os fields preenchidos</returns>
        /// <remarks>N�o devem ser utilizadas opera��es de persistence sobre um registo parcialmente posicionado</remarks>
        [Obsolete("Use List<CSGenioAlstren> searchList(PersistentSupport sp, User utilizador, CriteriaSet where, string []campos) instead")]
        public static List<CSGenioAlstren> searchList(PersistentSupport sp, User user, string where, string []fields)
        {
            return sp.searchListWhere<CSGenioAlstren>(where, user, fields);
        }
       
        /// <summary>
        /// Procura todos os registos desta area que obedecem a uma condi��o
        /// </summary>
        /// <param name="sp">O suporte persistente de onde obter a lista</param>
        /// <param name="utilizador">O contexto do user</param>
        /// <param name="where">A condi��o de procura dos registos. Usar null to obter todos os registos</param>
        /// <param name="campos">Os fields a serem preenchidos na area</param>
        /// <returns>Uma lista de registos da areas com todos os fields preenchidos</returns>
        /// <remarks>N�o devem ser utilizadas opera��es de persistence sobre um registo parcialmente posicionado</remarks>
        public static List<CSGenioAlstren> searchList(PersistentSupport sp, User user, CriteriaSet where, string []fields)
        {
            return sp.searchListWhere<CSGenioAlstren>(where, user, fields);
        }

        /// <summary>
        /// Procura todos os registos desta area que obedecem a uma condi��o
        /// </summary>
        /// <param name="sp">O suporte persistente de onde obter a lista</param>
        /// <param name="utilizador">O contexto do user</param>
        /// <param name="where">A condi��o de procura dos registos. Usar null to obter todos os registos</param>
        /// <param name="campos">Os fields a serem preenchidos na area</param>
        /// <param name="distinct">Obter distinct de fields</param>
        /// <param name="noLock">NOLOCK</param>
        /// <returns>Uma lista de registos da areas com todos os fields preenchidos</returns>
        /// <remarks>N�o devem ser utilizadas opera��es de persistence sobre um registo parcialmente posicionado</remarks>
        public static List<CSGenioAlstren> searchList(PersistentSupport sp, User user, CriteriaSet where, string[] fields, bool distinct, bool noLock = false)
        {
			return sp.searchListWhere<CSGenioAlstren>(where, user, fields, distinct, noLock);
        }

       	/// <summary>
        /// Procura todos os registos desta area que obedecem a uma condi��o
        /// </summary>
        /// <param name="sp">O suporte persistente de onde obter a lista</param>
        /// <param name="utilizador">O contexto do user</param>
        /// <param name="where">A condi��o de procura dos registos. Usar null to obter todos os registos</param>
        /// <param name="campos">Os fields a serem preenchidos na area</param>
		/// <param name="distinct">Obter distinct de fields</param>
        /// <returns>Uma lista de registos da areas com todos os fields preenchidos</returns>
        /// <remarks>N�o devem ser utilizadas opera��es de persistence sobre um registo parcialmente posicionado</remarks>
        public static void searchListAdvancedWhere(PersistentSupport sp, User user, CriteriaSet where, ListingMVC<CSGenioAlstren> listing)
        {
			sp.searchListAdvancedWhere<CSGenioAlstren>(where, listing);			
        }

        /// <summary>
        /// Procura todos os registos desta area que obedecem a uma condi��o
        /// </summary>
        /// <param name="sp">O suporte persistente de onde obter a lista</param>
        /// <param name="utilizador">O contexto do user</param>
        /// <param name="where">A condi��o de procura dos registos. Usar null to obter todos os registos</param>
        /// <param name="distinct">Obter distinct de fields</param>
        /// <param name="noLock">NOLOCK</param>
        /// <returns>Uma lista de registos da areas com todos os fields preenchidos</returns>
        public static List<CSGenioAlstren> searchList(PersistentSupport sp, User user, CriteriaSet where, bool distinct, bool noLock = false)
        {
            return searchList(sp, user, where, null, distinct, noLock);
        }

	}
}
