using System;
using System.Data;
using Quidgest.Persistence.GenericQuery;

namespace Quidgest.Persistence.Dialects
{
    /// <summary>
    /// Specificities of the DBMS MS Sql Server 2008
    /// </summary>
    /// <remarks>
    /// <!--
    /// Author: CX 2011.06.28
    /// Modified:
    /// Reviewed:
    /// -->
    /// </remarks>
    public class SqlServer2008Dialect : SqlServer2005Dialect
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
        public SqlServer2008Dialect()
        {
            RegisterType(DbType.DateTime2, "DATETIME2");
            RegisterType(DbType.DateTimeOffset, "DATETIMEOFFSET");
            RegisterType(DbType.Date, "DATE");
            RegisterType(DbType.Time, "TIME");

            string[] typesArr = RegisteredTypesToArray();

            RegisterFunction(SqlFunctionType.Round, new SqlFunctionTemplate("ROUND({0}, {1})"));

            RegisterFunction(SqlFunctionType.Cast, new SqlFunctionTemplate("CAST({0} AS {1})", false, false, "AS", typesArr));
            RegisterFunction(SqlFunctionType.Custom, new SqlFunctionTemplate("dbo.{0}({1})", true, false, ",", typesArr));
            RegisterFunction(SqlFunctionType.SysCustom, new SqlFunctionTemplate("{0}({1})", true, false, ",", typesArr));
        }
    }
}
