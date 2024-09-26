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
	public class CSGenioAlstcol : DbArea
	{
	    /// <summary>
		/// Meta-informação sobre esta àrea
		/// </summary>
		protected static AreaInfo informacao = InicializaAreaInfo();

		public CSGenioAlstcol(User user,string module)
		{
			fields = new Hashtable();
            this.user = user;
            this.module = module;
		}
	
		public CSGenioAlstcol(User user) : this(user, user.CurrentModule)
		{
		}
	
		private static AreaInfo InicializaAreaInfo()
		{
			AreaInfo info = new AreaInfo();
			
			/*Information das areas*/
			info.TableName = "PROlstcol";
			info.ShadowTabName = "";
			info.PrimaryKeyName = "codlstcol";
            info.HumanKeyName = "tabela";
			info.ShadowTabKeyName = "";
			info.Alias = "lstcol";
			info.IsDomain =  false;
			info.AreaDesignation = "Configuração de coluna";
			info.AreaPluralDesignation = "Configuração de colunas";
			info.DescriptionCav = "Configuração de coluna";
			
			//sincronização
			info.SyncIncrementalDateStart = TimeSpan.FromHours(9.0);
			info.SyncIncrementalDateEnd = TimeSpan.FromHours(23.0);
			info.SyncCompleteHour = TimeSpan.FromHours(1.0);
			info.SyncIncrementalPeriod = TimeSpan.FromHours(1);
			info.BatchSync = 100;
			info.SyncType = SyncType.Central;
					
			info.RegisterFieldDB(new Field("codlstcol", FieldType.CHAVE_PRIMARIA));
			info.DBFields["codlstcol"].FieldSize = 6;
			info.KeyType = CodeType.INT_KEY;
			info.RegisterFieldDB(new Field("codlstusr", FieldType.CHAVE_ESTRANGEIRA));
			info.DBFields["codlstusr"].FieldSize = 6;

			info.RegisterFieldDB(new Field("tabela", FieldType.TEXTO));
			info.RegisterFieldDB(new Field("alias", FieldType.TEXTO));
            info.RegisterFieldDB(new Field("campo", FieldType.TEXTO));
            info.RegisterFieldDB(new Field("visivel", FieldType.LOGICO));
            info.RegisterFieldDB(new Field("posicao", FieldType.NUMERO));
            info.RegisterFieldDB(new Field("operacao", FieldType.NUMERO));
            info.RegisterFieldDB(new Field("tipo", FieldType.NUMERO));
            info.RegisterFieldDB(new Field("zzstate", FieldType.INTEIRO));


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

		// USE /[MANUAL PRO TABAUX lstcol]/

		
        public string FormMode { get; set; }
        public string ResultMsg { get; set; }

        public static FieldRef FldCodlstcol { get { return m_fldCodlstcol; } }
        private static FieldRef m_fldCodlstcol = new FieldRef("lstcol", "codlstcol");

        /// <summary>Field : "" Tipo: "+" Formula:  ""</summary>
        public string ValCodlstcol
        {
            get { return (string)returnValueField(FldCodlstcol); }
            set { insertNameValueField(FldCodlstcol, value); }
        }


        /// <summary>Field : "" Tipo: "CE" Formula:  ""</summary>
        public static FieldRef FldCodlstusr { get { return m_fldCodlstusr; } }
        private static FieldRef m_fldCodlstusr = new FieldRef("lstcol", "codlstusr");

        /// <summary>Field : "" Tipo: "CE" Formula:  ""</summary>
        public string ValCodlstusr
        {
            get { return (string)returnValueField(FldCodlstusr); }
            set { insertNameValueField(FldCodlstusr, value); }
        }


        /// <summary>Field : "Tabela" Tipo: "C" Formula:  ""</summary>
        public static FieldRef FldTabela { get { return m_fldTabela; } }
        private static FieldRef m_fldTabela = new FieldRef("lstcol", "tabela");

        /// <summary>Field : "Tabela" Tipo: "C" Formula:  ""</summary>
        public string ValTabela
        {
            get { return (string)returnValueField(FldTabela); }
            set { insertNameValueField(FldTabela, value); }
        }


        /// <summary>Field : "Alias" Tipo: "C" Formula:  ""</summary>
        public static FieldRef FldAlias { get { return m_fldAlias; } }
        private static FieldRef m_fldAlias = new FieldRef("lstcol", "alias");

        /// <summary>Field : "Alias" Tipo: "C" Formula:  ""</summary>
        public string ValAlias
        {
            get { return (string)returnValueField(FldAlias); }
            set { insertNameValueField(FldAlias, value); }
        }


        /// <summary>Field : "Campo" Tipo: "C" Formula:  ""</summary>
        public static FieldRef FldCampo { get { return m_fldCampo; } }
        private static FieldRef m_fldCampo = new FieldRef("lstcol", "campo");

        /// <summary>Field : "Campo" Tipo: "C" Formula:  ""</summary>
        public string ValCampo
        {
            get { return (string)returnValueField(FldCampo); }
            set { insertNameValueField(FldCampo, value); }
        }


        /// <summary>Field : "Visível" Tipo: "L" Formula:  ""</summary>
        public static FieldRef FldVisivel { get { return m_fldVisivel; } }
        private static FieldRef m_fldVisivel = new FieldRef("lstcol", "visivel");

        /// <summary>Field : "Visível" Tipo: "L" Formula:  ""</summary>
        public int ValVisivel
        {
            get { return (int)returnValueField(FldVisivel); }
            set { insertNameValueField(FldVisivel, value); }
        }


        /// <summary>Field : "Posição" Tipo: "N" Formula:  ""</summary>
        public static FieldRef FldPosicao { get { return m_fldPosicao; } }
        private static FieldRef m_fldPosicao = new FieldRef("lstcol", "posicao");

        /// <summary>Field : "Posição" Tipo: "N" Formula:  ""</summary>
        public decimal ValPosicao
        {
            get { return (decimal)returnValueField(FldPosicao); }
            set { insertNameValueField(FldPosicao, value); }
        }


        /// <summary>Field : "operação" Tipo: "N" Formula:  ""</summary>
        public static FieldRef FldOperacao { get { return m_fldOperacao; } }
        private static FieldRef m_fldOperacao = new FieldRef("lstcol", "operacao");

        /// <summary>Field : "operação" Tipo: "N" Formula:  ""</summary>
        public decimal ValOperacao
        {
            get { return (decimal)returnValueField(FldOperacao); }
            set { insertNameValueField(FldOperacao, value); }
        }


        /// <summary>Field : "Tipo" Tipo: "N" Formula:  ""</summary>
        public static FieldRef FldTipo { get { return m_fldTipo; } }
        private static FieldRef m_fldTipo = new FieldRef("lstcol", "tipo");

        /// <summary>Field : "Tipo" Tipo: "N" Formula:  ""</summary>
        public decimal ValTipo
        {
            get { return (decimal)returnValueField(FldTipo); }
            set { insertNameValueField(FldTipo, value); }
        }


        /// <summary>Field : "ZZSTATE" Type: "INT" Formula:  ""</summary>
        public static FieldRef FldZzstate { get { return m_fldZzstate; } }
        private static FieldRef m_fldZzstate = new FieldRef("lstcol", "zzstate");



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
        public static CSGenioAlstcol search(PersistentSupport sp, string key, User user, string[] fields = null)
        {
			if (string.IsNullOrEmpty(key)) //to proteger chamadas "cegas"
				return null;
		    CSGenioAlstcol area = new CSGenioAlstcol(user, user.CurrentModule);
            if(sp.getRecord(area, key, fields))
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
        [Obsolete("Use List<CSGenioAlstcol> searchList(PersistentSupport sp, User utilizador, CriteriaSet where) instead")]
        public static List<CSGenioAlstcol> searchList(PersistentSupport sp, User user, string where)
        {
            return sp.searchListWhere<CSGenioAlstcol>(where, user, null);
        }

        /// <summary>
        /// Procura todos os registos desta area que obedecem a uma condição
        /// </summary>
        /// <param name="sp">O suporte persistente de onde obter a lista</param>
        /// <param name="utilizador">O contexto do user</param>
        /// <param name="where">A condição de procura dos registos. Usar null to obter todos os registos</param>
        /// <returns>Uma lista de registos da areas com todos os fields preenchidos</returns>
        public static List<CSGenioAlstcol> searchList(PersistentSupport sp, User user, CriteriaSet where)
        {
            return searchList(sp, user, where, null);
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
        [Obsolete("Use List<CSGenioAlstcol> searchList(PersistentSupport sp, User utilizador, CriteriaSet where, string []campos) instead")]
        public static List<CSGenioAlstcol> searchList(PersistentSupport sp, User user, string where, string []fields)
        {
            return sp.searchListWhere<CSGenioAlstcol>(where, user, fields);
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
        public static List<CSGenioAlstcol> searchList(PersistentSupport sp, User user, CriteriaSet where, string []fields)
        {
            return sp.searchListWhere<CSGenioAlstcol>(where, user, fields);
        }

        /// <summary>
        /// Procura todos os registos desta area que obedecem a uma condição
        /// </summary>
        /// <param name="sp">O suporte persistente de onde obter a lista</param>
        /// <param name="utilizador">O contexto do user</param>
        /// <param name="where">A condição de procura dos registos. Usar null to obter todos os registos</param>
        /// <param name="campos">Os fields a serem preenchidos na area</param>
        /// <param name="distinct">Obter distinct de fields</param>
        /// <param name="noLock">NOLOCK</param>
        /// <returns>Uma lista de registos da areas com todos os fields preenchidos</returns>
        /// <remarks>Não devem ser utilizadas operações de persistence sobre um registo parcialmente posicionado</remarks>
        public static List<CSGenioAlstcol> searchList(PersistentSupport sp, User user, CriteriaSet where, string[] fields, bool distinct, bool noLock = false)
        {
			return sp.searchListWhere<CSGenioAlstcol>(where, user, fields, distinct, noLock);
        }

       	/// <summary>
        /// Procura todos os registos desta area que obedecem a uma condição
        /// </summary>
        /// <param name="sp">O suporte persistente de onde obter a lista</param>
        /// <param name="utilizador">O contexto do user</param>
        /// <param name="where">A condição de procura dos registos. Usar null to obter todos os registos</param>
        /// <param name="campos">Os fields a serem preenchidos na area</param>
		/// <param name="distinct">Obter distinct de fields</param>
        /// <returns>Uma lista de registos da areas com todos os fields preenchidos</returns>
        /// <remarks>Não devem ser utilizadas operações de persistence sobre um registo parcialmente posicionado</remarks>
        public static void searchListAdvancedWhere(PersistentSupport sp, User user, CriteriaSet where, ListingMVC<CSGenioAlstcol> listing)
        {
			sp.searchListAdvancedWhere<CSGenioAlstcol>(where, listing);			
        }

        /// <summary>
        /// Procura todos os registos desta area que obedecem a uma condição
        /// </summary>
        /// <param name="sp">O suporte persistente de onde obter a lista</param>
        /// <param name="utilizador">O contexto do user</param>
        /// <param name="where">A condição de procura dos registos. Usar null to obter todos os registos</param>
        /// <param name="distinct">Obter distinct de fields</param>
        /// <param name="noLock">NOLOCK</param>
        /// <returns>Uma lista de registos da areas com todos os fields preenchidos</returns>
        public static List<CSGenioAlstcol> searchList(PersistentSupport sp, User user, CriteriaSet where, bool distinct, bool noLock = false)
        {
            return searchList(sp, user, where, null, distinct, noLock);
        }

	}
}
