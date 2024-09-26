using CSGenio.framework;
using CSGenio.business;
using CSGenio.persistence;
using Quidgest.Persistence.GenericQuery;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


namespace WebTest
{
    public class Utils
    {
        public static string ParseLookup(string value, PersistentSupport sp, User u)
        {
            List<string> listFunctions = new List<string> { "Lookup", "LookupLast", "LookupFirst" };
            String res = "";

            if (!string.IsNullOrEmpty(value))
            {
                // 1. Verificar se o parametro é um valor com lookup ou não
                if (HasLookup(value))
                {
                    //  try
                    //   {
                    // 1.1.Se for com lookup, fazer o parse do mesmo
                    // Split do lookup;
                    string[] strsplit = StringUtils.Split(value, "(", ")");
                    string funcName = strsplit[0].ToString();

                    if (!listFunctions.Contains(funcName, StringComparer.OrdinalIgnoreCase))
                    {
                        throw new TestException("Wrong lookup function name", "ParseLookup, Wrong lookup function name", "Wrong lookup function name");
                    }

                    // Faz replace de todas os caracteres adicionais da fórmula: chavetas e aspas
                    string stringformated = strsplit[1].Replace("{", "").Replace("}", "").Replace("\"", "");

                    // Faz split das virgulas para separar os vários pares de parâmetros
                    string[] parameters = stringformated.Split(new char[] { ',' });

                    // Encontrar a tabela
                    // Para cada parametro "{[TABELA->CAMPO, "valor"}"
                    // indices zeros ou pares têm o par tabela->campo. Restantes são os valores
                    //string tablename = parameters[0].ToLower();

                    List<string> fields = new List<string>();
                    List<string> values = new List<string>();
                    int i = 0;
                    string[] stringSeparators = new string[] { "->" };
                    List<string> listTables = new List<string>();

                    foreach (string parameter in parameters)
                    {
                        // Se for par é um tuplo tabela->campo
                        //[TABELA->CAMPO]
                        if (IsEven(i))
                        {
                            string aux = parameter.Replace("[", "").Replace("]", "");
                            string[] aux1 = aux.Split(stringSeparators,
                            StringSplitOptions.RemoveEmptyEntries);

                            //tabela
                            listTables.Add(aux1[0].ToLower());

                            //campo
                            fields.Add(aux1[1]);
                        }

                        else
                        {
                            values.Add(Utils.formataValorCampo(listTables[(listTables.Count - 1)], fields[(fields.Count - 1)], parameter));
                        }
                        i++;
                    }

                    List<string> tablesDistinct = listTables.Distinct().ToList();
                    if (tablesDistinct.Count > 1)
                    {
                        throw new TestException("More than one table in Lookup formula: " + value, "ParseLookup, More than one table in Lookup formula: " + value, "More than one table in Lookup formula: " + value);
                    }

					try{
                    //Retorna o primeiro registo [0]
                    res = QueryDB(tablesDistinct[0], fields, values, u, sp, funcName)[0].ToString();
					} catch (Exception e)
					{
						throw new TestException("No records found for Lookup formula: " + value, "ParseLookup, No records found for Lookup formula: " + value, "No records found for Lookup formula: " + value + " - " + e.Message);
					}

                }
                else
                // 1.2. Se não for, retornar per si
                {
                    res = value;
                }
            }
            return res;
        }


        private static bool HasLookup(string value)
        {
            return value.StartsWith("Lookup");
        }

        private static ArrayList QueryDB(string tablename, List<string> fields, List<string> values, User u,PersistentSupport  sp, string funcName)
        {
            DbArea area = (DbArea)Area.createArea(tablename, u, u.CurrentModule);

            // Construct query
            SelectQuery query = new SelectQuery();
            query.Select(area.TableName, area.PrimaryKeyName);
            query.From(area.TableName, area.TableName);

            // Criteria
            CriteriaSet innerConditions = CriteriaSet.And();

            for (int i = 0; i < fields.Count; i++)
            {
                innerConditions.Criterias.Add(new Criteria(
                        new ColumnReference(area.TableName, fields[i]),
                        QueryUtils.ParseEphOperator("="),
                        values[i]));
            }

            query.Where(innerConditions);
            // É preciso ordenar consoante o lookup
            // Se eu tiver um datacria, ordena por isso
            // Caso contrário é random
            string fieldStamp = GetDataCriaField(area);

            if (!string.IsNullOrEmpty(fieldStamp))
            {
                if (funcName.Equals("LookupFirst"))
                    query.OrderBy(area.TableName, fieldStamp, Quidgest.Persistence.GenericQuery.SortOrder.Ascending);
                else if (funcName.Equals("LookupLast"))
                    query.OrderBy(area.TableName, fieldStamp, Quidgest.Persistence.GenericQuery.SortOrder.Descending);
                else
                    query.OrderBy(area.TableName, fieldStamp, Quidgest.Persistence.GenericQuery.SortOrder.Ascending);
            }

            // Vai buscar apenas a coluna da chave primária
            ArrayList resultado = sp.executeReaderOneRow(query);

            return resultado;
        }

        private static string GetDataCriaField(Area area)
        {

            string[] camposCarimbo = area.StampFieldsIns;
            string res = "";
            int i = 0;
            // Se não tem campos carimbo, não tem campo DATACRIA
            if (camposCarimbo == null || camposCarimbo.Length == 0)
            {
                res = "";
            }
            else
            {

                while (string.IsNullOrEmpty(res) && (i <= camposCarimbo.Length))
                {
                    Field campoCarimbo = (Field)area.DBFields[camposCarimbo[i]];
                    if (campoCarimbo.FieldType == FieldType.DATACRIA)
                    {
                        res = campoCarimbo.Name;
                    }
                    i++;
                }
            }

            return res;
        }

        public static string formataValorCampo(string area, string field, string value)
        {
            AreaInfo areaB = Area.GetInfoArea(area);
            Field cmp = areaB.DBFields[field.ToLower()];
            FieldType tipo = cmp.FieldType;

            if ((cmp.FieldType == FieldType.CHAVE_ESTRANGEIRA) || (cmp.FieldType == FieldType.CHAVE_PRIMARIA))
                return (value.PadLeft(cmp.FieldSize, ' '));
            else
                return (value);
        }


        public static bool IsEven(int value)
        {
            return value % 2 == 0;
        }


    }
}
