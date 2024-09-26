using System;
using System.Collections;
using System.Collections.Generic;
using CSGenio.framework;
using CSGenio.business;
using Quidgest.Persistence.GenericQuery;
using System.Linq;
using System.Text;

namespace CSGenio.persistence
{
    public static class QueryUtils
    {
		private static readonly Random random = new Random();
        private static readonly object syncLock = new object();

        public static T[] ToArray<T>(IEnumerable<T> enumerable)
        {
            if (enumerable == null)
            {
				throw new PersistenceException(null, "QueryUtils.ToArray<T>", "Enumerable is null.");
            }

            IList<T> aux = new List<T>(enumerable);
            T[] result = new T[aux.Count];
            aux.CopyTo(result, 0);

            return result;
        }

        public static void setFromTabDirect(SelectQuery query, IList<Relation> relations, IArea area)
        {
            setFromTabDirect(query, relations, area.Information);
        }

        public static void setFromTabDirect(SelectQuery query, IList<Relation> relations, AreaInfo area)
        {
            query.From(area.QSystem, area.TableName, area.Alias);

            foreach (Relation r in relations)
            {
                query.Join(r.TargetTable, r.AliasTargetTab, TableJoinType.Left)
                    .On(CriteriaSet.And()
                        .Equal(r.AliasSourceTab, r.SourceRelField, r.AliasTargetTab, r.TargetRelField));
            }
        }

        public static SelectQuery buildQueryCount(SelectQuery query)
        {
            SelectQuery newQuery = (SelectQuery)query.Clone();

            newQuery.SelectFields.Clear();
            newQuery.OrderByFields.Clear();

            newQuery.Select(SqlFunctions.Count(1), "count")
                .Offset(0)
                .Page(1)
                .PageSize(null);

			newQuery.noLock = query.noLock;

            return newQuery;
        }

        public static void buildQueryInsert(InsertQuery query, IArea area)
        {
            query.Into(area.QSystem, area.TableName);

            IEnumerator enumCampos = area.Fields.Values.GetEnumerator();
            IDictionary<string, Field> camposBD = area.DBFields;
            while (enumCampos.MoveNext())
            {
                RequestedField campoPedido = (RequestedField)enumCampos.Current;

                // Foreign fields (EPHs) can be present in the requested fields list
                if (!camposBD.ContainsKey(campoPedido.Name))
                    continue;

                if (camposBD[campoPedido.Name] != null)
                {
                    Field campoBD = (Field)camposBD[campoPedido.Name];
					if (campoBD.IsVirtual)
                        continue;

                    query.Value(campoPedido.Name, ToValidDbValue(campoPedido.Value, campoBD));
                }
            }
        }

        public static object getRandomValue(Field Qfield)
        {
			// When you create a Random object, it's seeded with a value from the system clock.
            // If you create Random instances too close in time, they will all be seeded with the same random sequence,
            // which means you get the same value lots of times.
            // So, we should create a single Random object for the class, instead of creating one Random object each time the function is called.
            // See: http://stackoverflow.com/questions/767999/random-number-generator-only-generating-one-random-number
            //Random nr = new Random();
            //int Qvalue = nr.Next(-99999,0);
            //SJ (26.04.2007) Limitou-se o number random á largura do Qfield -1 (por causa do sign '-') por causa dos fields do tipo caracter

            int minValue = int.MinValue;
            if(Qfield.FieldSize < 11)
                // For higher FieldSize we have to assume minValue = int.MinValue, because otherwise minValue can become positive due to overflow.
                minValue = (int)(Math.Pow(10, Qfield.FieldSize - 1) * -1) + 1;

            //int Qvalue = nr.Next(minValue, 0);
			// Because Random is not thread safe, we need to do some synchronization
            int Qvalue;
            lock (syncLock)
            {
                Qvalue = random.Next(minValue, 0);
            }
            //a conversaoBd assume (e bem) que o Qfield ja vem no tipo certo, no entanto aqui queremos transformar o inteiro
            //em diferentes tipos consoante o tipo de Qfield.
            return convertIntegerToInterno(Qvalue, Qfield.FieldType.Formatting);
        }

