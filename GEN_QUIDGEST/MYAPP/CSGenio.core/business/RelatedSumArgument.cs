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
    public class RelatedSumArgument : Formula
    {
        private string aliasOrigem;//alias da table do Qfield que é somado
        private string aliasSR;//alias da table do Qfield que é soma relacionada
        private string campoSR;//Qfield que é soma relacionada
        private string campoArg;//Qfield argumento da soma relacionada
        private char sign;//sign do Qfield
        private bool isCampo;//se é Qfield, caso contrário é um number

        /// <summary>
        /// Constructor da classe
        /// </summary>
        /// <param name="aliasSR">alias da table do Qfield que é soma relacionada</param>
        /// <param name="campoSR">Qfield que é soma relacionada</param>
        /// <param name="sinal">sign do Qfield</param>
        /// <param name="isCampo">se é Qfield ou inteiro</param>
        public RelatedSumArgument(string aliasOrigem, string aliasSR, string campoSR, string campoArg, char sign, bool isCampo)
        {
            this.aliasOrigem = aliasOrigem;
            this.aliasSR = aliasSR;
            this.campoSR = campoSR;
            this.campoArg = campoArg;
            this.sign = sign;
            this.isCampo = isCampo;

        }

        public string AliasSource
        {
            get { return aliasOrigem; }
            set { aliasOrigem = value; }
        }

        public string AliasSR
        {
            get { return aliasSR; }
            set { aliasSR = value; }
        }

        public string SRField
        {
            get { return campoSR; }
            set { campoSR = value; }
        }

        public string ArgField
        {
            get { return campoArg; }
            set { campoArg = value; }
        }

        public char Signal
        {
            get { return sign; }
            set { sign = value; }
        }

        public bool IsField
        {
            get { return isCampo; }
            set { isCampo = value; }
        }

    }

}
