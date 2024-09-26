 using System;
using System.Collections;

namespace CSGenio.framework
{
	/// <summary>
	/// Summary description for EPH.
	/// </summary>
	/// 
	
	public class EPHCondition : ICloneable
	{
        //AV 20091229 Alterei a classe to permitir EPH em árvore, com múltiplos Qvalues e 
        //aplicadas a fields diferentes de chaves
		private string nomeEPH;
		private string sistemaTabela;//system da table relacionada com a table psw
		private string QtableName;//name da table relacionada com a table psw
		private string aliasTabela;//alias da table relacionada com a table psw
		private string relationalField;//Qfield que estabelece a relação
        private string tabelaEPH;//table da ficha a usar na EPH
        private string campoEPH;//Qfield com o Qvalue a usar na EPH
        private FieldType tipoCampoEPH;
		private string intialForm;

        public EPHCondition(string nomeEPH, string sistemaTabela, string QtableName, string aliasTabela, string relationalField, string tabelaEPH, string campoEPH, FieldType tipoCampoEPH, string intialForm)
        {
            this.nomeEPH = nomeEPH;
            this.sistemaTabela = sistemaTabela;
            this.QtableName = QtableName;
            this.aliasTabela = aliasTabela;
            this.relationalField = relationalField;
            this.tabelaEPH = tabelaEPH;
            this.campoEPH = campoEPH;
            this.tipoCampoEPH = tipoCampoEPH;
			this.intialForm = intialForm;
		}

		/// <summary>
		/// 
		/// </summary>
		public string TableSystem
		{
			get{return sistemaTabela;}
		}
		
		/// <summary>
		/// 
		/// </summary>
		public string TableName
		{
			get{return QtableName;}
		}
		
		/// <summary>
		/// 
		/// </summary>
		public string AliasTable
		{
			get{return aliasTabela;}
		}
		
		/// <summary>
		/// 
		/// </summary>
		public string RelationField
		{
			get{return relationalField;}
		}

		/// <summary>
		/// 
		/// </summary>
		public string EPHName
		{
			get{return nomeEPH;}
		}
		
        /// <summary>
        /// 
        /// </summary>
        public string EPHTable
        {
            get { return tabelaEPH; }
        }

        /// <summary>
        /// 
        /// </summary>
        public string EPHField
        {
            get { return campoEPH; }
        }

        /// <summary>
        /// 
        /// </summary>
        public FieldType EPHFieldType
        {
            get { return tipoCampoEPH; }
        }

		/// <summary>
        /// Get the Intial form set to EPH
        /// </summary>
        public string IntialForm
        {
            get { return intialForm; }
        }

        /// <summary>
        /// Clone EPHCondition
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
			return new EPHCondition(nomeEPH, sistemaTabela, QtableName, aliasTabela, relationalField, tabelaEPH, campoEPH, tipoCampoEPH, IntialForm);
        }
	}
}