        private static object convertIntegerToInterno(int Qvalue, FieldFormatting tipo)
        {
            switch (tipo)
            {
                case FieldFormatting.CARACTERES:
                    return Qvalue.ToString();
                case FieldFormatting.INTEIRO:
                    return Qvalue;
                case FieldFormatting.FLOAT:
                    return (decimal)Qvalue;
                case FieldFormatting.DATA:
                case FieldFormatting.DATAHORA:
                case FieldFormatting.DATASEGUNDO:
                    return DateTime.MinValue;
                default:
					throw new PersistenceException(null, "QueryUtils.convertIntegerToInterno", "Type not accepted: " + tipo.ToString());
            }
        }

        public static void buildQueryInsertShadow(InsertQuery query, IArea area, FunctionType functionType, string user)
        {
            query.Into(area.QSystem, area.ShadowTabName);

            IEnumerator enumCampos = area.Fields.Values.GetEnumerator();
            IDictionary<string, Field> camposBD = area.DBFields;
            while (enumCampos.MoveNext())
            {
                RequestedField campoPedido = (RequestedField)enumCampos.Current;
                if (camposBD[campoPedido.Name] != null)
                {
                    Field campoBD = (Field)camposBD[campoPedido.Name];
                    if (campoBD.IsVirtual)
                        continue;

                    query.Value(campoPedido.Name, ToValidDbValue(campoPedido.Value, campoBD));
                }
            }

            //20051207 nao esquecer de preencher o operdel e de criar operation
            //The operation is going to be hardcoded for now.
            query.Value("operacao", "D")
                .Value("operdel", user)
                .Value("datadel", DateTime.Now);
        }

        [Obsolete("Use fillQueryUpdate(UpdateQuery, IArea) instead.")]
        public static void fillQueryUpdate(UpdateQuery query, IArea area, string userName)
        {
            fillQueryUpdate(query, area);
        }

        public static void fillQueryUpdate(UpdateQuery query, IArea area)
        {
            string QtableName = area.TableName.Trim();
            query.Update(area.QSystem, QtableName);

            //SO 20060919
            query.Where(CriteriaSet.And()
                .Equal(QtableName, area.PrimaryKeyName, area.returnValueField(area.Alias + "." + area.PrimaryKeyName)));
            Dictionary<string, Field> camposBD = area.DBFields;
            IEnumerator enumCampos = area.Fields.Values.GetEnumerator();
            while (enumCampos.MoveNext())
            {
                RequestedField campoPedido = (RequestedField)enumCampos.Current;
                if (!campoPedido.Name.Equals(area.PrimaryKeyName) && camposBD[campoPedido.Name] != null)
                {
                    Field campoBD = (Field)camposBD[campoPedido.Name];
					if (campoBD.IsVirtual)
                        continue;

                    // Encrypt the password fields before save in the database
                    if(campoBD.FieldType.Equals(FieldType.PASSWORD))
                    {
                        // The password field, if it does not have the value, will not change what is in the database.
                        if (!campoBD.isEmptyValue(campoPedido.Value))
                        {
                            var encryptedData = (EncryptedDataType)campoPedido.Value;
                            //If we only have the decrypted value, we'll try use the encryption associated with the field.
                            if (string.IsNullOrWhiteSpace(encryptedData.EncryptedValue as string) && campoBD.EncryptFieldValueFunction != null)
                                encryptedData.EncryptedValue = campoBD.EncryptFieldValueFunction(encryptedData.DecryptedValue, campoBD, area);

                            query.Set(campoPedido.Name, ToValidDbValue(encryptedData, campoBD));
                        }
                    }
                    else if (!campoBD.FieldType.Equals(FieldType.IMAGEM_JPEG) && !campoBD.FieldType.Equals(FieldType.PATH)
                        && !campoBD.FieldType.Equals(FieldType.DATACRIA) && !campoBD.FieldType.Equals(FieldType.OPERCRIA)
                        && !campoBD.FieldType.Equals(FieldType.HORACRIA) && !campoBD.FieldType.Equals(FieldType.INSTANTECRIA) /*&& !campoBD.FieldType.Equals(FieldType.FICHEIRO_BD)*/)
                    {
						query.Set(campoPedido.Name, ToValidDbValue(campoPedido.Value, campoBD));
                    }
                    else
                    {
                        // testa se foi feito o upload (* significa que o file não foi alterado)
                        if (campoPedido.Value.ToString().Length != 0 && !campoPedido.Value.ToString().StartsWith("*")
                            && !campoBD.FieldType.Equals(FieldType.DATACRIA) && !campoBD.FieldType.Equals(FieldType.OPERCRIA)
                            && !campoBD.FieldType.Equals(FieldType.HORACRIA) && !campoBD.FieldType.Equals(FieldType.INSTANTECRIA))
                        {
                            query.Set(campoPedido.Name, ToValidDbValue(campoPedido.Value, campoBD));
                        }
                    }
                }
            }
        }

