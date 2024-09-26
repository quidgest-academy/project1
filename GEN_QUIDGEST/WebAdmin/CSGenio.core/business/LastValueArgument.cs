using System;
using System.Collections;
using System.Diagnostics;
using System.Text;
using CSGenio.framework;
using CSGenio.persistence;
using Quidgest.Persistence.GenericQuery;

namespace CSGenio.business
{
    /// <summary>
    /// Argumento de relacao de ultimo Qvalue
    /// </summary>
    public class LastValueArgument
    {
        private string aliasRUV;//table que tem a relação de último Qvalue      
        private string[] camposRUV;//Qfield que são fórmulas relação de último Qvalue
        private string[] camposConsultados;//fields que são argumento da relação de último Qvalue
        private string campoDataConsultada;//Qfield data que limita
        private string campoDataEncerramento;//Qfield data adicional que serve de encerramento
        private bool encerramentoIsToday;//Can be checked to see if the closing value is Today
        private CriteriaSet condition;//condição to filtrar o preenchimento do último Qvalue
        
        /// <summary>
        /// Constructor da classe
        /// </summary>
        /// <param name="aliasRUV">table que tem a relação de último Qvalue</param>
        /// <param name="camposRUV">fields que são fórmulas das relações de último Qvalue</param>
        /// <param name="camposConsultados">fields argumentos das relações de último Qvalue</param>
        /// <param name="campoDataConsultada">Qfield data da table consultada</param>
        public LastValueArgument(string aliasRUV, string[] camposRUV, string[] camposConsultados, string campoDataConsultada, CriteriaSet condition, string campoDataEncerramento=null, bool encerramentoIsToday=false)
        {
            this.aliasRUV = aliasRUV;
            this.camposRUV = camposRUV;
            this.camposConsultados = camposConsultados;
            this.campoDataConsultada = campoDataConsultada;
            this.condition = condition;
            this.campoDataEncerramento = campoDataEncerramento;
            this.encerramentoIsToday = encerramentoIsToday;
        }

		public ArrayList ReadLvr(PersistentSupport sp, IArea areaConsulted, string relSourceField, object relationValue, FieldFormatting formattingRelationalField, FieldFormatting formattingSortingField, string TargetRelField = null)
        {
            return ReadLvr(sp, areaConsulted.QSystem, areaConsulted.TableName, relSourceField, relationValue, formattingRelationalField, formattingSortingField, TargetRelField);
        }

        /// <summary>
        /// Faz uma query to ler os ultimos Qvalues actuais
        /// </summary>
        /// <param name="sp"></param>
        /// <param name="tabelaConsultada"></param>
        /// <param name="campoRelOrigem"></param>
        /// <param name="valorRelacao"></param>
        /// <param name="formatacaoCampoRelacao"></param>
        /// <param name="formatacaoCampoConsultado"></param>
        /// <returns>Os Qvalues dos fields a consultar do último Qvalue actual to a relação fornecida. Caso não exista registo o array estará vazio.</returns>
        [Obsolete("Please use 'ArrayList LerRuv(PersistentSupport sp, IArea areaConsultada, string campoRelOrigem, object valorRelacao, FieldFormatting formatacaoCampoRelacao, FieldFormatting formatacaoCampoOrdenacao)' instead")]
        public ArrayList ReadLvr(PersistentSupport sp, string consultedTable, string relSourceField, object relationValue, FieldFormatting formattingRelationalField, FieldFormatting formattingSortingField)
        {
            return ReadLvr(sp, null, consultedTable, relSourceField, relationValue, formattingRelationalField, formattingSortingField);
        }

