using System;
using System.Collections.Generic;
using System.Text;
using Quidgest.Persistence.GenericQuery;
using System.Data;

namespace Quidgest.Persistence.Dialects
{
    /// <summary>
    /// Specificities of the DBMS MS Sql Server 2000
    /// </summary>
    /// <remarks>
    /// <!--
    /// Author: CX 2011.06.28
    /// Modified:
    /// Reviewed:
    /// -->
    /// </remarks>
    public class SqlServer2000Dialect : Dialect
    {
        /// <summary>
        /// Overriden. The prefix for the sql variables for this DBMS.
        /// </summary>
        /// <remarks>
        /// <!--
        /// Author: CX 2011.06.28
        /// Modified:
        /// Reviewed:
        /// -->
        /// </remarks>
        public override string NamedPrefix
        {
            get
            {
                return "@";
            }
        }

        /// <summary>
        /// Overriden. True if the named prefix should be used in the sql references to da variable.
        /// </summary>
        /// <remarks>
        /// <!--
        /// Author: CX 2011.08.15
        /// Modified:
        /// Reviewed:
        /// -->
        /// </remarks>
        public override bool UseNamedPrefixInSql
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Overriden. True if the named prefix should be used in the parameter.
        /// </summary>
        /// <remarks>
        /// <!--
        /// Author: CX 2011.08.15
        /// Modified:
        /// Reviewed:
        /// -->
        /// </remarks>
        public override bool UseNamedPrefixInParameter
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Overriden. True if the DBMS supports a max number of results specified in the query.
        /// </summary>
        /// <remarks>
        /// <!--
        /// Author: CX 2011.06.28
        /// Modified:
        /// Reviewed:
        /// -->
        /// </remarks>
        public override bool SupportsLimit
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Overriden. True if the DBMS supports skipping a specified number of results in the query.
        /// </summary>
        /// <remarks>
        /// <!--
        /// Author: CX 2011.06.28
        /// Modified:
        /// Reviewed:
        /// -->
        /// </remarks>
        public override bool SupportsLimitOffset
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Overriden. True if the limit should be inserted right after the select keyword, otherwise it is inserted at the end of the query.
        /// </summary>
        /// <remarks>
        /// <!--
        /// Author: CX 2011.06.28
        /// Modified:
        /// Reviewed:
        /// -->
        /// </remarks>
        public override bool BindLimitParametersFirst
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Overriden. The open quote char fot his DBMS.
        /// </summary>
        /// <remarks>
        /// <!--
        /// Author: CX 2011.06.28
        /// Modified:
        /// Reviewed:
        /// -->
        /// </remarks>
        public override char OpenQuote
        {
            get
            {
                return '[';
            }
        }

        /// <summary>
        /// Overriden. The close quote char for this DBMS.
        /// </summary>
        /// <remarks>
        /// <!--
        /// Author: CX 2011.06.28
        /// Modified:
        /// Reviewed:
        /// -->
        /// </remarks>
        public override char CloseQuote
        {
            get
            {
                return ']';
            }
        }