        public static void increaseQuery(SelectQuery query, IList<SelectField> select, ITableSource from, IList<TableJoin> joins, CriteriaSet where, int numRecords, CriteriaSet conditions, IList<ColumnSort> orderRequest, bool selectDistinct)
        {
            query.SelectFields.Clear();
            foreach (SelectField field in select)
            {
                query.SelectFields.Add(field);
            }
            query.FromTable = from;
            if (joins != null)
            {
                foreach (TableJoin join in joins)
                {
                    query.Joins.Add(join);
                }
            }
            query.WhereCondition = CriteriaSet.And()
                .SubSet(where)
                .SubSet(conditions);
            query.OrderByFields.Clear();
            if (orderRequest != null)
            {
                foreach (ColumnSort sort in orderRequest)
                {
                    query.OrderByFields.Add(sort);
                }
            }
            query.Distinct(selectDistinct);
            if (numRecords > 0)
            {
                query.PageSize(numRecords);
            }
        }

        public static void setWhereGetPos(SelectQuery query, IList<ColumnSort> sorting, IArea area, string primaryKeyValue)
        {
            if (query.WhereCondition == null)
            {
                query.Where(CriteriaSet.And());
            }

            if (sorting.Count == 0)
                throw new PersistenceException(null, "QuerySelect.setWhereGetPos", "No fields to order by.");

            SelectQuery innerQuery = new SelectQuery()
                .Select(sorting[0].Expression, "order")
                .From(area.QSystem, area.TableName, area.Alias)
                .Where(CriteriaSet.And()
                    .Equal(area.Alias, area.PrimaryKeyName, primaryKeyValue));
			if (innerQuery.Joins != null)
            {
                foreach (TableJoin join in query.Joins)
                {
                    innerQuery.Joins.Add(join);
                }
            }

            if (sorting[0].Order == SortOrder.Ascending)
            {
                query.WhereCondition.LesserOrEqual(sorting[0].Expression, innerQuery);
            }
            else
            {
                query.WhereCondition.GreaterOrEqual(sorting[0].Expression, innerQuery);
            }
        }

        public static void addWhere(SelectQuery query, CriteriaSet condition, CriteriaSetOperator conector)
        {
            if (condition != null)
            {
                if (query.WhereCondition == null)
                {
                    query.Where(new CriteriaSet(conector));
                }

                query.WhereCondition.SubSet(condition);
            }
        }

