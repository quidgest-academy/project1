using System;
using System.Text;
using CSGenio.framework;
using CSGenio.persistence;
using Quidgest.Persistence.GenericQuery;

namespace CSGenio.business
{
    /// <summary>
    /// Esta classe representa a propriedade do Qfield ser argumento de soma relacionada
    /// </summary>
    public class ListAggregateArgument : Formula
    {
        private string aliasOrigem;//alias da table do Qfield que é somado
        private string aliasLG;//alias da table do Qfield que é soma relacionada
        private string campoLG;//Qfield que é a list aggregate
        private string campoArg;//Qfield argumento da list aggregate
        private string sortingField;//Qfield que vai ser feita a ordenação
        private string campoSeparador;//Qfield separador

        /// <summary>
        /// Constructor da classe
        /// </summary>
        /// <param name="aliasSR">alias da table do Qfield que é soma relacionada</param>
        /// <param name="campoSR">Qfield que é soma relacionada</param>
        /// <param name="sinal">sign do Qfield</param>
        /// <param name="isCampo">se é Qfield ou inteiro</param>
        public ListAggregateArgument(string aliasOrigem, string aliasLG, string campoLG, string campoArg, string sortingField, string campoSeparador)
        {
            this.aliasOrigem = aliasOrigem;
            this.aliasLG = aliasLG;
            this.campoLG = campoLG;
            this.campoArg = campoArg;
            this.sortingField = sortingField;
            this.campoSeparador = campoSeparador;
        }

        public string AliasSource
        {
            get { return aliasOrigem; }
            set { aliasOrigem = value; }
        }

        public string AliasLG
        {
            get { return aliasLG; }
            set { aliasLG = value; }
        }

        public string LGField
        {
            get { return campoLG; }
            set { campoLG = value; }
        }

        public string ArgField
        {
            get { return campoArg; }
            set { campoArg = value; }
        }

        public string SortField
        {
            get { return sortingField; }
            set { sortingField = value; }
        }

        public string SeparatorField
        {
            get { return campoSeparador; }
            set { campoSeparador = value; }
        }
    }
}
