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
	public class CSGenioAnotificationemailsignature : DbArea
	{
	    /// <summary>
		/// Meta-informa��o sobre esta �rea
		/// </summary>
		protected static AreaInfo informacao = InicializaAreaInfo();

		public CSGenioAnotificationemailsignature(User user,string module)
		{
			fields = new Hashtable();
            this.user = user;
            this.module = module;
		}
	
		public CSGenioAnotificationemailsignature(User user) : this(user, user.CurrentModule)
		{
		}
	
		private static AreaInfo InicializaAreaInfo()
		{
			AreaInfo info = new AreaInfo();
			
			/*Information das areas*/
			info.TableName = "notificationemailsignature";
			info.ShadowTabName = "";
			info.PrimaryKeyName = "codsigna";
            info.HumanKeyName = "name";
			info.ShadowTabKeyName = "";
			info.Alias = "notificationemailsignature";
			info.IsDomain =  true;
			info.AreaDesignation = "Assinatura";
			info.AreaPluralDesignation = "Assinaturas";
			info.DescriptionCav = "Assinatura";
			
			//sincroniza��o
			info.SyncIncrementalDateStart = TimeSpan.FromHours(9.0);
			info.SyncIncrementalDateEnd = TimeSpan.FromHours(23.0);
			info.SyncCompleteHour = TimeSpan.FromHours(1.0);
			info.SyncIncrementalPeriod = TimeSpan.FromHours(1);
			info.BatchSync = 100;
			info.SyncType = SyncType.Central;
					
			info.RegisterFieldDB(new Field("codsigna", FieldType.CHAVE_PRIMARIA));
			info.DBFields["codsigna"].FieldSize = 6;

			info.KeyType = CodeType.INT_KEY;
			info.RegisterFieldDB(new Field("name", FieldType.TEXTO));
			info.RegisterFieldDB(new Field("image", FieldType.IMAGEM_JPEG));
			info.RegisterFieldDB(new Field("textass", FieldType.TEXTO));
			
			info.RegisterFieldDB(new Field("opercria", FieldType.OPERCRIA));
			info.RegisterFieldDB(new Field("datacria", FieldType.DATACRIA));
            info.RegisterFieldDB(new Field("opermuda", FieldType.OPERMUDA));
            info.RegisterFieldDB(new Field("datamuda", FieldType.DATAMUDA));
			info.RegisterFieldDB(new Field("zzstate", FieldType.INTEIRO));
			
            // Carimbos autom�ticos na BD
            //------------------------------
   			info.StampFieldsIns = new string[] {
			 "opercria","datacria"
			};
  			info.StampFieldsIns = new string[] {
			 "opermuda","datamuda"
			};

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

		// USE /[MANUAL PRO TABAUX notificationemailsignature]/

		
        public string FormMode { get; set; }
        public string ResultMsg { get; set; }

        ////////////////////////////////////////////////////////////////////////////BD fields CONVERTED TO Areainfo style
        /// <summary>Campo : "PK da tabela notificationemailsignature" Tipo: "+" Formula:  ""</summary>
        public static FieldRef FldCodsigna { get { return m_fldCodsigna; } }
        private static FieldRef m_fldCodsigna = new FieldRef("notificationemailsignature", "codsigna");

        /// <summary>Campo : "PK da tabela notificationemailsignatures" Tipo: "+" Formula:  ""</summary>
        public string ValCodsigna
        {
            get { return (string)returnValueField(FldCodsigna); }
            set { insertNameValueField(FldCodsigna, value); }
        }

		/// <summary>Campo : "Nome da assinatura" Tipo: "C" Formula:  ""</summary>
        public static FieldRef FldName { get { return m_fldName; } }
        private static FieldRef m_fldName = new FieldRef("notificationemailsignature", "name");

        /// <summary>Campo : "Nome do assinatura" Tipo: "C" Formula:  ""</summary>
        public string ValName
        {
            get { return (string)returnValueField(FldName); }
            set { insertNameValueField(FldName, value); }
        }
		
        /// <summary>Campo : "Imagem" Tipo: "IJ" Formula:  ""</summary>
        public static FieldRef FldImage { get { return m_fldImage; } }
        private static FieldRef m_fldImage = new FieldRef("notificationemailsignature", "image");

        /// <summary>Campo : "Imagem" Tipo: "IJ" Formula:  ""</summary>
        public byte[] ValImage
        {
            get { return (byte[])returnValueField(FldImage); }
            set { insertNameValueField(FldImage, value); }
        }

        /// <summary>Campo : "Texto da assinatura" Tipo: "C" Formula:  ""</summary>
        public static FieldRef FldTextass { get { return m_fldTextass; } }
        private static FieldRef m_fldTextass = new FieldRef("notificationemailsignature", "textass");

        /// <summary>Campo : "Texto da assinatura" Tipo: "C" Formula:  ""</summary>
        public string ValTextass
        {
            get { return (string)returnValueField(FldTextass); }
            set { insertNameValueField(FldTextass, value); }
        }

        /// <summary>Campo : "Altera��o: Data" Tipo: "ED" Formula:  ""</summary>
        public static FieldRef FldDatamuda { get { return m_fldDatamuda; } }
        private static FieldRef m_fldDatamuda = new FieldRef("notificationemailsignature", "datamuda");

        /// <summary>Campo : "Altera��o: Data" Tipo: "ED" Formula:  ""</summary>
        public DateTime ValDatamuda
        {
            get { return (DateTime)returnValueField(FldDatamuda); }
            set { insertNameValueField(FldDatamuda, value); }
        }


        /// <summary>Campo : "Cria��o: Data" Tipo: "OD" Formula:  ""</summary>
        public static FieldRef FldDatacria { get { return m_fldDatacria; } }
        private static FieldRef m_fldDatacria = new FieldRef("notificationemailsignature", "datacria");

        /// <summary>Campo : "Cria��o: Data" Tipo: "OD" Formula:  ""</summary>
        public DateTime ValDatacria
        {
            get { return (DateTime)returnValueField(FldDatacria); }
            set { insertNameValueField(FldDatacria, value); }
        }


        /// <summary>Campo : "Cria��o: Operador" Tipo: "ON" Formula:  ""</summary>
        public static FieldRef FldOpercria { get { return m_fldOpercria; } }
        private static FieldRef m_fldOpercria = new FieldRef("notificationemailsignature", "opercria");

        /// <summary>Campo : "Cria��o: Operador" Tipo: "ON" Formula:  ""</summary>
        public string ValOpercria
        {
            get { return (string)returnValueField(FldOpercria); }
            set { insertNameValueField(FldOpercria, value); }
        }


        /// <summary>Campo : "Altera��o: Operador" Tipo: "EN" Formula:  ""</summary>
        public static FieldRef FldOpermuda { get { return m_fldOpermuda; } }
        private static FieldRef m_fldOpermuda = new FieldRef("notificationemailsignature", "opermuda");

        /// <summary>Campo : "Altera��o: Operador" Tipo: "EN" Formula:  ""</summary>
        public string ValOpermuda
        {
            get { return (string)returnValueField(FldOpermuda); }
            set { insertNameValueField(FldOpermuda, value); }
        }


        /// <summary>Campo : "ZZSTATE" Tipo: "INT" Formula:  ""</summary>
        public static FieldRef FldZzstate { get { return m_fldZzstate; } }
        private static FieldRef m_fldZzstate = new FieldRef("notificationemailsignature", "zzstate");

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
        public static CSGenioAnotificationemailsignature search(PersistentSupport sp, string key, User user, string[] fields = null)
        {
            if (string.IsNullOrEmpty(key)) //para proteger chamadas "cegas"
                return null;
            CSGenioAnotificationemailsignature area = new CSGenioAnotificationemailsignature(user, user.CurrentModule);
            if (sp.getRecord(area, key, fields))
                return area;
            return null;
        }

        /// <summary>
        /// Procura todos os registos desta area que obedecem a uma condi��o
        /// </summary>
        /// <param name="sp">O suporte persistente de onde obter a lista</param>
        /// <param name="User">O contexto do User</param>
        /// <param name="where">A condi��o de procura dos registos. Usar null para obter todos os registos</param>
        /// <returns>Uma lista de registos da areas com todos os campos preenchidos</returns>
        public static List<CSGenioAnotificationemailsignature> searchList(PersistentSupport sp, User User, CriteriaSet where)
        {
            return searchList(sp, User, where, null);
        }
   
        /// <summary>
        /// Procura todos os registos desta area que obedecem a uma condi��o
        /// </summary>
        /// <param name="sp">O suporte persistente de onde obter a lista</param>
        /// <param name="User">O contexto do User</param>
        /// <param name="where">A condi��o de procura dos registos. Usar null para obter todos os registos</param>
        /// <param name="campos">Os campos a serem preenchidos na area</param>
        /// <returns>Uma lista de registos da areas com todos os campos preenchidos</returns>
        /// <remarks>N�o devem ser utilizadas opera��es de persistence sobre um registo parcialmente posicionado</remarks>
        public static List<CSGenioAnotificationemailsignature> searchList(PersistentSupport sp, User User, CriteriaSet where, string[] campos)
        {
            return sp.searchListWhere<CSGenioAnotificationemailsignature>(where, User, campos);
        }
		
        /// <summary>
        /// Procura todos os registos desta area que obedecem a uma condi��o
        /// </summary>
        /// <param name="sp">O suporte persistente de onde obter a lista</param>
        /// <param name="User">O contexto do User</param>
        /// <param name="where">A condi��o de procura dos registos. Usar null para obter todos os registos</param>
        /// <param name="campos">Os campos a serem preenchidos na area</param>
        /// <param name="distinct">Obter distinct de campos</param>
        /// <param name="noLock">NOLOCK</param>
        /// <returns>Uma lista de registos da areas com todos os campos preenchidos</returns>
        /// <remarks>N�o devem ser utilizadas opera��es de persistence sobre um registo parcialmente posicionado</remarks>
		public static void searchListAdvancedWhere(PersistentSupport sp, User User, CriteriaSet where, ListingMVC<CSGenioAnotificationemailsignature> listing)
        {
            sp.searchListAdvancedWhere<CSGenioAnotificationemailsignature>(where, listing);
        }
		
        /// <summary>
        /// Procura todos os registos desta area que obedecem a uma condi��o
        /// </summary>
        /// <param name="sp">O suporte persistente de onde obter a lista</param>
        /// <param name="User">O contexto do User</param>
        /// <param name="where">A condi��o de procura dos registos. Usar null para obter todos os registos</param>
        /// <param name="campos">Os campos a serem preenchidos na area</param>
        /// <param name="distinct">Obter distinct de campos</param>
        /// <param name="noLock">NOLOCK</param>
        /// <returns>Uma lista de registos da areas com todos os campos preenchidos</returns>
        /// <remarks>N�o devem ser utilizadas opera��es de persistence sobre um registo parcialmente posicionado</remarks>
        public static List<CSGenioAnotificationemailsignature> searchList(PersistentSupport sp, User User, CriteriaSet where, string[] campos, bool distinct, bool noLock = false)
        {
            return sp.searchListWhere<CSGenioAnotificationemailsignature>(where, User, campos, distinct, noLock);
        }

        /// <summary>
        /// Procura todos os registos desta area que obedecem a uma condi��o
        /// </summary>
        /// <param name="sp">O suporte persistente de onde obter a lista</param>
        /// <param name="User">O contexto do User</param>
        /// <param name="where">A condi��o de procura dos registos. Usar null para obter todos os registos</param>
        /// <param name="campos">Os campos a serem preenchidos na area</param>
        /// <param name="distinct">Obter distinct de campos</param>
        /// <returns>Uma lista de registos da areas com todos os campos preenchidos</returns>
        /// <remarks>N�o devem ser utilizadas opera��es de persistence sobre um registo parcialmente posicionado</remarks>
		public static List<CSGenioAnotificationemailsignature> searchList(PersistentSupport sp, User User, CriteriaSet where, bool distinct, bool noLock = false)
        {
            return searchList(sp, User, where, null, distinct, noLock);
        }

	}
}