        public static void increaseQueryBetweenLines(SelectQuery query, IList<SelectField> select, ITableSource from, CriteriaSet where, CriteriaSet conditions, IList<ColumnSort> sorting, int firstLine, int lastLine, bool selectDistinct)
        {
            query.SelectFields.Clear();
            foreach (SelectField field in select)
            {
                query.SelectFields.Add(field);
            }
            query.FromTable = from;
            query.WhereCondition = CriteriaSet.And()
                .SubSet(where)
                .SubSet(conditions);
            query.OrderByFields.Clear();
            foreach (ColumnSort sort in sorting)
            {
                query.OrderByFields.Add(sort);
            }
            query.Distinct(selectDistinct).PageSize(lastLine - firstLine).Offset(firstLine);
        }

        public static CriteriaOperator ParseEphOperator(string ephOperator)
        {
            switch (ephOperator)
            {
                case "=":
                    return CriteriaOperator.Equal;
                case "<":
                    return CriteriaOperator.Lesser;
                case ">":
                    return CriteriaOperator.Greater;
                case "<=":
                    return CriteriaOperator.LesserOrEqual;
                case ">=":
                    return CriteriaOperator.GreaterOrEqual;
                case "!=":
                    return CriteriaOperator.NotEqual;
                default:
					throw new PersistenceException(null, "QueryUtils.ParseEphOperator", "Unknown operator: " + ephOperator);
            }
        }

        public static CriteriaOperator ParseSqlOperator(string sqlOperator)
        {
            switch (sqlOperator)
            {
                case "=":
                    return CriteriaOperator.Equal;
                case "<>":
                    return CriteriaOperator.NotEqual;
                case ">":
                    return CriteriaOperator.Greater;
                case ">=":
                    return CriteriaOperator.GreaterOrEqual;
                case "<":
                    return CriteriaOperator.Lesser;
                case "<=":
                    return CriteriaOperator.LesserOrEqual;
                case "LIKE":
                    return CriteriaOperator.Like;
                case "IN":
                    return CriteriaOperator.In;
                case "NOT IN":
                    return CriteriaOperator.NotIn;
                case "EXISTS":
                    return CriteriaOperator.Exists;
                case "NOT EXISTS":
                    return CriteriaOperator.NotExists;
                default:
					throw new PersistenceException(null, "QueryUtils.ParseSqlOperator", "Unknown operator: " + sqlOperator);
            }
        }

        public static void addSelect(SelectQuery query, IList<SelectField> fields)
        {
            foreach (SelectField field in fields)
            {
                query.SelectFields.Add(field);
            }
        }

        public static void increaseQuery(SelectQuery query, CSGenio.persistence.PersistentSupport.ControlQueryDefinition definition)
        {
            if (definition.SelectFields != null)
            {
                foreach (SelectField field in definition.SelectFields)
                {
                    query.SelectFields.Add(field);
                }
            }

            if (definition.FromTable != null)
            {
                query.FromTable = definition.FromTable;
            }

            if (definition.Joins != null)
            {
                foreach (TableJoin join in definition.Joins)
                {
                    query.Joins.Add(join);
                }
            }

            if (definition.WhereConditions != null)
            {
                if (query.WhereCondition == null)
                {
                    query.Where(CriteriaSet.And());
                }
                query.WhereCondition.SubSet(definition.WhereConditions);
            }

            query.Distinct(definition.Distinct);
        }

        public static object ToValidDbValue(object value, Field field)
        {

			if (field.FieldType == FieldType.CHAVE_ESTRANGEIRA || field.FieldType == FieldType.CHAVE_FALSA)
            {
                if (string.IsNullOrEmpty(Convert.ToString(value)))
                   return DBNull.Value;
                else
                    return value;
            }