        private ArrayList ReadLvr(PersistentSupport sp, string schemaConsultado, string consultedTable, string relSourceField, object relationValue, FieldFormatting formattingRelationalField, FieldFormatting formattingSortingField, string TargetRelField = null)
        {
            Debug.Assert(sp != null);
            Debug.Assert(consultedTable != null);
            Debug.Assert(relSourceField != null);
            Debug.Assert(relationValue != null);

            SelectQuery select = new SelectQuery();
            foreach (string field in camposConsultados)
            {
                select.Select(consultedTable, field);
            }
            //SO 20061211 alteração do constructor QuerySelect           

            //Make inner join with relation table
            if(campoDataEncerramento != null && TargetRelField != null && !encerramentoIsToday)
            {
                select.From(schemaConsultado, consultedTable, consultedTable)

                    .Join(schemaConsultado.ToLower() + campoDataEncerramento.Split('.')[0])
                    .On(CriteriaSet.And().Equal(consultedTable, relSourceField, schemaConsultado.ToLower() + campoDataEncerramento.Split('.')[0], TargetRelField))

                    .Where(CriteriaSet.And()
                    .Equal(consultedTable, "zzstate", 0)
                    .Equal(consultedTable, relSourceField, relationValue));
            }
            else
            {
                select.From(schemaConsultado, consultedTable, consultedTable)
                    .Where(CriteriaSet.And()
                    .Equal(consultedTable, "zzstate", 0)
                    .Equal(consultedTable, relSourceField, relationValue));
            }
            

            if (formattingSortingField.Equals(FieldFormatting.DATA) || formattingSortingField.Equals(FieldFormatting.DATAHORA) || formattingSortingField.Equals(FieldFormatting.DATASEGUNDO) )
            {
                if (encerramentoIsToday)
                {
                    select.WhereCondition.LesserOrEqual(consultedTable, campoDataConsultada, DateTime.Now);
                }
                else
                {
                    if(campoDataEncerramento != null && TargetRelField != null)
                    {
                        select.WhereCondition.LesserOrEqual(consultedTable, campoDataConsultada, schemaConsultado.ToLower() + campoDataEncerramento.Split('.')[0], campoDataEncerramento.Split('.')[1]);
                    }
                }
                
                select.OrderBy(consultedTable, campoDataConsultada, SortOrder.Descending);
            }
            else if (formattingSortingField.Equals(FieldFormatting.CARACTERES) || formattingSortingField.Equals(FieldFormatting.TEMPO))
            {
                select.OrderBy(SqlFunctions.Upper(new ColumnReference(consultedTable, campoDataConsultada)), SortOrder.Descending);
            }
            else if (formattingSortingField.Equals(FieldFormatting.FLOAT) ||
                formattingSortingField.Equals(FieldFormatting.LOGICO) ||
                formattingSortingField.Equals(FieldFormatting.INTEIRO))
            {
                select.OrderBy(consultedTable, campoDataConsultada, SortOrder.Descending);
            }
            else
            {
                throw new BusinessException("O campo " + campoDataConsultada + " não pode ser ordenado.", "LastValueArgument.LerRuv", "The field " + campoDataConsultada + " can't be ordered.");
            }
            select.PageSize(1);
            //acrescentar a condição do filtro do último Qvalue
            select.WhereCondition.SubSet(condition);

            return sp.executeReaderOneRow(select);
        }

        /// <summary>
        /// Alias da table que tem a relação de último Qvalue
        /// </summary>
        public string AliasRUV
        {
            get { return aliasRUV; }
        }

        /// <summary>
        /// Field que é fórmula de último Qvalue
        /// </summary>
        public string[] LVRFields
        {
            get { return camposRUV; }
        }

        /// <summary>
        /// Field argumento da relação de último Qvalue
        /// </summary>
        public string[] ConsultedFields
        {
            get { return camposConsultados; }
        }

        /// <summary>
        /// Field data da table consultada
        /// </summary>
        public string ConsultedDateFields
        {
            get { return campoDataConsultada; }
        }

        /// <summary>
        /// Field adicional, não obrigatório que serve de encerramento
        /// </summary>
        public string EndDateField
        {
            get { return campoDataEncerramento; }
        }

    }
}
