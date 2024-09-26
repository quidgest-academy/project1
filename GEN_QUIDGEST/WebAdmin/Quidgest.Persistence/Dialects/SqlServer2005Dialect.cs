using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Quidgest.Persistence.GenericQuery;

namespace Quidgest.Persistence.Dialects
{
    /// <summary>
    /// Specificities of the DBMS MS Sql Server 2005
    /// </summary>
    /// <remarks>
    /// <!--
    /// Author: CX 2011.06.28
    /// Modified:
    /// Reviewed:
    /// -->
    /// </remarks>
    public class SqlServer2005Dialect : SqlServer2000Dialect
    {
        /// <summary>
        /// Constructor. Initializes the supported functions.
        /// </summary>
        /// <remarks>
        /// <!--
        /// Author: CX 2011.07.07
        /// Modified:
        /// Reviewed:
        /// -->
        /// </remarks>
        public SqlServer2005Dialect()
        {
            RegisterType(DbType.AnsiString, "VARCHAR(MAX)");
            RegisterType(DbType.String, "NVARCHAR(MAX)");
            RegisterType(DbType.Xml, "XML");

            string[] typesArr = RegisteredTypesToArray();

            RegisterFunction(SqlFunctionType.Cast, new SqlFunctionTemplate("CAST({0} AS {1})", false, false, "AS", typesArr));
            RegisterFunction(SqlFunctionType.Custom, new SqlFunctionTemplate("dbo.{0}({1})", true, false, ",", typesArr));
            RegisterFunction(SqlFunctionType.SysCustom, new SqlFunctionTemplate("{0}({1})", true, false, ",", typesArr));
        }

        /// <summary>
        /// Overriden. Adds the limit clause to the query.
        /// </summary>
        /// <param name="sql">The generated sql</param>
        /// <param name="offset">the value of the offset parameter</param>
        /// <param name="limit">the value of the limit parameter</param>
        /// <remarks>
        /// <!--
        /// Author: CX 2011.06.28
        /// Modified:
        /// Reviewed:
        /// -->
        /// </remarks>
        public override void AddLimitString(StringBuilder sql, int offset, int? limit)
        {
            if (sql == null)
            {
                throw new ArgumentNullException("sql");
            }

            if (offset < 0)
            {
                throw new ArgumentOutOfRangeException("offset", offset, "Value cannot be lesser than 0");
            }

            if (limit < 0)
            {
                throw new ArgumentOutOfRangeException("limit", limit, "Value cannot be lesser than 0");
            }

            string s = sql.ToString();

            int? max = null;
            if (limit != null)
            {
                max = UseMaxForLimit ? limit.Value : offset + limit.Value;
            }

            int selectInsertPoint = sql.ToString().StartsWith("SELECT DISTINCT") ? 15 : 6;

            if (offset == 0)
            {
                if (max != null)
					sql.Insert(selectInsertPoint, " TOP " + max.Value + " ");
                return;
            }

            // find the alias of the select fields
            IList<string> alias = new List<string>();
            int parentisisCount = 0;
            int lastSpace = selectInsertPoint;
            string lastWord = null;
            for (int i = selectInsertPoint + 1; i < s.Length; i++)
			{
                if (s[i] == ' ')
                {
                    lastWord = s.Substring(lastSpace + 1, i - lastSpace - 1);
                    lastSpace = i;
                }
                else if (s[i] == '(')
                {
                    parentisisCount++;
                }
                else if (s[i] == ')')
                {
                    parentisisCount--;
                }
                else if (parentisisCount == 0)
                {
                    if (s[i] == ',')
                    {
                        if (i > 0 && s[i - 1] != ' ')
                        {
                            lastWord = s.Substring(lastSpace + 1, i - lastSpace - 1);
                        }
                        lastSpace = i;
                        alias.Add(lastWord);
                    }
                    else if ((s[i] == 'F' || s[i] == 'f')
                        && s.IndexOf("FROM ", i, StringComparison.InvariantCultureIgnoreCase) == i)
                    {
                        alias.Add(lastWord);
                        break;
                    }
                }
			}

			// find the order by clause
            int orderByPoint = s.LastIndexOf("ORDER BY", StringComparison.InvariantCultureIgnoreCase);
            string orderClause = null;
            if (orderByPoint < 0)
            {
                string orderField = alias[0].Replace(".","].[");
                orderClause = "ORDER BY " + orderField;
            }
            else
            {
                orderClause = s.Substring(orderByPoint);
                // insert new sql from bottom to top to keep the insert points correct
                sql = sql.Remove(orderByPoint, s.Length - orderByPoint); // remove the order by clause, it will go to the ROW_NUMBER() OVER(...)
            }
            // add the outter query to satisfy the limit
            string[] aliasArr = new string[alias.Count];
            alias.CopyTo(aliasArr, 0);
            sql.Insert(selectInsertPoint,
                " " + String.Join(",", aliasArr)
                + " FROM (SELECT " + (max != null ? "TOP " + max : "") + " ROW_NUMBER() OVER(" + orderClause + ") __genio_sort_row, ");
            sql.Append(") AS query WHERE __genio_sort_row > " + offset + " ORDER BY __genio_sort_row");
        }
    }
}