            if(field.FieldFormat == FieldFormatting.ENCRYPTED)
            {
                return (value as EncryptedDataType)?.EncryptedValue;
            }
			else if (value is DateTime && ((DateTime)value) == DateTime.MinValue)
            {
                return null;
            }
			else if (field.FieldFormat == FieldFormatting.GUID)
			{
				if (string.IsNullOrEmpty(Convert.ToString(value)))
				{
					return null;
				}
				else
				{
					return value.ToString();
				}
			}
			else if (field.FieldType == FieldType.MEMO_COMP_RTF)
			{
				if (string.IsNullOrEmpty(Convert.ToString(value)))
				{
					return System.Text.Encoding.UTF8.GetBytes("");
				}
				else
				{
					return System.Text.Encoding.UTF8.GetBytes(value.ToString());
				}
			}
			else if (field.FieldType == FieldType.GEOGRAPHY)
            {
                if (string.IsNullOrEmpty(Convert.ToString(value)))
                    return null;
                else
                    return value;
            }
            else if (field.FieldType == FieldType.GEO_SHAPE || field.FieldType == FieldType.GEOMETRIC)
            {
                return value?.ToString();
            }
            else if (field.FieldType == FieldType.DATA)
            {
                return ((DateTime)value).Date;
            }

            return value;
        }

        // TODO: As seguintes funções devem ser revistos to poder reaproveitar...
        // tablesRelationships <= Tornar a função privada que irá receber SelectQuery aumentava query com Joins e devolvia SelectQuery
        // SetInnerJoins <=

        /// <summary>
        /// Método que permite obter a clausula from to fields que não estão em tables directamente referenciadas
        /// Este método devolve uma lista com os id's dos caminhos necessários to construir os queries
        /// </summary>
        /// <param name="tabIndAux">tables indirectamente relacionadas com a area</param>
        /// <param name="area">area do pedido</param>
        /// <returns>A lista de relações to chegar às areas pedidas</returns>
        public static List<Relation> tablesRelationships(List<string> tabIndAux, IArea area)
        {
            List<Relation> relations = new List<Relation>();
            foreach (string outra in tabIndAux)
            {
                //se já chegámos a esta table não precisamos dos caminhos
                if (relations.Exists(x => x.AliasTargetTab == outra))
                    continue;

                List<Relation> areasRelationships = area.Information.GetRelations(outra);
                if(areasRelationships == null)
                    continue;

                //combinar as relações to a nova area com as existentes
                foreach (Relation rel in areasRelationships)
                    if (!relations.Contains(rel))
                        relations.Add(rel);
            }
            return relations;
        }

        /// <summary>
        /// Checks fields used on the selectquery for different tables from the base area.
        /// Adds the different areas to the otherTables list
        /// </summary>
        /// <param name="fields">Array of used fields</param>
        /// <param name="area">The base area</param>
        /// <param name="otherTables">List of other tables</param>
        public static void checkFieldsForForeignTables(string[] fields, Area area, List<string> otherTables)
        {
            foreach (string field in fields)
            {
                string fieldArea = field.Split(new char[] { '.' })[0];
                if (!fieldArea.Equals(area.Alias) && !otherTables.Contains(fieldArea))
                    otherTables.Add(fieldArea);
            }
        }