        /// <summary>
        /// Constructor. Initializes the supported functions.
        /// </summary>
        /// <remarks>
        /// <!--
        /// Author: CX 2011.06.28
        /// Modified: CX 2011.07.07
        /// Reviewed:
        /// -->
        /// </remarks>
        public SqlServer2000Dialect()
        {
            RegisterType(DbType.AnsiStringFixedLength, "CHAR(255)");
            RegisterType(DbType.AnsiString, "VARCHAR(8000)");
            RegisterType(DbType.Binary, "IMAGE");
            RegisterType(DbType.Boolean, "BIT");
            RegisterType(DbType.Byte, "TINYINT");
            RegisterType(DbType.Currency, "MONEY");
            RegisterType(DbType.Date, "DATE");
            RegisterType(DbType.DateTime, "DATETIME");
            RegisterType(DbType.Decimal, "DECIMAL(19,5)");
            RegisterType(DbType.Double, "DOUBLE");
            RegisterType(DbType.Guid, "UNIQUEIDENTIFIER");
            RegisterType(DbType.Int16, "SMALLINT");
            RegisterType(DbType.Int32, "INT");
            RegisterType(DbType.Int64, "BIGINT");
            RegisterType(DbType.Single, "REAL");
            RegisterType(DbType.StringFixedLength, "NCHAR(4000)");
            RegisterType(DbType.String, "NVARCHAR(8000)");
            RegisterType(DbType.Time, "DATETIME");

            RegisterType(CustomDbType.StandardAnsiString, "VARCHAR(50)");
            RegisterType(CustomDbType.StandardDecimalSearch, "DECIMAL(38,10)");

            string[] typesArr = RegisteredTypesToArray();

            RegisterFunction(SqlFunctionType.Locate, new SqlFunctionTemplate("CHARINDEX({1}, {0}, {2})"));
            RegisterFunction(SqlFunctionType.Trim, new SqlFunctionTemplate("RTRIM(LTRIM({0}))"));
            RegisterFunction(SqlFunctionType.Length, new SqlFunctionTemplate("LEN({0})"));
            RegisterFunction(SqlFunctionType.BitLength, new SqlFunctionTemplate("(DATALENGTH({0})*8)"));
            RegisterFunction(SqlFunctionType.Module, new SqlFunctionTemplate("(({0}) % ({1}))"));
            RegisterFunction(SqlFunctionType.Cast, new SqlFunctionTemplate("CAST({0} AS {1})", false, false, "AS", typesArr));
            RegisterFunction(SqlFunctionType.Extract, new SqlFunctionTemplate("DATEPART({0}, {1})", false, false, ",", new[] { "SECOND", "MINUTE", "HOUR", "DAY", "MONTH", "YEAR" }));
            RegisterFunction(SqlFunctionType.Concat, new SqlFunctionTemplate("({0})", true, false, "+"));

            RegisterFunction(SqlFunctionType.CurrentTimestamp, new SqlFunctionTemplate("CURRENT_TIMESTAMP"));
            RegisterFunction(SqlFunctionType.SystemDate, new SqlFunctionTemplate("GETDATE()"));

            RegisterFunction(SqlFunctionType.Second, new SqlFunctionTemplate("DATEPART(SECOND, {0})"));
            RegisterFunction(SqlFunctionType.Minute, new SqlFunctionTemplate("DATEPART(MINUTE, {0})"));
            RegisterFunction(SqlFunctionType.Hour, new SqlFunctionTemplate("DATEPART(HOUR, {0})"));
            RegisterFunction(SqlFunctionType.Day, new SqlFunctionTemplate("DATEPART(DAY, {0})"));
            RegisterFunction(SqlFunctionType.Month, new SqlFunctionTemplate("DATEPART(MONTH, {0})"));
            RegisterFunction(SqlFunctionType.Year, new SqlFunctionTemplate("DATEPART(YEAR, {0})"));

            RegisterFunction(SqlFunctionType.Custom, new SqlFunctionTemplate("dbo.{0}({1})", true, false, ",", typesArr));
            RegisterFunction(SqlFunctionType.SysCustom, new SqlFunctionTemplate("{0}({1})", true, false, ",", typesArr));

			RegisterFunction(SqlFunctionType.Week, new SqlFunctionTemplate("DATEPART(WEEK, {0})"));
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

            if (limit != null && limit < 0)
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
            int fromInsertPoint = -1;

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
                        fromInsertPoint = i;
                        alias.Add(lastWord);
                        break;
                    }
                }
			}

            string[] aliasArr = new string[alias.Count];
            alias.CopyTo(aliasArr, 0);

            // insert new sql from bottom to top to keep the insert points correct
            // step 2 - select the top results into a temp table with the row number
            sql.Insert(fromInsertPoint, " INTO #tmp_sort_table ");
            sql.Insert(selectInsertPoint, (max != null ? " TOP " + max : "") + " IDENTITY(INT, 1, 1) AS __genio_sort_row, ");
            // step 1 - drop the temp table if it exists
            sql.Insert(0, "IF EXISTS (SELECT TOP 1 1 FROM dbo.sysobjects WHERE id = OBJECT_ID(N'#tmp_sort_table') AND OBJECTPROPERTY(id, N'IsUserTable') = 1) DROP TABLE #tmp_sort_table" + Environment.NewLine + Environment.NewLine);
            // step 3 - select all rows after the offset from the temp table
            sql.AppendLine();
            sql.AppendLine();
            sql.Append("SELECT ");
            sql.Append(String.Join(",", aliasArr));
            sql.AppendLine(" FROM #tmp_sort_table WHERE __genio_sort_row > " + offset + " ORDER BY __genio_sort_row");
            sql.AppendLine();
            sql.AppendLine();
            // step 4 - drop the temp table
            sql.Append("DROP TABLE #tmp_sort_table");

        }

        /// <summary>
        /// Quotes the specified name for use as a schema name
        /// </summary>
        /// <param name="schemaName">The name to quote</param>
        /// <returns>The quoted name for use as a schema name</returns>
        /// <remarks>
        /// <!--
        /// Author: CX 2011.11.02
        /// Modified:
        /// Reviewed:
        /// -->
        /// </remarks>
        public override string QuoteForSchemaName(string schemaName)
        {
            if (String.IsNullOrEmpty(schemaName))
            {
                return String.Empty;
            }

            string[] parts = schemaName.Split('.');
            for (int i = 0; i < parts.Length; i++)
            {
                if (!IsQuoted(parts[i]))
                {
                    parts[i] = Quote(parts[i]);
                }
            }

            return String.Join(".", parts);
        }

        /// <summary>
        /// Get DELETE statement with alias
        /// </summary>
        /// <param name="table_name">Table name</param>
        /// <param name="alias">Alias</param>
        /// <returns>DELETE [<alias>] FROM [<table_name>] AS [<alias>]</returns>
        public override string DeleteStatement(string table_name, string alias)
        {
            return string.Format("DELETE {0} FROM {1} AS {0}", alias, table_name);
        }

        /// <summary>
        /// Get UPDATE statement with alias
        /// </summary>
        /// <param name="table_name">Table name</param>
        /// <param name="alias">Alias</param>
        /// <param name="set_Clause">SET Clause</param>
        /// <returns>UPDATE [<alias>] SET expression FROM [<table_name>] [<alias>]</returns>
        public override string UpdateStatement(string table_name, string alias, string set_Clause)
        {
            return string.Format("UPDATE {0} {1} FROM {2} AS {0}", alias, set_Clause, table_name);
        }
    }
}