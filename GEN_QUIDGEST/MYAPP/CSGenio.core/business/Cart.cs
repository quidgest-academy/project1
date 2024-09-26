using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using CSGenio.framework;
using CSGenio.business;
using CSGenio.persistence;
using Quidgest.Persistence.GenericQuery;

namespace CSGenio.business
{
    /// <summary>
    /// Classe abstracta que representa um carrinho de compras.
    /// </summary>
    public abstract class Cart
    {

        protected static Hashtable todosCarrinhos;
        
        //AV 2009/11/19 Criei a lista de nomes e Qvalues de fields to usar 
        //em vez da strimg de condição
        private static List<string> nomesCampos;
        private static List<string> fieldsvalues;
        
        static Cart()
        {
            nomesCampos = new List<string>();
            fieldsvalues = new List<string>();
            todosCarrinhos = new Hashtable();
        }

        public static Cart createCart(User user, string module, List<string> arrayFieldNames, List<string> arrayFieldValues)
        {
            nomesCampos = arrayFieldNames;
            fieldsvalues = arrayFieldValues;
            
            switch ((int)todosCarrinhos[module])
            {
                default:
                    throw new BusinessException(null, "CategoriesTree.criarArvore", "Error creating the Categories Tree: " + module);
            }
        }

        public abstract string Lines
        {
            get;
        }

        public abstract string Products
        {
            get;
        }

        public abstract string Aggregator
        {
            get;
        }

		[Obsolete("Use CriteriaSet CondicaoGeneric instead")]
        public abstract string Condition
        {
            get;
        }

        public abstract CriteriaSet GenericCondition
        {
            get;
        }

        /// <summary>
        /// Dados necessários to processar o pagamento do carrinho através do Pay Pal 
        /// (a implementação de outros modos de pagamento será reproduzida com novas propriedades assim podemos vir a ter uns DadosUnicre)
        /// </summary>
        public string PPData
        {
            get { return framework.Configuration.PP_Email + "[" + framework.Configuration.PP_Name; ; }
        }

        /// <summary>
        /// Module do programa que tem um carrinho de compras.
        /// </summary>
        public abstract string Module
        {
            get;
        }

        public abstract User User
        {
            get;
            set;
        }

        /// <summary>
        /// Metodo to obter um carrinho.
        /// </summary>
        /// <returns>string com o codigo interno</returns>
        public string GetAggregator(PersistentSupport sp)
        {
            //SO 2007.05.29 adicionei o module
            Area area = Area.createArea(Aggregator, User, Module);

            IList<ColumnSort> ordena = new List<ColumnSort>();

            //cada carrinho só pertence a um user
            if (!nomesCampos.Contains(area.Alias + ".opercria"))
            {
                nomesCampos.Add(area.Alias + ".opercria");
                fieldsvalues.Add(User.Name);
            }

            //string module = this.Module;

            //(RS 2010.10.27) deixei aqui o 'unico' a false to ficar exactamente com a semantica anterior
            //TODO: verificar se pode entrar a true to se poder retirar o if length = 1 daqui
            List<EPHOfArea> ephsDaArea = area.CalculateAreaEphs(User.Ephs, null, false); 
            foreach (EPHOfArea eph in ephsDaArea)
                if (eph.Relation != null)
                {
                    string crorigem = eph.Relation.SourceRelField;
                    //AreaInfo tabelaEPH = Area.GetInfoArea(ephsArea[j].Table);
                    if (eph.ValuesList.Length == 1)
                    {
                        nomesCampos.Add(area.Alias + "." + crorigem);
                        fieldsvalues.Add(eph.ValuesList[0]);
                    }
                }
                else
                {
                    nomesCampos.Add(area.Alias + "." + eph.Eph.Field);
                    fieldsvalues.Add(eph.ValuesList[0]);
                }

            if (GenericCondition == null || (GenericCondition.Criterias.Count == 0 && GenericCondition.SubSets.Count == 0)) //Neste caso criamos sempre um carrinho. Será que faz sentido???
            {
                Area Qresult = area.insertPseud(sp,nomesCampos.ToArray(), fieldsvalues.ToArray());
                return ((RequestedField)Qresult.Fields[area.Alias + "." + area.PrimaryKeyName]).Value.ToString();
            }
            else
            {
                try
                {
                    //buscar com as condições
                    CriteriaSet condicaoSeleccao = GenericCondition;
                    int ponto = area.Alias.Length;
                    for (int i = 0; i < nomesCampos.Count; i++)
                    {
                        FieldType fieldType = ((Field)area.DBFields[nomesCampos[i].Substring(ponto + 1)]).FieldType;
                        condicaoSeleccao.Equal(area.Alias, nomesCampos[i], fieldsvalues[i]);
                    }
                    string[] key = { area.Alias + "." + area.PrimaryKeyName };

                    area.insertNamesFields(key);

                    area.selectOne(condicaoSeleccao, ordena, "", sp);
                    return ((RequestedField)area.Fields[area.Alias + "." + area.PrimaryKeyName]).Value.ToString();
                }
                catch (BusinessException)
                {
                    ColumnReference nomeValorCampoAux = (ColumnReference)GenericCondition.Criterias[0].LeftTerm;
                    if (!nomesCampos.Contains(area.Alias + "." + nomeValorCampoAux.ColumnName))
                    {
                        nomesCampos.Add(nomeValorCampoAux.ColumnName);
                        fieldsvalues.Add(Convert.ToString(GenericCondition.Criterias[0].RightTerm));
                    }

                    Area Qresult = area.insertPseud(sp, nomesCampos.ToArray(), fieldsvalues.ToArray());                        

                    return ((RequestedField)Qresult.Fields[area.Alias + "." + area.PrimaryKeyName]).Value.ToString();
                }
            }
        }        
    }
}