        /// <summary>
        /// Checks the conditions used on the selectQuery for different tables from the base area.
        /// Adds the different areas to the otherTables list
        /// </summary>
        /// <param name="condicao">The condition used on the selectQuery</param>
        /// <param name="area">The base area</param>
        /// <param name="otherTables"></param>
        public static void checkConditionsForForeignTables(CriteriaSet condition, Area area, List<string> otherTables)
        {
            if (condition == null)
                return;
            foreach (Criteria c in condition.Criterias)
            {
                if (c.LeftTerm is ColumnReference)
                {
                    ColumnReference f = c.LeftTerm as ColumnReference;
                    if (!f.TableAlias.Equals(area.Alias) && !otherTables.Contains(f.TableAlias))
                        otherTables.Add(f.TableAlias);
                }
                else if (c.LeftTerm is Quidgest.Persistence.FieldRef)
                {
                    var f = c.LeftTerm as Quidgest.Persistence.FieldRef;
                    if (!f.Area.Equals(area.Alias) && !otherTables.Contains(f.Area))
                        otherTables.Add(f.Area);
                }
                else if (c.LeftTerm is SqlFunction)
                {
                    _checkConditionsSqlFunctionForForeignTables(c.LeftTerm as SqlFunction, area, otherTables);
                }
                else if(c.LeftTerm is SelectQuery)
                {
					//RMR(2019-07-02) - Exists operator does not imply a direct relation, as it can refere to a foreign key, or any other field
                    CriteriaOperator[] excludedOperators = new CriteriaOperator[] { CriteriaOperator.Exists, CriteriaOperator.NotExists };
                    if (excludedOperators.Contains(c.Operation))
                        continue;

                    var sq = c.LeftTerm as SelectQuery;
                    if (sq.FromTable != null && !sq.FromTable.TableAlias.Equals(area.Alias) && !otherTables.Contains(sq.FromTable.TableAlias))
                        otherTables.Add(sq.FromTable.TableAlias);
                    foreach (var f in sq.SelectFields)
                    {
                        if (!f.Alias.Equals(area.Alias) && !otherTables.Contains(f.Alias))
                            otherTables.Add(f.Alias);
                    }
                    checkConditionsForForeignTables(sq.WhereCondition, area, otherTables);
                }
                else if (c.LeftTerm is CriteriaSet)
                {
                    checkConditionsForForeignTables((CriteriaSet)c.LeftTerm, area, otherTables);
                }
            }

            // Need to apply recursion for subsets
            foreach (CriteriaSet subset in condition.SubSets)
                checkConditionsForForeignTables(subset, area, otherTables);
        }

        private static void _checkConditionsSqlFunctionForForeignTables(SqlFunction condition, Area area, List<string> otherTables)
        {
            foreach (var arg in condition.Arguments)
            {
                if (arg is ColumnReference)
                {
                    ColumnReference f = arg as ColumnReference;
                    if (!f.TableAlias.Equals(area.Alias) && !otherTables.Contains(f.TableAlias))
                        otherTables.Add(f.TableAlias);
                }
                else if (arg is Quidgest.Persistence.FieldRef)
                {
                    var f = arg as Quidgest.Persistence.FieldRef;
                    if (!f.Area.Equals(area.Alias) && !otherTables.Contains(f.Area))
                        otherTables.Add(f.Area);
                }
                else if (arg is SqlFunction)
                {
                    _checkConditionsSqlFunctionForForeignTables(arg as SqlFunction, area, otherTables);
                }
            }
        }

        public static void SetInnerJoins(string[] requestedFields, CriteriaSet conditions, Area area, SelectQuery querySelect)
        {
            // Look for inner joins
            List<string> tabelasAcima = new List<string>();
            if (querySelect.SelectFields != null)
                checkFieldsForForeignTables(requestedFields, area, tabelasAcima);
            if (conditions != null)
                checkConditionsForForeignTables(conditions, area, tabelasAcima);

            List<Relation> relations = tablesRelationships(tabelasAcima, area);
            setFromTabDirect(querySelect, relations, area);
        }

        /// <summary>
        /// Devolve os fields de uma área em format de text separados por vírgulas
        /// de mode a serem usados no SQL.
        /// Deve ser preterido sobre o "*", porque podem ter sido apagadas colunas
        /// nas definições, mas ainda permanecem no repositório SQL - a consequência é uma
        /// excepção no cliente, uma vez que são obtidas essas colunas no retorno da query;
        /// depois o cliente tenta aceder à sua estrutura correspondente à table nessas
        /// colunas e as mesmas não existem!
        /// </summary>
        /// <param name="area"></param>
        /// <returns></returns>
        /// <remarks>eu queria usar o LINQ e recorrer à função acima, mas
        /// o GenioServer está feito sobre a plataforma .NET 2.0
        /// </remarks>
        public static StringBuilder fieldsToSQL(IArea area)
        {
            StringBuilder res = new StringBuilder(area.DBFields.Count * 8);

            foreach (Field Qfield in area.DBFields.Values)
                res.Append(Qfield.Name + ",");

            if (res.Length > 0)
                res.Remove(res.Length - 1, 1);

            return res;
        }

    }
}
