using System;
using System.Collections;
using System.Data.SqlTypes;
using System.Text;
using CSGenio.framework;
using CSGenio.persistence;
using Quidgest.Persistence.GenericQuery;

namespace CSGenio.business
{
    /// <summary>
    /// Descreve os tipos poss�veis de f�rmulas internas.
    /// </summary>
    public class EndPeriodFormula : Formula
    {
        private string campoData;//Qfield que marca o start de periodo
        private string campoAgrupar;//Qfield que permite agrupar

        /// <summary>
        /// Constructor da classe
        /// </summary>
        /// <param name="campoData">recebe o Qfield data</param>
        /// <param name="campoAgrupar">recebe o Qfield que agrupa</param>
        public EndPeriodFormula(string campoData, string campoAgrupar)
        {
            this.campoData = campoData;
            this.campoAgrupar = campoAgrupar;
        }

        /// <summary>
        /// Decrementa o Qvalue de fim de periodo de acordo com a formata��o
        /// </summary>
        /// <param name="valor">O Qvalue to o in�cio do periodo</param>
        /// <param name="formatacao">A formata��o do Qvalue</param>
        /// <returns>O Qvalue to o fecho do periodo</returns>
        private object DecFimPeriodo(object Qvalue, FieldFormatting formatting)
        {
            if (Field.isEmptyValue(Qvalue, formatting))
                return Field.GetValorEmpty(formatting);

            //TODO: Usar o FieldType e n�o o FieldFormatting, alias, s� devia existir sempre o FieldType
            switch (formatting)
            {
                case FieldFormatting.DATA:
                case FieldFormatting.ANO_MES_DIA:
                case FieldFormatting.DIA_MES_ANO:
                    return ((DateTime)Qvalue).AddDays(-1);
                case FieldFormatting.INTEIRO:
                    return ((int)Qvalue) - 1;
                case FieldFormatting.DATAHORA:
                    return ((DateTime)Qvalue).AddMinutes(-1);
                case FieldFormatting.DATASEGUNDO:
                    return ((DateTime)Qvalue).AddSeconds(-1);
                case FieldFormatting.FLOAT:
                    return ((decimal)Qvalue) - 1;
                case FieldFormatting.TEMPO:
                    return HourFunctions.HoursAdd(Qvalue as string, -1);
                default:
                    return Qvalue;
            }
        }

        /// <summary>
        /// Propriedade to fazer get e set do Qfield data
        /// </summary>
        public string DateField
        {
            get { return campoData; }
            set { campoData = value; }
        }

        /// <summary>
        /// Propriedade to fazer get e set do Qfield que serve to agrupar
        /// </summary>
        public string GroupField
        {
            get { return campoAgrupar; }
            set { campoAgrupar = value; }
        }

        public object readEndPeriod(PersistentSupport sp, Area area, object start, object grouping, object keyValue)
        {
            SelectQuery querySelect = new SelectQuery()
				.Select(area.Alias, campoData)
                .From(area.QSystem, area.TableName, area.Alias)
				.Where(CriteriaSet.And()
					.NotEqual(area.Alias, area.PrimaryKeyName, keyValue)
					.Greater(area.Alias, campoData, start));
            if (campoAgrupar != null)
            {
                querySelect.WhereCondition.Equal(area.Alias, campoAgrupar, grouping);
            }
            querySelect.OrderBy(area.Alias, campoData, SortOrder.Ascending).PageSize(1);

            object result = sp.ExecuteScalar(querySelect);
            FieldFormatting formData = area.returnFormattingDBField(campoData);
            result = DBConversion.ToInternal(result, formData);
            result = DecFimPeriodo(result, formData);
            return result;
        }

        public string getPreviousRecord(PersistentSupport sp, Area area, object start, object grouping)
        {
            SelectQuery querySelect = new SelectQuery()
				.Select(area.Alias, area.PrimaryKeyName)
				.From(area.QSystem, area.TableName, area.Alias)
				.Where(CriteriaSet.And()
					.Equal(area.Alias, "zzstate", 0)
					.Lesser(area.Alias, campoData, start));
            if (campoAgrupar != null)
            {
                querySelect.WhereCondition.Equal(area.Alias, campoAgrupar, grouping);
            }
            querySelect.OrderBy(area.Alias, campoData, SortOrder.Descending).PageSize(1);

            object result = sp.ExecuteScalar(querySelect);
            return DBConversion.ToKey(result);
        }
    }
}
