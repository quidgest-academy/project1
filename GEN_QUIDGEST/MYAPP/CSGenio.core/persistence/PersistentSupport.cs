using CSGenio.business;
using CSGenio.core.messaging;
using CSGenio.framework;
using ExecuteQueryCore;
using Quidgest.Persistence;
using Quidgest.Persistence.GenericQuery;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace CSGenio.persistence
{
    /// <summary>
    /// Abstract class for the implementation of a generic database persistent support.
    /// </summary>
    public abstract class PersistentSupport : IPersistentSupport
    {
        protected bool m_QueueMode = false;

        public bool QueueMode
        {
            get { return m_QueueMode; }
            set { m_QueueMode = value; }
        }

        public class ControlQueryDefinition
        {
            public IList<SelectField> SelectFields
            {
                get;
                private set;
            }

            public ITableSource FromTable
            {
                get;
                private set;
            }

            public IList<TableJoin> Joins
            {
                get;
                private set;
            }

            public CriteriaSet WhereConditions
            {
                get;
                private set;
            }

            public bool Distinct
            {
                get;
                private set;
            }

            public ControlQueryDefinition(
                IList<SelectField> selectFields, ITableSource fromTable, IList<TableJoin> joins, CriteriaSet whereConditions)
                : this (selectFields, fromTable, joins, whereConditions, false)
            {
            }

            public ControlQueryDefinition(
                IList<SelectField> selectFields, ITableSource fromTable, IList<TableJoin> joins, CriteriaSet whereConditions, bool distinct)
            {
                SelectFields = selectFields;
                FromTable = fromTable;
                Joins = joins;
                WhereConditions = whereConditions;
                Distinct = distinct;
            }

            public SelectQuery ToSelectQuery()
            {
                SelectQuery result = new SelectQuery();

                if (SelectFields != null)
                    foreach (SelectField f in SelectFields)
                        result.SelectFields.Add(f);

                result.FromTable = FromTable;

                if (Joins != null)
                    foreach (TableJoin j in Joins)
                        result.Joins.Add(j);

                result.WhereCondition = WhereConditions.Clone() as CriteriaSet;

                result.Distinct(Distinct);

                return result;
            }
        }

        protected int timeout;
        /// <summary>
        /// Check sp Timeout
        /// </summary>
        public virtual int Timeout
        {
            get { return timeout; }
            set { timeout = value; }
        }

        private IDbConnection conDatabase;
        private IDbTransaction transDatabase;

        public virtual IDbConnection Connection
        {
            get { return conDatabase; }
            protected set { conDatabase = value; }
        }

        /// <summary>
        /// Gets a value indicating whether the connection is closed.
        /// </summary>
        public virtual bool ConnectionIsClosed { get { return Connection?.State == ConnectionState.Closed; } }

        public virtual IDbTransaction Transaction
        {
            get { return transDatabase; }
            protected set { transDatabase = value; }
        }

        /// <summary>
        /// Check if Transaction is null (closed)
        /// </summary>
        public virtual bool TransactionIsClosed { get { return Transaction == null; } }

        public virtual DatabaseType DatabaseType { get; protected set;}

        [Obsolete("Use IDictionary<string, ControlQueryDefinition> controlQueries instead")]
        protected static Hashtable controlos;
        protected static IDictionary<string, ControlQueryDefinition> controlQueries;
        protected static Dictionary<int, PersistentSupport> ligacoes;
        [Obsolete("Use SelectQuery overrideDbeditQuery(User utilizador, string modulo, CriteriaSet condicoes) instead")]
        public delegate QuerySelect overrideDbedit(User user, string module, string conditions, PersistentSupport sp);
        public delegate SelectQuery overrideDbeditQuery(User user, string module, CriteriaSet conditions, IList<ColumnSort> orderBy, PersistentSupport sp);
        [Obsolete("Use IDictionary<string, overrideDbeditQuery> controlQueriesOverride instead")]
        protected static Dictionary<string, overrideDbedit> controlosOverride;
        protected static IDictionary<string, overrideDbeditQuery> controlQueriesOverride;
        protected static Hashtable manualQueries;
        protected QuerySchemaMapping m_mapping = new QuerySchemaMapping();
		protected static Hashtable notifications;

		protected static List<CSGenioAnotificationemailsignature> emailSignatures;

        public virtual QuerySchemaMapping SchemaMapping
        {
            get
            {
                return m_mapping;
            }
            protected set
            {
                m_mapping = value;
            }
        }

        /// <summary>
        /// Sql syntax dialect for the current provider
        /// </summary>
        /// <returns>The syntax dialect</returns>
        public virtual Dialect Dialect { get; protected set; }

        /// <summary>
        /// Dataset Id that created this persistent support
        /// </summary>
        public string Id { get; protected set; }
        /// <summary>
        /// logged client from "frontend" app
        /// </summary>
		public string ClientId { get; protected set; }
        /// <summary>
        /// Only read only commands are allows to this persistence store
        /// </summary>
        public virtual bool ReadOnly { get; protected set; }
        /// <summary>
        /// If this connection is to be established as a master connection
        /// </summary>
        public virtual bool IsMaster { get; protected set; }

        /**
         * Static Constructor
         * - The hashtable controls are keyed to the identifier of the loaded menu option and
         * as the corresponding querie object. These queries are generated by GENIO.
         * - connectionpool objects are filled with
         * data read from the web.xml to the Configuration object.
         */
        static PersistentSupport()
        {
            //queries generated by GENIO
#pragma warning disable 618 //TODO ficam marcados assim explicitamente to saber quer tem de ser mudado
            InitControlos();
#pragma warning restore 618
            InitControlQueries();
            InitManualQueries();
            ligacoes = new Dictionary<int, PersistentSupport>();
			InitNotifications();

        }

        private static void InitManualQueries()
        {
            
        }

        public static Hashtable getManualQueries()
        {
            return manualQueries;
        }

		private static void InitNotifications()
        {
            
        }

        public static Hashtable getNotifications()
        {
            return notifications;
        }

		private void InitEmailSignatures(string year)
        {
            PersistentSupport sp = PersistentSupport.getPersistentSupport(year);

            String SignaturesArea = "notificationemailsignature";
            User user = new User("Q_NOTIFS", "", Configuration.DefaultYear)
            {
                CurrentModule = "NOT"
            };
            user.AddModuleRole(user.CurrentModule, Role.ADMINISTRATION);
            emailSignatures = CSGenioAnotificationemailsignature.searchList(sp, user, CriteriaSet.And().Equal(new Quidgest.Persistence.FieldRef(SignaturesArea, "zzstate"), 0));

        }

        public List<CSGenioAnotificationemailsignature> getEmailSignatures(string year)
        {
            InitEmailSignatures(year);
            return emailSignatures;
        }

        [Obsolete("Use InitControlQueries() instead")]
        private static void InitControlos()
        {
            controlos = new Hashtable();
            controlosOverride = new Dictionary<string, overrideDbedit>();

        }

        /// <summary>
        /// MH - Initialization of elements that cannot be initialized in CSGenio.core due to the use of GlobalFunctions and CSGenioA
        /// </summary>
        /// <param name="in_controlQueries"></param>
        /// <param name="in_controlQueriesOverride"></param>
        public static void SetControlQueries(IDictionary<string, ControlQueryDefinition> in_controlQueries, IDictionary<string, overrideDbeditQuery> in_controlQueriesOverride)
        {
            controlQueries = in_controlQueries;
            controlQueriesOverride = in_controlQueriesOverride;
        }

        private static void InitControlQueries()
        {
            controlQueries = new ControlQueryDictionary();
            controlQueriesOverride = new Dictionary<string, overrideDbeditQuery>();
/*

*/
        }

        /// <summary>
        /// Gets the select command from the SelectQuery,
        /// so that a DataReader can be created from that select command
        /// </summary>
        /// <returns>IDbCommand created from the SelectQuery</returns>
        /// <remarks>
        /// Created by [CJP] at [2016.07.04]
        /// </remarks>
        public IDbCommand GetSelectCommand(SelectQuery query)
        {
            var renderer = new QueryRenderer(this);
            renderer.SchemaMapping = SchemaMapping;

            var sql = renderer.GetSql(query);
            var parameters = renderer.ParameterList;

            IDbDataAdapter adapter = CreateAdapter(sql);

            AddParameters(adapter.SelectCommand, parameters);

            return adapter.SelectCommand;
        }


        /// <summary>
        /// Executes the query in the persistent support
        /// </summary>
        /// <returns>the <see cref="CSGenio.persistence.DataMatrix"/> With the results</returns>
        /// <remarks>
        /// <!--
        /// Author: CX 2011.06.28
        /// Modified:
        /// Reviewed:
        /// -->
        /// </remarks>
        public DataMatrix Execute(SelectQuery query)
        {
            var renderer = new QueryRenderer(this);
            renderer.SchemaMapping = SchemaMapping;

            var sql = renderer.GetSql(query);
			long st = DateTime.Now.Ticks;
            if (Log.IsDebugEnabled) Log.Debug(string.Format("[QueryExecuteSelect] {0}.", sql) + Environment.NewLine + renderer.PrintParameters());

            var parameters = renderer.ParameterList;

            IDbDataAdapter adapter = CreateAdapter(sql);

            AddParameters(adapter.SelectCommand, parameters);

            DataSet ds = new DataSet();
            adapter.Fill(ds);

			if (Log.IsDebugEnabled) Log.Debug("[QueryExecuteSelect] " + (DateTime.Now.Ticks - st) / TimeSpan.TicksPerMillisecond + "ms");

            return new DataMatrix(ds);
        }

        /// <summary>
        /// Executes the query in the persistent support
        /// </summary>
        /// <returns>The query result</returns>
        /// <remarks>
        /// <!--
        /// Author: CX 2011.06.28
        /// Modified:
        /// Reviewed:
        /// -->
        /// </remarks>
        public int Execute(InsertQuery query)
        {
            var renderer = new QueryRenderer(this);
            renderer.SchemaMapping = SchemaMapping;

            var sql = renderer.GetSql(query);
			long st = DateTime.Now.Ticks;
            if (Log.IsDebugEnabled) Log.Debug(string.Format("[QueryExecuteInsert] {0}.", sql) + Environment.NewLine + renderer.PrintParameters());
            var parameters = renderer.ParameterList;

            IDbCommand command = CreateCommand(sql, parameters);

            int result = command.ExecuteNonQuery();
			if (Log.IsDebugEnabled) Log.Debug("[QueryExecuteInsert] " + (DateTime.Now.Ticks - st) / TimeSpan.TicksPerMillisecond + "ms");
			return result;
        }

        /// <summary>
        /// Executes the query in the persistent support
        /// </summary>
        /// <returns>The query result</returns>
        /// <remarks>
        /// <!--
        /// Author: CX 2011.06.28
        /// Modified:
        /// Reviewed:
        /// -->
        /// </remarks>
        public int Execute(DeleteQuery query)
        {
            var renderer = new QueryRenderer(this);
            renderer.SchemaMapping = SchemaMapping;

            var sql = renderer.GetSql(query);
			long st = DateTime.Now.Ticks;
            if (Log.IsDebugEnabled) Log.Debug(string.Format("[QueryExecuteDelete] {0}.", sql) + Environment.NewLine + renderer.PrintParameters());
            var parameters = renderer.ParameterList;

            IDbCommand command = CreateCommand(sql, parameters);

            int result = command.ExecuteNonQuery();
			if (Log.IsDebugEnabled) Log.Debug("[QueryExecuteDelete] " + (DateTime.Now.Ticks - st) / TimeSpan.TicksPerMillisecond + "ms");
			return result;
        }

        /// <summary>
        /// Executes the query in the persistent support
        /// </summary>
        /// <returns>The query result</returns>
        /// <remarks>
        /// <!--
        /// Author: CX 2011.06.28
        /// Modified:
        /// Reviewed:
        /// -->
        /// </remarks>
        public int Execute(UpdateQuery query)
        {
            var renderer = new QueryRenderer(this);
            renderer.SchemaMapping = SchemaMapping;

            var sql = renderer.GetSql(query);
			long st = DateTime.Now.Ticks;
            if (Log.IsDebugEnabled) Log.Debug(string.Format("[QueryExecuteUpdate] {0}.", sql) + Environment.NewLine + renderer.PrintParameters());
            var parameters = renderer.ParameterList;

            IDbCommand command = CreateCommand(sql, parameters);

            int result = command.ExecuteNonQuery();
			if (Log.IsDebugEnabled) Log.Debug("[QueryExecuteUpdate] " + (DateTime.Now.Ticks - st) / TimeSpan.TicksPerMillisecond + "ms");
			return result;
        }

        [Obsolete("Use void adicionaControlo(string id, ControlQueryDefinition controlo) instead")]
        private static void adicionaControlo(string id, string[] controlo)
        {
            if (!controlos.ContainsKey(id))
                controlos.Add(id,controlo);
        }

        /// <summary>
        /// Adds a control to the controls hastable without repeating IDs.
        /// </summary>
        /// <param name="id">Control ID</param>
        /// <param name="controlo">Query</param>
        private static void adicionaControlo(string id, ControlQueryDefinition controlo)
        {
            if (!controlQueries.ContainsKey(id))
            {
                controlQueries.Add(id, controlo);
            }
        }

        /// <summary>
        /// This is the signature that any persistent support factory must implement in order to instance the correct class
        /// </summary>
        /// <param name="dbType">The database provider type</param>
        /// <returns>An instance of the persistent support provider</returns>
        public delegate PersistentSupport SpFactory(DatabaseType dbType);

        /// <summary>
        /// Global registered factory. Use RegisterSpFactory method to set.
        /// </summary>
        private static SpFactory m_spfactory;

        /// <summary>
        /// Registers a the factory method for concrete sp implementations
        /// </summary>
        /// <param name="factory">The factory function</param>
        public static void RegisterSpFactory(SpFactory factory)
        {
            m_spfactory = factory;
        }

        /// <summary>
        /// Check the database service for connectivity
        /// </summary>
        /// <param name="id">The datasystem to connect to</param>
        /// <returns>True if a connection is available, false otherwise</returns>
        public static bool TestDBConnection(string id)
        {
            try
            {
                var sp = getPersistentSupport(id, timeout: 1);
                sp.openConnection();
                sp.closeConnection();
                return true;
            }
            catch (Exception)
            {
                // Ignorar
            }
            return false;
        }

        /// <summary>
        /// Gets the default backups location.
        /// </summary>
        /// <returns>A string representing the path to the default backups location.</returns>
        public static string GetDefaultBackupsLocation()
        {
            return System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "dbs", "backup");
        }

        /// <summary>
        /// Creates a backup of the specified database schema for a given year.
        /// </summary>
        /// <param name="year">The year identifier for the database configuration.</param>
        /// <param name="username">The username for the database connection.</param>
        /// <param name="password">The password for the database connection.</param>
        /// <param name="location">Optional. The directory path where the backup file will be saved. 
        /// If not provided, the default backup location will be used.</param>
        /// <returns>The full path to the created backup file.</returns>
        /// <exception cref="PersistenceException">Thrown when there is an error during the backup process.</exception>
        public static string Backup(string year, string username, string password, string location = "")
        {
            try
            {
                DataSystemXml dataSystem = Configuration.ResolveDataSystem(year, Configuration.DbTypes.NORMAL);
                var sp = getPersistentSupportMaster(year, username, password);

                return sp.Backup(dataSystem.Schemas[0].Schema, location);
            }
            catch (FrameworkException ex)
            {
                if (ex.UserMessage == null)
                    throw new PersistenceException("Erro ao criar o backup.", "PersistentSupport.Backup", "Error while backing up the database: " + ex.Message, ex);
                else
                    throw new PersistenceException("Erro ao criar o backup: " + ex.UserMessage, "PersistentSupport.Backup", "Error while backing up the database: " + ex.Message, ex);
            }
        }

        /// <summary>
        /// Performs the backup operation for the specified database schema.
        /// </summary>
        /// <param name="schema">The name of the database schema to back up.</param>
        /// <param name="location">Optional. The directory path where the backup file will be saved. 
        /// If not provided, the default backup location will be used.</param>
        /// <returns>The full path to the created backup file.</returns>
        public virtual string Backup(string schema, string location = "")
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Restores the database by the backup indicated in the path variable
        /// </summary>
        /// <param name="year">Year</param>
        /// <param name="username">User db</param>
        /// <param name="password">password</param>
        /// <param name="path">path to the backup of the db</param>
        public static void Restore(string year, string username, string password, string path)
        {
            try
            {
                DataSystemXml dataSystem = Configuration.ResolveDataSystem(year, Configuration.DbTypes.NORMAL);
                var sp = getPersistentSupportMaster(year, username, password);

                sp.Restore(dataSystem.Schemas[0].Schema, path);
            }
            catch (FrameworkException ex)
            {
				if (ex.UserMessage == null)
					throw new PersistenceException("Erro ao restaurar base de dados.", "PersistentSupport.Restore", "Error restoring the database: " + ex.Message, ex);
				else
					throw new PersistenceException("Erro ao restaurar base de dados: " + ex.UserMessage, "PersistentSupport.Restore", "Error restoring the database: " + ex.Message, ex);
            }
        }

        /// <summary>
        /// Restores a database from a specified backup file.
        /// </summary>
        /// <param name="schema">The name of the target database to be restored.</param>
        /// <param name="path">The full path to the backup file.</param>
        public virtual void Restore(string schema, string path)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Path to the reindex profiling log
        /// </summary>
        public static string LogReindexPath(string path = "")
        {
            if (string.IsNullOrEmpty(path))
                path = AppDomain.CurrentDomain.BaseDirectory;

            return System.IO.Path.Combine(path, "temp", "logReindex.xml");
        }

        /// <summary>
        /// Database Reindex
        /// </summary>
        public static void upgradeSchema(string year, string username, string password, List<RdxScript> orderExec, string defPath = "", string confPath = "")
        {
            if(string.IsNullOrEmpty(defPath))
                defPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Scripts", Configuration.Program, "_ReIdx", "Reindex");

            upgradeSchema(year, username, password, orderExec, defPath, "", true);
        }

		/// <summary>
        /// Database Reindex
        /// </summary>
        public static void upgradeSchema(string year, string username, string password, List<RdxScript> orderExec, string path, string dirFilestream, bool zero)
        {
            RdxParamUpgradeSchema param = new RdxParamUpgradeSchema();
            param.Year = year;
            param.Username = username;
            param.Password = password;
            param.OrderExec = orderExec;
            param.Path = path;
            param.DirFilestream = dirFilestream;
            param.Zero = zero;
            param.Origin = "External";

            upgradeSchema(param);
        }

        /// <summary>
        /// Database Reindex
        /// </summary>
        public static void upgradeSchema(RdxParamUpgradeSchema param, CancellationToken cToken)
        {
            DataSystemXml dataSystem = Configuration.ResolveDataSystem(param.Year, Configuration.DbTypes.NORMAL);
            upgradeSchema(param, dataSystem, cToken);
        }

        /// <summary>
        /// Database Reindex
        /// </summary>
        public static void upgradeSchema(RdxParamUpgradeSchema param)
        {
            DataSystemXml dataSystem = Configuration.ResolveDataSystem(param.Year, Configuration.DbTypes.NORMAL);
            upgradeSchema(param, dataSystem, new CancellationToken());
        }

        /// <summary>
        /// Database Reindex
        /// </summary>
        public static void upgradeSchema(RdxParamUpgradeSchema param, DataSystemXml dataSystem, string auxSrcDBSchema = null)
        {
            upgradeSchema(param, dataSystem, new CancellationToken(), auxSrcDBSchema);
        }

        /// <summary>
        /// Database Reindex
        /// </summary>
        /// <param name="param">Reindex Upgrade Schema Parameters</param>
        /// <param name="dataSystem">Target DataSystem</param>
        /// <param name="auxSrcDBSchema">Aux Database schema (W_GnSrcBD) - Used in year change scripts.</param>
        public static void upgradeSchema(RdxParamUpgradeSchema param, DataSystemXml dataSystem, CancellationToken cToken, string auxSrcDBSchema = null)
        {
            if (param == null || dataSystem == null)
                return;
            string schema    = dataSystem.Schemas[0].Schema;
            string schemaLog = dataSystem.DataSystemLog != null && dataSystem.DataSystemLog.Schemas.Count != 0 ? dataSystem.DataSystemLog.Schemas[0].Schema : "";

            int year = 0;
            if (!int.TryParse(param.Year, out year))
                param.Year = "0";

			//change some parameters to replace depend datatype
            /*DatabaseType actualType = (DatabaseType)Enum.Parse(typeof(DatabaseType), dataSystem.Type, true);
            var schemaDB = schema;
            if (actualType == DatabaseType.ORACLE)
                if (schemaDB.IndexOf("C##") != 0)
                    schemaDB = "C##" + schemaDB; //on oracle 12 the user must start with C##*/

            //configure the list of replaces to the scripts
            List<KeyValuePair<string, string>> result = new List<KeyValuePair<string, string>>();
            result.Add(new KeyValuePair<string, string>("W_GnBD", schema));
            result.Add(new KeyValuePair<string, string>("W_GnTBS", schema));
            result.Add(new KeyValuePair<string, string>("W_GnIDX_TBS", schema));
            result.Add(new KeyValuePair<string, string>("W_GnAppSigla", CSGenio.framework.Configuration.Acronym));
            result.Add(new KeyValuePair<string, string>("W_GnReUse", "0"));
            result.Add(new KeyValuePair<string, string>("W_GnLogBD", schemaLog));

            result.Add(new KeyValuePair<string, string>("W_GnPSW", param.Password));
            result.Add(new KeyValuePair<string, string>("W_PathFS", param.DirFilestream));
            result.Add(new KeyValuePair<string, string>("W_AppAno", param.Year));
            result.Add(new KeyValuePair<string, string>("W_GnAppAno", param.Year));
            result.Add(new KeyValuePair<string, string>("W_GnZeroTrue", param.Zero ? "1" : "0")); //this must be chosen by the user

            if(!string.IsNullOrEmpty(auxSrcDBSchema)) // Source BD used in Qyear change scripts
                result.Add(new KeyValuePair<string, string>("W_GnSrcBD", auxSrcDBSchema));

            string logfile = LogReindexPath();
            if(System.IO.File.Exists(logfile))
            {
                var fi = new System.IO.FileInfo(logfile);
                if (fi.Length > 10 * 1024 * 1024) //10 mb
                {
                    string logbackupfile = System.IO.Path.Combine(
                        System.IO.Path.GetDirectoryName(logfile),
                        System.IO.Path.GetFileNameWithoutExtension(logfile) +
                        DateTime.Now.ToString("yyyyMMdd_HHmm") + "." +
                        System.IO.Path.GetExtension(logfile)
                    );
                    System.IO.File.Move(logfile, logbackupfile);
                }
            }
            var eq = new ExecuteQueryCore.ExecuteQueryWorker(param.Path, param.OrderExec, result, ExecuteQueryCore.NewLineFormat.Unix, logfile);

            try
            {
                using(IDbConnection conn = getPersistentSupport(dataSystem.Name, param.Username, param.Password).Connection,
                    AdmConn = getPersistentSupportMaster(dataSystem.Name, param.Username, param.Password).Connection,
                    LogConn = getPersistentSupportLog(dataSystem.Name, param.Username, param.Password).Connection)
                {
                    RdxParamExecuteServer paramEx = new RdxParamExecuteServer();
                    paramEx.Conn = conn;
                    paramEx.AdmConn = AdmConn;
                    paramEx.LogConn = LogConn;
                    paramEx.ContinueAfterError = false;
                    paramEx.Origin = param.Origin;

                    paramEx.ChangedExecuteServer += (sender, eventArgs, status) =>
                    {
                        param.OnChangedExecutionScript(EventArgs.Empty, status);
                    };

                    eq.ExecuteServer(paramEx, cToken);
				}
            }
            catch (OperationCanceledException e) { throw e; }
			catch (GenioException ex)
			{
				if (ex.UserMessage == null)
					throw new PersistenceException("Erro ao atualizar o schema da base de dados.", "PersistentSupport.upgradeSchema", "Error upgrading database schema: " + ex.Message, ex);
				else
					throw new PersistenceException(ex.UserMessage, "PersistentSupport.upgradeSchema", "Error upgrading database schema: " + ex.Message, ex);
			}
            catch (Exception ex)
            {
                throw new PersistenceException("Erro ao atualizar o schema da base de dados.", "PersistentSupport.upgradeSchema", "Error upgrading database schema: " + ex.Message, ex);
            }
        }

		/// <summary>
        /// Transfer MSMQ log data from the system DB to the system log DB
        /// Called from log database PersistentSupport
        /// </summary>
        /// <param name="all">True to transfer all log data</param>
		public virtual void transferMSMQLog(bool all)
        {
			throw new NotImplementedException();
		}

        /// <summary>
        /// Transfer log data from the system DB to the system log DB
        /// Called from log database PersistentSupport
        /// </summary>
        /// <param name="all">True to transfer all log data</param>
        /// <param name="job">The transfer log job.</param>
        public virtual void transferLog(bool all, ExecuteQueryCore.TransferLogOperation job)
        {
            // Get system database PersistentSupport
            PersistentSupport systemSp = PersistentSupport.getPersistentSupport(this.SchemaMapping.Name);

            string table = "log" + Configuration.Program + "all";

            // Filter rows by date (specified in configuration file)
            CriteriaSet filter    = CriteriaSet.And();
            CriteriaSet filterMem = CriteriaSet.And();
            if (!all && Configuration.MaxLogRowDays > 0)
            {
                DateTime lastDate = DateTime.Today.AddDays(-Configuration.MaxLogRowDays);
                filter.LesserOrEqual(table, "date", lastDate);
                filterMem.LesserOrEqual(CSGenioAmem.FldAltura, lastDate);
            }

            try
            {
                // Open transactions
                systemSp.openTransaction();
                this.openTransaction();

                // ----------------------------------------------
                // LogGENall transfer
                // ----------------------------------------------

                // Row count
                SelectQuery query = new SelectQuery()
                    .Select(SqlFunctions.Count("1"), "count")
                    .From(table)
                    .Where(filter);

                DataMatrix values = systemSp.Execute(query);
                int count = values.GetInteger(0, 0);

                if (count <= 0)
                    throw new PersistenceException("Erro durante a transferência de logs.", "PersistentSupport.transferLog", "No logs to transfer.");

                int page = 1;

                // Insert rows into log database
                while (count > 0)
                {
                    query = new SelectQuery()
                        .Select(table, "cod")
                        .Select(table, "date")
                        .Select(table, "who")
                        .Select(table, "op")
                        .Select(table, "logtable")
                        .Select(table, "logfield")
                        .Select(table, "val")
                        .PageSize(10000)
                        .Page(page)
                        .From(table)
                        .Where(filter)
                        .OrderBy(2, SortOrder.Ascending);

                    values = systemSp.Execute(query);

                    if (values.NumRows <= 0)
                        break;

                    for (int i = 0; i < values.NumRows; i++)
                    {
                        InsertQuery insert = new InsertQuery()
                            .Into(table)
                            .Value("cod",       values.GetString(i, 0))
                            .Value("date",      values.GetDate(i, 1))
                            .Value("who",       values.GetString(i, 2))
                            .Value("op",        values.GetString(i, 3))
                            .Value("logtable",  values.GetString(i, 4))
                            .Value("logfield",  values.GetString(i, 5))
                            .Value("val",       values.GetString(i, 6));

                        this.Execute(insert);
                    }

                    count -= values.NumRows;
                    page++;
                }

                // Delete rows from system database
                DeleteQuery delete = new DeleteQuery()
                    .Delete(table)
                    .Where(filter);

                systemSp.Execute(delete);

                // ----------------------------------------------
                // MEM transfer
                // ----------------------------------------------

                query = new SelectQuery()
                    .Select(SqlFunctions.Count("1"), "count")
                    .From(Area.AreaMEM)
                    .Where(filterMem);

                values = systemSp.Execute(query);
                count = values.GetInteger(0, 0);
                page = 1;

                // Insert rows into log database
                while (count > 0)
                {
                    query = new SelectQuery()
                        .Select(CSGenioAmem.FldCodmem)
                        .Select(CSGenioAmem.FldLogin)
                        .Select(CSGenioAmem.FldAltura)
                        .Select(CSGenioAmem.FldRotina)
                        .Select(CSGenioAmem.FldObs)
                        .Select(CSGenioAmem.FldHostid)
                        .Select(CSGenioAmem.FldZzstate)
                        .PageSize(10000)
                        .Page(page)
                        .From(Area.AreaMEM)
                        .Where(filterMem)
                        .OrderBy(CSGenioAmem.FldAltura, SortOrder.Ascending);

                    values = systemSp.Execute(query);

                    if (values.NumRows <= 0)
                        break;

                    for (int i = 0; i < values.NumRows; i++)
                    {
                        InsertQuery insert = new InsertQuery()
                            .Into(Area.AreaMEM.Table)
                            .Value(CSGenioAmem.FldCodmem,   values.GetString(i, 0))
                            .Value(CSGenioAmem.FldLogin,    values.GetString(i, 1))
                            .Value(CSGenioAmem.FldAltura,   values.GetDate(i, 2))
                            .Value(CSGenioAmem.FldRotina,   values.GetString(i, 3))
                            .Value(CSGenioAmem.FldObs,      values.GetString(i, 4))
                            .Value(CSGenioAmem.FldHostid,   values.GetString(i, 5))
                            .Value(CSGenioAmem.FldZzstate,  values.GetString(i, 6));
                        this.Execute(insert);
                    }

                    count -= values.NumRows;
                    page++;
                }

                // Delete rows from system database
                delete = new DeleteQuery()
                    .Delete(Area.AreaMEM)
                    .Where(filterMem);

                systemSp.Execute(delete);

                // ----------------------------------------------

                // Close transactions
                systemSp.closeTransaction();
                this.closeTransaction();
            }
			catch (GenioException ex)
			{
				// Rollback transactions
                systemSp.rollbackTransaction();
                this.rollbackTransaction();
				if (ex.ExceptionSite == "PersistentSupport.transferLog")
					throw;
				if (ex.UserMessage == null)
					throw new PersistenceException("Erro durante a transferência de logs.", "PersistentSupport.transferLog", "Error transfering log data from the database to the log: " + ex.Message, ex);
				else
					throw new PersistenceException("Erro durante a transferência de logs: " + ex.UserMessage, "PersistentSupport.transferLog", "Error transfering log data from the database to the log: " + ex.Message, ex);
			}
            catch (Exception ex)
            {
                // Rollback transactions
                systemSp.rollbackTransaction();
                this.rollbackTransaction();
                throw new PersistenceException("Erro durante a transferência de logs.", "PersistentSupport.transferLog", "Error transfering log data from the database to the log: " + ex.Message, ex);
            }
        }

        /// <summary>
        /// Method to return the persistent support subclass depending on the type of connection
        /// </summary>
        /// <param name="id">Datasystem identificator</param>
        /// <param name="user">User name to use as audit</param>
        /// <param name="readOnly">is the connection only reading data and never writing</param>
        /// <param name="dbType">the category of datasystem being requested</param>
        /// <param name="timeout">set a specific timeout for database connection to be established</param>
        /// <returns>Returns an instance of PersistentSupport</returns>
        public static PersistentSupport getPersistentSupport(string id, string user = null, string password = null, bool readOnly = false, Configuration.DbTypes dbType = Configuration.DbTypes.NORMAL, int timeout = 0)
        {
            var ds = Configuration.ResolveDataSystem(id, dbType);
            var res = m_spfactory(ds.GetDatabaseType());
            res.DatabaseType = ds.GetDatabaseType();
            res.Id = id;
            res.ClientId = user;
            res.ReadOnly = readOnly;
            res.MapSchemas(ds);
            res.BuildConnection(ds, user, password, connectionTimeout:timeout);
            return res;
        }

        /// <summary>
        /// Method to return the persistent support subclass depending on the type of connection
        /// </summary>
        /// <param name="id">The auxiliary database id</param>
        /// <returns>Returns an instantiation of PersistentSupport</returns>
        public static PersistentSupport getPersistentSupportAux(string id, string user = null, string password = null)
        {
            return getPersistentSupport(id, user, password, false, Configuration.DbTypes.AUXILIAR);
        }

        /// <summary>
        /// Method to return the persistent support subclass depending on the type of connection
        /// </summary>
        /// <param name="id">The auxiliary database id</param>
        /// <returns>Returns an instantiation of PersistentSupport</returns>
        public static PersistentSupport getPersistentSupportLog(string id, string user = null, string password = null)
        {
            return getPersistentSupport(id, user, password, false, Configuration.DbTypes.LOG);
        }

        /// <summary>
        /// Method to return a master level permission persistent support depending on the type of connection
        /// </summary>
        /// <param name="id">The id of the datasystem to connect to</param>
        /// <param name="login">The database user login</param>
        /// <param name="password">the database user password</param>
        /// <returns>Returns an instance of PersistentSupport</returns>
        public static PersistentSupport getPersistentSupportMaster(string id, string login, string password)
        {
            var ds = Configuration.ResolveDataSystem(id, Configuration.DbTypes.NORMAL);
            var res = m_spfactory(ds.GetDatabaseType());
            res.DatabaseType = ds.GetDatabaseType();
            res.Id = id;
            res.ClientId = login;
            res.IsMaster = true;
            res.MapSchemas(ds);
            res.BuildConnection(ds, login, password);
            return res;
        }

        private void MapSchemas(DataSystemXml ds)
        {
            SchemaMapping.Name = ds.Name;
            foreach (DataXml schema in ds.Schemas)
                SchemaMapping.AddMapping(schema.Id.ToUpperInvariant(), TransformSchemaName(schema.Schema));
        }

        /// <summary>
        /// Build the connection to the database according to the instanced provider
        /// </summary>
        /// <param name="dataSystem">The datasystem metadata</param>
        /// <param name="login">An optional login override to the database. Null to use the datasystem specified login.</param>
        /// <param name="password">An optional password override to the database. Null to use the datasystem specified password.</param>
        /// <param name="connectionTimeout">The connection establishment timeout. 0 to use the database default</param>
        protected abstract void BuildConnection(DataSystemXml dataSystem, string login = null, string password = null, int connectionTimeout = 0);

        /// <summary>
        /// Takes a schema name and formats in a way that the instanced provider will accept.
        /// The schema represents the database qualified name.
        /// </summary>
        /// <param name="schema">the schema name</param>
        /// <returns>The transformed schema name</returns>
        protected virtual string TransformSchemaName(string schema)
        {
            return schema;
        }

        /// <summary>
        /// Function that opens a connection
        /// </summary>
        public virtual void openConnection()
        {
            try
            {
                if (Connection.State != ConnectionState.Open)
                {
                    Log.Debug("Abre a conexão à base de dados.");
                    Connection.Open();
                }
            }
            catch (Exception ex)
            {
                throw new PersistenceException("Não foi possível estabelecer ligação à base de dados.", "PersistentSupport.openConnection", "Error opening connection: " + ex.Message, ex);
            }
        }


        /// <summary>
        /// Method to close a connection
        /// </summary>
        public virtual void closeConnection()
        {
            try
            {
                if (Connection.State != ConnectionState.Closed)
                {
				    SendDeferedQueues();
                    SendDeferredMessages();
                    Log.Debug("Fecha a conexão à base de dados.");
                    Connection.Close();
                }
            }
            catch (Exception ex)
            {
                throw new PersistenceException("Erro ao fechar a ligação à base de dados.", "PersistentSupport.closeConnection", "Error closing connection: " + ex.Message, ex);
            }
            finally
            {
                ClearDeferedQueues();
                ClearDeferredMessages();
            }
        }

        /// <summary>
        /// Opens a transaction
        /// </summary>
        public virtual void openTransaction()
        {
            try
            {
			    // [RC] 17/05/2017 - There is no need for this test here because the function openConnection already tests for opened connections
                //if (Connection.State != ConnectionState.Open) //check if the connection is open
                openConnection();
                if (Transaction == null)
                {
                    Log.Debug("Inicia a transacção à base de dados.");
                    Transaction = Connection.BeginTransaction();
                }
            }
            catch (Exception ex)
            {
                rollbackTransaction();
                throw new PersistenceException("Falha na ligação à base de dados.", "PersistentSupport.abrirTransaccao", "Error begining transaction: " + ex.Message, ex);
            }
        }


        /// <summary>
        /// Closes a transaction
        /// </summary>
        public virtual void closeTransaction()
        {
            try
            {
                if (Transaction != null)
                {
                    Log.Debug("commit da transacção à base de dados.");
                    SendDeferredMessages();
                    Transaction.Commit();
					SendDeferedQueues();
                    Transaction.Dispose();
                    Transaction = null;
					ClearDeferedQueues();
                    ClearDeferredMessages();
                    closeConnection();
                }
            }
            catch (Exception ex)
            {
                rollbackTransaction();
                throw new PersistenceException("Falha na ligação à base de dados.", "PersistentSupport.fecharTransaccao", "Error ending transaction: " + ex.Message, ex);
            }
			finally
            {
                ClearDeferedQueues();
                ClearDeferredMessages();
            }
        }

        /// <summary>
        /// Rollback a transaction
        /// </summary>
        public virtual void rollbackTransaction()
        {
            try
            {
                if (Transaction != null)
                {
                    Log.Debug("rollback da transacção à base de dados.");
                    Transaction.Rollback();
                }
            }
            catch (Exception ex)
            {
                //Don't rethrow exception here, if the rollback fails there is nothing more we can do other than close the connection.
                //Throwing exception here leads to code with infinite chains of try catch to handle the rollback exception of the rollback exception of the....
                Log.Error("Error rolling back transaction: " + ex.Message);
            }
            finally
            {
				ClearDeferedQueues();
                ClearDeferredMessages();
                closeConnection();
				if (Transaction != null)
                {
					Transaction.Dispose();
					Transaction = null;
				}
            }
        }

		private List<IArea> m_deferedQueues = new List<IArea>();

        /// <summary>
        /// Defers the insert of the queue message in the database until the whole transaction is commited
        /// </summary>
        /// <param name="mqqueue">The message to insert in the queue</param>
        public void DeferQueueToCommit(Area mqqueue)
        {
            m_deferedQueues.Add(mqqueue);
        }

        /// <summary>
        /// Clears the list of defered queues
        /// </summary>
        public void ClearDeferedQueues()
        {
            m_deferedQueues.Clear();
        }

        /// <summary>
        /// Send all the defered queues sql commands to the database
        /// </summary>
        public void SendDeferedQueues()
        {
            foreach (var q in m_deferedQueues)
                insertPseud(q);
        }

        //---------------------------------------------------------
        private class DeferedMessageEntry
        {
            public PublisherMetadata pub;
            public AreaDataset dataset;
        }

        private List<DeferedMessageEntry> m_deferedMessages = new List<DeferedMessageEntry>();

        private AreaDatasetTable GetDeferedDatatable(PublisherMetadata pub, PublisherTable table)
        {
            //if there isnt a dataset for this publisher yet, create one
            var entry = m_deferedMessages.Find(x => x.pub == pub);
            if (entry == null)
            {
                entry = new DeferedMessageEntry
                {
                    pub = pub,
                    dataset = new AreaDataset()
                };
                m_deferedMessages.Add(entry);
            }

            //ensure the table is added to the dataset
            return entry.dataset.AddTable(table.Table);
        }

        /// <summary>
        /// Defers the update message to be send when the transaction is commited
        /// </summary>
        /// <param name="pub">The publication of the message</param>
        /// <param name="table">The table being updated</param>
        /// <param name="row">The row to update</param>
        public void DeferMessageUpdate(PublisherMetadata pub, PublisherTable table, Area row)
        {
            AreaDatasetTable dst = GetDeferedDatatable(pub, table);

            // if the row is already in the dataset rows update it   
            if (dst.Updated.ContainsKey(row.QPrimaryKey))
                dst.Updated[row.QPrimaryKey] = row;
            // if the row is already in the dataset rows add the row
            else
                dst.Updated.Add(row.QPrimaryKey, row);
        }

        /// <summary>
        /// Defers the delete message to be send when the transaction is commited
        /// </summary>
        /// <param name="pub">The publication of the message</param>
        /// <param name="table">The table being deleted</param>
        /// <param name="row">The row to delete</param>
        public void DeferMessageDelete(PublisherMetadata pub, PublisherTable table, Area row)
        {
            AreaDatasetTable dst = GetDeferedDatatable(pub, table);

            // if the row is already in the updated rows we need to remove it
            dst.Updated.Remove(row.QPrimaryKey);

            // add this primary key to the list of removed rows
            if(!dst.Deleted.Contains(row.QPrimaryKey))
                dst.Deleted.Add(row.QPrimaryKey);
        }

        private void SendDeferredMessages()
        {
            MessagingService messaging = MessagingService.Instance;
            foreach (var entry in m_deferedMessages)
                messaging.SendMessage(entry.pub, entry.dataset, Id);
        }

        private void ClearDeferredMessages()
        {
            m_deferedMessages.Clear();
        }
        //---------------------------------------------------------

		public delegate void RetryableAction();

		/// <summary>
        /// Encapsulates a generic asção in a retryable transaction
        /// If the action throws a retryable exception, try to perform the action again up to a limit of 5 attempts
        /// </summary>
        /// <param name="a">The action to be taken</param>
        /// <example>
        /// sp.TransactionRetry(() => { model.Save(); });
        /// </example>
        public void TransactionRetry(RetryableAction a)
        {
            int retry = 0;
            bool sucess = false;

            while (!sucess && retry < 5)
            {
                try
                {
                    openTransaction();
                    a();
                    closeTransaction();
                    sucess = true;
                }
                catch (Exception e)
                {
                    rollbackTransaction();
                    closeConnection();

                    //search in innerExceptions for an exception of the persistent media type that is Retryable
                    bool retryable = false;
                    Exception level = e;
                    while (level != null)
                    {
                        PersistenceException ep = level as PersistenceException;
                        if(ep != null && ep.IsRetryable)
                        {
                            retryable = true;
                            break;
                        }
                        level = level.InnerException;
                    }

                    if (retryable)
                    {
                        if (CSGenio.framework.Log.IsDebugEnabled) Log.Debug("RETRY from error" + (level == null ? "" : level.Message));
                        retry++;
                    }
                    else
                    {
						if (e is GenioException)
							throw new PersistenceException((e as GenioException).UserMessage, "PersistentSupport.TransactionRetry", "Exception is not retryable: " + e.Message, e);
						else
							throw new PersistenceException(null, "PersistentSupport.TransactionRetry", "Exception is not retryable: " + e.Message, e);
                    }
                }
            }
        }

                /// <summary>
        ///Checks whether there is a record on the table with primary key equal to the given
        ///This function assumes the existence of an open connection.
        /// </summary>
        /// <param name="campo">name of the field that is the primary key of the table</param>
        /// <param name="tabela">from which we are checking whether the registration exists</param>
        /// <param name="valorCampo">primary key value</param>
        /// <param name="formatacao">if it is string, date, number,...</param>
        /// <returns>true if exists and false if it does not exist</returns>
        [Obsolete("Use 'bool Existe(string campo, string tabela, object valorCampo)' instead")]
        public bool exists(string Qfield, string table, object fieldValue, FieldFormatting formatting)
        {
            return Exists(Qfield, null, table, fieldValue);
        }

        /// <summary>
        /// Check if a record exists with a given field value
        /// </summary>
        /// <param name="Qfield">The field name</param>
        /// <param name="table">The table name</param>
        /// <param name="fieldValue">Field value</param>
        /// <returns>True if the record exits</returns>
        public bool Exists(string Qfield, string table, object fieldValue)
        {
            return Exists(Qfield, null, table, fieldValue);
        }

         /// <summary>
        /// Check if a record exists with a given field value
        /// </summary>
        /// <param name="Qfield">The field name</param>
        /// <param name="schema">The schema for this table</param>
        /// <param name="table">The table name</param>
        /// <param name="fieldValue">Field value</param>
        /// <returns>True if the record exits</returns>
        public bool Exists(string Qfield, string schema, string table, object fieldValue)
        {
            try
            {
                if (Log.IsDebugEnabled) Log.Debug(string.Format("Check if record exists. [table] {0} [field] {1} [key] {2}", table, Qfield, fieldValue));

                SelectQuery query = new SelectQuery()
                    .Select(new SqlValue(1), "exists")
                    .From(schema, table, table)
                    .Where(CriteriaSet.And()
                        .Equal(table, Qfield, fieldValue))
                        .PageSize(1);

                DataMatrix mx = Execute(query);

                return mx != null && mx.NumRows > 0;
            }
            catch (PersistenceException ex)
            {
                if (ex.UserMessage == null)
                    throw new PersistenceException("O registo não foi encontrado.", "PersistentSupport.existe",
                        "Error trying to find record with value " + fieldValue + " in field " + Qfield + " in table " + table + ": " + ex.Message, ex);
                else
                    throw new PersistenceException("The record was not found: " + ex.UserMessage, "PersistentSupport.existe",
                        "Error trying to find record with value " + fieldValue + " in field " + Qfield + " in table " + table + ": " + ex.Message, ex);
            }
            catch (Exception ex)
            {
                throw new PersistenceException("O registo não foi encontrado.", "PersistentSupport.existe",
                    "Error trying to find record with value " + fieldValue + " in field " + Qfield + " in table " + table + ": " + ex.Message, ex);
            }
        }

        [Obsolete("Use Exists(string Qfield, string schema, string table, object fieldValue) instead ")]
        public bool exists(string Qfield, string schema, string table, object fieldValue, FieldFormatting formatting)
        {
			return Exists(Qfield, schema, table, fieldValue);
        }

        /// <summary>
        ///Checks whether there is a record on the table with primary key equal to the given
        ///This function assumes the existence of an open connection.
        /// </summary>
        /// <param name="campo">name of the field that is the primary key of the table</param>
        /// <param name="tabela">from which we are checking whether the registration exists</param>
        /// <param name="valorCampo">primary key value</param>
        /// <param name="formatacao">if it is string, date, number,...</param>
        /// <returns>true if exists and false if it does not exist</returns>
        public bool exists(string[] fields, string table, object[] fieldsvalues)
        {
            return exists(fields, null, table, fieldsvalues);
        }

        private bool exists(string[] fields, string schema, string table, object[] fieldsvalues)
        {
            try
            {
              if (Log.IsDebugEnabled) Log.Debug(string.Format("Verifica se existe o registo. [tabela] {0} [campo] {1} [codigo] {2}", table, fields.ToString(), fieldsvalues.ToString()));

                SelectQuery query = new SelectQuery()
                    .Select(new SqlValue(1), "exists")
                    .From(table);
                CriteriaSet conditions = CriteriaSet.And();
                for (int i = 0; i < fields.Length; i++)
                {
                    conditions.Equal(table, fields[i], fieldsvalues[i]);
                }
                query.Where(conditions);

                DataMatrix mx = Execute(query);

                return mx != null && mx.NumRows > 0;
            }
            catch (PersistenceException ex)
            {
                if (ex.UserMessage == null)
					throw new PersistenceException("Os registos não foram encontrados.", "PersistentSupport.existe",
						"Error trying to find records in table " + table + ": " + ex.Message, ex);
				else
					throw new PersistenceException("Os registos não foram encontrados: " + ex.UserMessage, "PersistentSupport.existe",
						"Error trying to find records in table " + table + ": " + ex.Message, ex);
            }
            catch (Exception ex)
            {
                throw new PersistenceException("Os registos não foram encontrados.", "PersistentSupport.existe",
					"Error trying to find records in table " + table + ": " + ex.Message, ex);
            }
        }

        /// <summary>
        /// Determines the value of the internal code of the records of a child table that have a
        /// relationship with parent table through relatedFields.
        /// This function assumes the existence of an open connection.
        /// </summary>
        /// <param name="camposRelacionados">establish the connection of the daughter table with the mother table</param>
        /// <param name="areaFilha">instance of the daughter area</param>
        /// <param name="valorCodigoMae">primary key value of the daughter table</param>
        /// <returns>Arraylist with the built-in codes of the daughter tables</returns>
        public ArrayList existsChild(string[] relatedFields, IArea areaChild, string parentCodeValue)
        {
          try
          {
            if (Log.IsDebugEnabled) Log.Debug(string.Format("Procura registos relacionados na tabela abaixo. [tabela] {0}", areaChild.TableName));

            ArrayList Qresult = new ArrayList();

            SelectQuery query = new SelectQuery()
                .Select(areaChild.Alias, areaChild.PrimaryKeyName)
                .From(areaChild.QSystem, areaChild.TableName, areaChild.Alias);

            CriteriaSet conditions = CriteriaSet.Or();
            for (int i = 0; i < relatedFields.Length; i++)
            {
                conditions.Equal(areaChild.Alias, relatedFields[i], parentCodeValue);
            }
            query.Where(conditions);

            Qresult = executeReaderOneColumn(query);

                return Qresult;
            }
            catch (PersistenceException ex)
            {
                //closeConnection();
                if (ex.UserMessage == null)
					throw new PersistenceException("Não foi possível encontrar os registos relacionados.", "PersistentSupport.existeFilha", "Error finding related records in child table: " + areaChild.TableName + ": " + ex.Message, ex);
				else
					throw new PersistenceException("Não foi possível encontrar os registos relacionados: " + ex.UserMessage, "PersistentSupport.existeFilha", "Error finding related records in child table: " + areaChild.TableName + ": " + ex.Message, ex);
            }
            catch (Exception ex)
            {
                //closeConnection();
                throw new PersistenceException("Não foi possível encontrar os registos relacionados.", "PersistentSupport.existeFilha", "Error finding related records in child table: " + areaChild.TableName + ": " + ex.Message, ex);
            }
        }

        /// <summary>
        ///  Determines the value of the internal code of the records of a child table that have a
        ///  foreign key to the registration of the parent table.
        ///  This function assumes the existence of an open connection
        /// </summary>
        /// <param name="campoRelacionado">establishes the relationship between the mother and daughter table</param>
        /// <param name="codigoInternoFilha">primary key name of the daughter table</param>
		/// <param name="sistemaFilha">child table prefix schema</param>
        /// <param name="tabelaFilha">name of the daughter table</param>
		/// <param name="aliasFilha">alias of daughter table</param>
        /// <param name="valorCodigoMae">value of the primary key of the parent table</param>
        /// <returns>Arraylist with the built-in codes of the daughter tables</returns>
        public ArrayList existsChild(string relatedField, string childInternalCode, string childSystem, string childTable, string aliasChild, object parentCodeValue)
        {
            try
            {
                if (Log.IsDebugEnabled) Log.Debug(string.Format("Procura registo relacionado na tabela abaixo. [tabela] {0}", childTable));

                ArrayList Qresult = new ArrayList();
                //string query = queryExistsChild(relatedField, childInternalCode, childTable, aliasChild, parentCodeValue);
                //Qresult = executeReaderOneColumn(query);
                SelectQuery query = queryExistsChild(relatedField, childInternalCode, childSystem, childTable, aliasChild, parentCodeValue);
                Qresult = executeReaderOneColumn(query);
                return Qresult;
            }
            catch (PersistenceException ex)
            {
                closeConnection();
                if (ex.UserMessage == null)
					throw new PersistenceException("Não foi possível encontrar os registos relacionados.", "PersistentSupport.existeFilha", "Error finding related records in child area " + childTable + ": " + ex.Message, ex);
				else
					throw new PersistenceException("Não foi possível encontrar os registos relacionados: " + ex.UserMessage, "PersistentSupport.existeFilha", "Error finding related records in child area " + childTable + ": " + ex.Message, ex);
            }
            catch (Exception ex)
            {
                closeConnection();
                throw new PersistenceException("Não foi possível encontrar os registos relacionados.", "PersistentSupport.existeFilha", "Error finding related records in child area " + childTable + ": " + ex.Message, ex);
            }
        }

        public object returnField(IArea area, string Qfield, object codIntValue)
        {
            return returnField(area.QSystem, area.TableName, Qfield, area.PrimaryKeyName, codIntValue);
        }

        /// <summary>
        /// Returns a record field of a table that the internal code passed as a parameter
        /// Assumes an open connection
        /// </summary>
		/// <param name="schema">Prefix schema of the table to which the field belongs</param>
        /// <param name="tabela">Table to which the field belongs</param>
        /// <param name="campo">Field of the table to be returned</param>
        /// <param name="nomeCodInt">Name of the internal table code</param>
        /// <param name="valorCodInt">Value of the internal table code</param>
        /// <returns>returns the value of the field</returns>
        public object returnField(string schema, string table, string Qfield, string codIntName, object codIntValue)
        {
            try
            {
                if (Log.IsDebugEnabled) Log.Debug(string.Format("Devolve o valor do campo. [tabela] {0} [campo] {1} [codigo] {2}", table, Qfield, codIntValue));

                SelectQuery query = new SelectQuery()
                    .Select(table, Qfield)
                    .From(schema, table, table)
                    .Where(CriteriaSet.And()
                        .Equal(table, codIntName, codIntValue));

                DataMatrix mx = Execute(query);
                if (mx == null || mx.NumRows < 1)
                {
                    return null;
                }

                return mx.GetDirect(0, 0);
            }
            catch (PersistenceException ex)
            {
                closeConnection();
                throw new PersistenceException(ex.UserMessage, "PersistentSupport.devolveCampo", "Error returning field " + Qfield + " from table " + table + ": " + ex.Message, ex);

            }
            catch (Exception ex)
            {
                closeConnection();
                throw new PersistenceException(null, "PersistentSupport.devolveCampo", "Error returning field " + Qfield + " from table " + table + ": " + ex.Message, ex);
            }
        }


        public object returnFieldCondition(IArea area, string Qfield, CriteriaSet condition)
        {
            return returnFieldCondition(area.QSystem, area.TableName, Qfield, condition);
        }


        public object returnFieldCondition(string schema, string table, string Qfield, CriteriaSet condition)
        {
            try
            {
                string field = Qfield;
                if (field.IndexOf('.') != -1)
                {
                    field = field.Split('.')[1];
                }

                if (Log.IsDebugEnabled) Log.Debug(string.Format("Devolve o valor do campo. [tabela] {0} [campo] {1} [condicao] {2}", table, Qfield, condition));

                SelectQuery query = new SelectQuery()
                    .Select(table, field)
                    .From(schema, table, table)
                    .Where(condition);

                DataMatrix mx = Execute(query);
                if (mx == null || mx.NumRows < 1)
                {
                    return null;
                }

                return mx.GetDirect(0, 0);
            }
            catch (PersistenceException ex)
            {
                closeConnection();
                throw new PersistenceException(ex.UserMessage, "PersistentSupport.devolveCampo", "Error returning field " + Qfield + " from table " + table + "where " + condition.ToString() + ": " + ex.Message, ex);
            }
            catch (Exception ex)
            {
                closeConnection();
				throw new PersistenceException(null, "PersistentSupport.devolveCampo", "Error returning field " + Qfield + " from table " + table + "where " + condition.ToString() + ": " + ex.Message, ex);
            }
        }


        public ArrayList returnFieldsListConditions(string[] fieldsToGet, string table, string[] fieldsCondition, object[] fieldsvalues)
        {
            return returnFieldsListConditions(fieldsToGet, null, table, fieldsCondition, fieldsvalues);
        }

        public ArrayList returnFieldsListConditions(string[] fieldsToGet, string schema, string table, string[] fieldsCondition, object[] fieldsvalues)
        {
            try
            {
                if (Log.IsDebugEnabled) Log.Debug(string.Format("Verifica se existe o registo. [tabela] {0} [campo] {1} [codigo] {2}", table, fieldsCondition.ToString(), fieldsvalues.ToString()));

                SelectQuery query = new SelectQuery();
                foreach (string field in fieldsToGet)
                {
                    query.Select(table, field);
                }
                query.From(schema, table, table);
                CriteriaSet criteria = CriteriaSet.And();
                for (int i = 0; i < fieldsCondition.Length; i++)
                {
                    criteria.Equal(table, fieldsCondition[i], fieldsvalues[i]);
                }
                query.Where(criteria);

                return executeReaderOneRow(query);
            }
            catch (PersistenceException ex)
            {
                throw new PersistenceException(ex.UserMessage, "PersistentSupport.devolveCamposListaCondicoes", "Error returning fields " + fieldsvalues.ToString() + " from table " + table + "where " + fieldsCondition.ToString() + ": " + ex.Message, ex);
            }
            catch (Exception ex)
            {
				throw new PersistenceException(null, "PersistentSupport.devolveCamposListaCondicoes", "Error returning fields " + fieldsvalues.ToString() + " from table " + table + "where " + fieldsCondition.ToString() + ": " + ex.Message, ex);
            }
        }

        /// <summary>
        /// Returns a record field of a table that the internal code passed as a parameter
        /// Assumes an open connection
        /// </summary>
        /// <param name="area">Area to consult</param>
        /// <param name="listaCamposcampo">Fields of the table to be returned</param>
        /// <param name="nomeCodInt">Name of the internal table code</param>
        /// <param name="valorCodInt">Value of the internal table code</param>
        /// <returns>returns the value of the fields</returns>
        public ArrayList returnFields(IArea area, SelectField[] fieldsList, string codIntName, object codIntValue)
        {
            try
            {
                if (Log.IsDebugEnabled) Log.Debug(string.Format("Devolve o valor dos campos. [tabela] {0} [campos] {1} [codigo] {2}", area.TableName, fieldsList, codIntValue));

                SelectQuery query = new SelectQuery();
                foreach (SelectField field in fieldsList)
                {
                    query.SelectFields.Add(field);
                }
                query.From(area.QSystem, area.TableName, area.TableName)
                    .Where(CriteriaSet.And()
                        .Equal(area.TableName, codIntName, codIntValue));

                return executeReaderOneRow(query);
            }
            catch (PersistenceException ex)
            {
                closeConnection();
                throw new PersistenceException(ex.UserMessage, "PersistentSupport.devolveCampos", "Error returning fields " + fieldsList + " from table " + area.TableName + "where " + codIntName + "=" + codIntValue.ToString() + ": " + ex.Message, ex);
            }
            catch (Exception ex)
            {
                closeConnection();
				throw new PersistenceException(null, "PersistentSupport.devolveCampos", "Error returning fields " + fieldsList + " from table " + area.TableName + "where " + codIntName + "=" + codIntValue.ToString() + ": " + ex.Message, ex);
            }
        }

        public DataMatrix returnValuesDocums(IArea area, string fieldName, SelectField[] Qvalues, CriteriaSet condition)
        {
            return returnValuesDocums(area, fieldName, Qvalues, condition, null);
        }

        public DataMatrix returnValuesDocums(IArea area, string fieldName, SelectField[] Qvalues, CriteriaSet condition, ColumnSort[] order)
        {
            return returnValuesDocums(area, fieldName, true, Qvalues, condition, order);
        }

        public DataMatrix returnValuesDocums(IArea area, string fieldName, bool isForeignKey, SelectField[] Qvalues, CriteriaSet condition, ColumnSort[] order)
        {
            DataMatrix res = null;
            string tabelaDocums = "docums";
            object primaryKeyValue = area.returnValueField(area.Alias + "." + area.PrimaryKeyName);
            object chaveDocums = null;

            string dbFieldName = fieldName;
            if (isForeignKey)
                dbFieldName += "fk";

            if (area.DBFields.ContainsKey(dbFieldName))
            {
                chaveDocums = area.returnValueField(area.Alias + "." + dbFieldName);
            }
            else
            {
                SelectQuery qs1 = new SelectQuery()
                    .Select(area.Alias, dbFieldName)
                    .From(area.QSystem, area.TableName, area.Alias)
                    .Where(CriteriaSet.And()
                        .Equal(area.Alias, area.PrimaryKeyName, primaryKeyValue));

                DataMatrix mx = Execute(qs1);
                if (mx != null && mx.NumRows > 0)
                {
                    chaveDocums = mx.GetDirect(0, 0);
                }
            }

            if (chaveDocums == null || chaveDocums == DBNull.Value || chaveDocums.Equals(""))
            {
                return res;
            }
            else
            {
                SelectQuery qs2 = new SelectQuery();
                foreach (SelectField field in Qvalues)
                {
                    qs2.SelectFields.Add(field);
                }
                qs2.From(tabelaDocums, "docums")
                    .Where(condition);
                if (order != null)
                {
                    foreach (ColumnSort sort in order)
                    {
                        qs2.OrderByFields.Add(sort);
                    }
                }

                res = Execute(qs2);

                return res;
            }
        }

        public object returnValueDocums(IArea area, string fieldName)
        {
            string tabelaDocums = "docums";
            object primaryKeyValue = area.returnValueField(area.Alias + "." + area.PrimaryKeyName);
            Object chaveDocums = null;

            if (area.Fields.ContainsKey(fieldName + "fk"))
            {
                chaveDocums = area.returnValueField(area.Alias + "." + fieldName + "fk");
            }
            else
            {

                SelectQuery qs1 = new SelectQuery()
                    .Select(area.Alias, fieldName + "fk")
                    .From(area.QSystem, area.TableName, area.Alias)
                    .Where(CriteriaSet.And()
                        .Equal(area.Alias, area.PrimaryKeyName, primaryKeyValue));

                DataMatrix mx = Execute(qs1);
                if (mx != null && mx.NumRows > 0)
                {
                    chaveDocums = mx.GetDirect(0, 0);
                }
            }

            if (chaveDocums == null || chaveDocums == DBNull.Value || chaveDocums.Equals(""))
            {
                return "";
            }
            else
            {
                SelectQuery qs2 = new SelectQuery()
                    .Select(tabelaDocums, "document")
                    .From(tabelaDocums)
                    .Where(CriteriaSet.And()
                        .Equal(tabelaDocums, "coddocums", chaveDocums));

                DataMatrix mx = Execute(qs2);

                if (mx == null || mx.NumRows < 1 || mx.GetDirect(0, 0) == null || mx.GetDirect(0, 0) == DBNull.Value)
                {
                    return "";
                }
                else
                {
                    return mx.GetDirect(0, 0);
                }
            }
        }

        /// <summary>
        /// Determines the value of the internal code of the records of a child table that
        /// have a relationship with the parent table through a related fields.
        /// This function assumes the existence of an open connection.
        /// </summary>
        /// <param name="camposRelacionado">establishes the connection of the daughter table with the mother table</param>
        /// <param name="codigoInternoFilha">name of the field that is primary key of the daughter table</param>
        /// <param name="tabelaFilha">name of the daughter table</param>
		/// <param name="aliasFilha">alias of daughter table</param>
        /// <param name="valorCodigoMae">primary key value of the daughter table</param>
        /// <returns>string corresponding to SQL question</returns>
        public SelectQuery queryExistsChild(string relatedField, string childInternalCode, string childTable, string aliasChild, object parentCodeValue)
        {
            return queryExistsChild(relatedField, childInternalCode, null, childTable, aliasChild, parentCodeValue);
        }

        public SelectQuery queryExistsChild(string relatedField, string childInternalCode, string childSystem, string childTable, string aliasChild, object parentCodeValue)
        {
            SelectQuery query = new SelectQuery()
                .Select(aliasChild, childInternalCode)
                .From(childSystem, childTable, aliasChild)
                .Where(CriteriaSet.And()
                    .Equal(aliasChild, relatedField, parentCodeValue));

            return query;
        }

        /// <summary>
        /// Method to return EPH values depending on user (level and password)
        /// </summary>
        /// <param name="codpsw">internal user code on psw table</param>
        /// <param name="condicoes">array with EPH conditions</param>
        /// <param name="modulo">module to consider</param>
        /// <returns>list with the conditions to each EPH to this user</returns>
        public string[] ValuesEPH(string codpsw, EPHCondition condition, string module)
        {
            //AV 20091229 Redone conditions to allow Tree EPH, with multiple values and
            //applied to different key fields
            try
            {
                List<string> Qvalues = new List<string>();

                // constructs the query to interrogate the BD about the value of the EPH limiter field
                SelectQuery query = new SelectQuery();
                if (condition.TableName == condition.EPHTable)
                {
                    query.Select(condition.AliasTable, condition.EPHField);
                }
                else
                {
                    query.Select(condition.AliasTable, condition.RelationField);
                }

                query.From(condition.TableSystem, condition.TableName, condition.AliasTable);
                query.Where(CriteriaSet.And()
                    .Equal(condition.AliasTable, "codpsw", codpsw));
                ArrayList valorChaves = executeReaderOneColumn(query);

                if (valorChaves != null)
                {
                    // MH [10/25/2016] - EPHTable may contain the long table name which is not supported by GetInfoArea because it will look for a CSGenioA class
                    AreaInfo tabelaEPH = Area.GetInfoArea(condition.EPHTable/*.substring(3)*/);

                    if (condition.TableName == tabelaEPH.TableName)
                        foreach (object chaveBD in valorChaves)
                        {
                            Qvalues.Add(DBConversion.ToString(chaveBD));
                        }
                    else
                    {
                        foreach (object chaveBD in valorChaves)
                        {
                            object key = chaveBD;
                            if (key != null && !Object.Equals(key, "") && !Object.Equals(key, Guid.Empty))
                            {
                                SelectQuery queryvalores = new SelectQuery()
                                    .Select(tabelaEPH.TableName, condition.EPHField)
                                    .From(tabelaEPH.TableName)
                                    .Where(CriteriaSet.And()
                                        .Equal(tabelaEPH.TableName, tabelaEPH.PrimaryKeyName, key));
                                ArrayList valorEPHs = executeReaderOneColumn(queryvalores);
                                // if there is no input to the user in the table that makes the interface to this EPH
                                // a condition should be placed in the session which does not allow it to view any
                                foreach (object Qvalue in valorEPHs)
                                {
                                    if (Qvalue != null)
                                        Qvalues.Add(DBConversion.ToString(Qvalue));
                                    else
                                    {
                                        Qvalues.Clear();
                                        return Qvalues.ToArray();
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    Qvalues.Clear();
                    return Qvalues.ToArray();
                }                    // another hypothesis is to throw an exception to the user's warning
                return Qvalues.ToArray();
            }
            catch (PersistenceException ex)
            {
                throw new PersistenceException(ex.UserMessage, "PersistentSupport.devolveCampos", "Error returning EPHs for user with password " + codpsw + " for module " + module + "where " + condition.ToString() + ": " + ex.Message, ex);
            }
            catch (Exception ex)
            {
				throw new PersistenceException(null, "PersistentSupport.devolveCampos", "Error returning EPHs for user with password " + codpsw + " for module " + module + "where " + condition.ToString() + ": " + ex.Message, ex);
            }
        }

		        /// <summary>
        /// Return initial eph values
        /// </summary>
        /// <param name="codpsw">internal user code on psw table</param>
        /// <param name="condicoes">array with EPH conditions</param>
        /// <param name="modulo">module to consider</param>
        /// <param name="values">values to filter</param>
        /// <returns>list with the conditions to each EPH to this user</returns>
        public string[] ValuesEphInitial(string codpsw, EPHCondition condition, string module, string[] values)
        {
            try
            {
                List<string> Qvalues = new List<string>();

                string primaryKeyName = Area.GetInfoArea(condition.EPHTable).PrimaryKeyName;
				string ligacao = primaryKeyName;

                // constructs the query to interrogate the BD about the value of the EPH limiter field
                SelectQuery query = new SelectQuery();
                if (condition.TableName == condition.EPHTable)
                {
                    if (condition.EPHField == primaryKeyName)
                        return values;

                    query.Select(condition.AliasTable, condition.EPHField);
                }
                else
                {
                    //if the connection is not the same as in the eph
                    string ligacao_nova = Area.GetInfoArea(condition.AliasTable).PrimaryKeyName;

                    if(ligacao_nova != primaryKeyName)
                        ligacao = ligacao_nova;

                    query.Select(condition.AliasTable, condition.RelationField);
                }

                query.From(condition.TableSystem, condition.TableName, condition.AliasTable);
                query.Where(CriteriaSet.And()
                    .In(condition.AliasTable, ligacao, new List<string>(values)));
                ArrayList valorChaves = executeReaderOneColumn(query);

                if (valorChaves != null)
                {
                    AreaInfo tabelaEPH = Area.GetInfoArea(condition.EPHTable);

                    if (condition.TableName == tabelaEPH.TableName)
                        foreach (object chaveBD in valorChaves)
                        {
                            Qvalues.Add(DBConversion.ToString(chaveBD));
                        }
                    else
                    {
                        foreach (object chaveBD in valorChaves)
                        {
                            object key = chaveBD;
                            if (key != null && !Object.Equals(key, "") && !Object.Equals(key, Guid.Empty))
                            {
                                SelectQuery queryvalores = new SelectQuery()
                                    .Select(tabelaEPH.TableName, condition.EPHField)
                                    .From(tabelaEPH.TableName)
                                    .Where(CriteriaSet.And()
                                        .Equal(tabelaEPH.TableName, tabelaEPH.PrimaryKeyName, key));
                                ArrayList valorEPHs = executeReaderOneColumn(queryvalores);
                                // if there is no input to the user in the table that makes the interface to this EPH
                                // a condition should be placed in the session which does not allow it to view any
                                foreach (object Qvalue in valorEPHs)
                                {
                                    if (Qvalue != null)
                                        Qvalues.Add(DBConversion.ToString(Qvalue));
                                    else
                                    {
                                        Qvalues.Clear();
                                        return Qvalues.ToArray();
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    Qvalues.Clear();
                    return Qvalues.ToArray();
                }                    // another hypothesis is to throw an exception to the user's warning
                return Qvalues.ToArray();
            }
            catch (PersistenceException ex)
            {
                throw new PersistenceException(ex.UserMessage, "PersistentSupport.devolveCampos", "Error returning EPHs for user with password " + codpsw + " for module " + module + "where " + condition.ToString() + ": " + ex.Message, ex);
            }
            catch (Exception ex)
            {
                throw new PersistenceException(null, "PersistentSupport.devolveCampos", "Error returning EPHs for user with password " + codpsw + " for module " + module + "where " + condition.ToString() + ": " + ex.Message, ex);
            }
        }

        /****************************************GETÚNICO************************************************/
        /// <summary>
        /// Function that returns a database record when it is the only one that meets the conditions
        /// </summary>
        /// <param name="condicoes">conditions for reading the register</param>
        /// <param name="area">area to which the register belongs</param>
        /// <param name="identificador">request identifier</param>
        /// <returns> area with the values of the fields given</returns>
        public void selectSingle(CriteriaSet conditions, IArea area, string identifier)
        {
            try
            {
                CriteriaSet allConditions = conditions;
                SelectQuery query = null;

                //check if there is a query override to the LED (query done in manual routine)
                if (controlQueriesOverride.ContainsKey(identifier))
                {
                    query = controlQueriesOverride[identifier](area.User, area.User.CurrentModule, conditions, null, this);
                }
                else
                {
                    //NH(01.10.2010) - Adds the conditions defined in the query to the control but removes the part of zzstate = 0
                    /*Não funcionava to os casos de um dbedit que tivesse um tipo de limitação FIXO*/
                    ControlQueryDefinition queryGenio = controlQueries[identifier];
                    CriteriaSet condicoesAux = null;

                    if (queryGenio.WhereConditions != null)
                    {
                        condicoesAux = CriteriaSet.And();
                        foreach (Criteria criteria in queryGenio.WhereConditions.Criterias)
                        {
                            // exclude ZZSTATE criteria
                            if (!(criteria.LeftTerm is ColumnReference
                                && String.Equals(((ColumnReference)criteria.LeftTerm).ColumnName, "ZZSTATE", StringComparison.InvariantCultureIgnoreCase)))
                            {
                                condicoesAux.Criterias.Add(criteria);
                            }
                        }
                        foreach (CriteriaSet subSet in queryGenio.WhereConditions.SubSets)
                        {
                            condicoesAux.SubSet(subSet);
                        }

                        if (condicoesAux.Criterias.Count > 0 || condicoesAux.SubSets.Count > 0)
                        {
                            allConditions.SubSet(condicoesAux);
                        }
                    }

                    query = querySeleccionaUm(allConditions, null, area);

                    SelectQuery queryCount = QueryUtils.buildQueryCount(query);
                    DataMatrix mx = Execute(queryCount);

                    int nr = 0;
                    if (mx != null && mx.NumRows > 0)
                    {
                        nr = mx.GetInteger(0, 0);
                    }

                    if (nr != 1)//if there are no records
                    {
                        Hashtable hresVaz = new Hashtable();
                        for (int i = 0; i < query.SelectFields.Count; i++)
                        {
                            hresVaz.Add(query.SelectFields[i].Alias, null);
                        }
                        fillAreaSelectOne(hresVaz, area);
                        return;//as it is an LED should return to empty area
                    }
                }

                ArrayList Qresult = executeReaderOneRow(query);
                Hashtable hresUnico = new Hashtable();
                for (int i = 0; i < Qresult.Count; i++)
                {
                    hresUnico.Add(query.SelectFields[i].Alias, Qresult[i]);
                }

                fillAreaSelectOne(hresUnico, area);
                return;
            }
            catch (PersistenceException ex)
            {
                throw new PersistenceException(ex.UserMessage, "PersistentSupport.seleccionaUnico",
					"Error selecting unique record from area " + area.ToString() + " where " + conditions.ToString() + ": " + ex.Message, ex);
            }
            catch (Exception ex)
            {
				throw new PersistenceException(null, "PersistentSupport.seleccionaUnico",
					"Error selecting unique record from area " + area.ToString() + " where " + conditions.ToString() + ": " + ex.Message, ex);
            }
        }

        /****************************************GET1************************************************/
        //old signature to ensure compatibility
        //[Obsolete("Please use seleccionaUm(CriteriaSet condicoes, IList<ColumnSort> ordenacao, IArea area, string identificador) instead")]
        //public void selectOne(string conditions, string sorting, IArea area, bool isled)
        //{
        //    selectOne(conditions, sorting, area, "");
        //}

        [Obsolete("Please use seleccionaUm(CriteriaSet condicoes, IList<ColumnSort> ordenacao, IArea area, string identificador) instead")]
        public void selectOne(CriteriaSet conditions, IList<ColumnSort> sorting, IArea area, bool isled)
        {
            selectOne(conditions, sorting, area, "");
        }

        /// <summary>
        /// Role that returns a database record
        /// </summary>
        /// <param name="condicoes">conditions for reading the register</param>
        /// <param name="ordenacao">ordering conditions</param>
        /// <param name="area">area to which the register belongs</param>
        /// <param name="identificador">request identifier</param>
        /// <returns> area with the values of the fields given</returns>
        [Obsolete("Use void seleccionaUm(CriteriaSet condicoes, IList<ColumnSort> ordenacao, IArea area, string identificador) instead")]
        public void selectOne(string conditions, string sorting, IArea area, string identifier)
        {
           try
           {
               QuerySelect query = null;
               //check if the request is an LED
               bool isLed = false;
               if (identifier.Length >= 3)
                   isLed = identifier.Substring(0, 3).Equals("LED");

				//in case it is an LED, check if there is a query override (query done in manual routine)
				if (isLed && controlosOverride.ContainsKey(identifier))//if it's an LED
					query = controlosOverride[identifier](area.User, area.User.CurrentModule, conditions, this);
				else
				{
					query = querySeleccionaUm(conditions, sorting, area);
					if (isLed)
					{
						string[] queryGenio = (string[])controlos[identifier];
						//NH(02.08.2010) - Does not place the condition zzstate = 0
						/*Não funcionava to os casos de inserção de registos através de um expõe table de um form em que a ficha ainda era Pseudo-nova.
							Não carimbava a relação.
						*/
						int pos = queryGenio[2].IndexOf("AND");
						if (pos!=-1)
						{
							string where = "";
							where = queryGenio[2].Substring(pos + 3);
							query.addWhere(where.Replace('*', '%'), "AND");
						}
					}

					string queryCount = query.buildQueryCount();
					object nrLinhas = executeScalar(queryCount);
					int nr = 0;
					if (nrLinhas is decimal)//in oracle is a decimal is not an int
						nr = decimal.ToInt32((decimal)nrLinhas);
					else
						nr = Convert.ToInt32(nrLinhas);
					if (nr > 1 && !isLed)
						throw new PersistenceException(null, "PersistentSupport.seleccionaUm",
							"There is more than one record in table " + area.ToString() + " satisfying the conditions " + conditions + ".");
					else if (nr == 0) // if there are no records
					{
						if (!isLed) // if it's not an LED should give error
							throw new PersistenceException(null, "PersistentSupport.seleccionaUm",
								"There are no records in table " + area.ToString() + " satisfying the conditions " + conditions + ".");
						else
							return; // if it is an LED should return to empty area
					}
				}
				query.buildQuery();
				ArrayList Qresult = executeReaderOneRow(query.Query);
				Hashtable hres = new Hashtable();
				for (int i = 0; i < Qresult.Count; i++)
				{
					hres.Add(query.SelectFields[i], Qresult[i]);
				}
				fillAreaSelectOne(hres, area);
				return;
           }
           catch (PersistenceException ex)
           {
				if (ex.ExceptionSite == "PersistentSupport.seleccionaUm")
					throw;
				throw new PersistenceException(ex.UserMessage, "PersistentSupport.seleccionaUm",
											   string.Format("Error selecting one record from area - [condicoes] {0}; [ordenacao] {1}; [area] {2}; [identificador] {3}: ", conditions, sorting, area.ToString(), identifier) + ex.Message, ex);
           }
           catch (Exception ex)
           {
				throw new PersistenceException(null, "PersistentSupport.seleccionaUm",
					                           string.Format("Error selecting one record from area - [condicoes] {0}; [ordenacao] {1}; [area] {2}; [identificador] {3}: ", conditions, sorting, area.ToString(), identifier) + ex.Message, ex);
           }
        }

        public void selectOne(CriteriaSet conditions, IList<ColumnSort> sorting, IArea area, string identifier, int pageSize = 1)
        {
            try
            {
                SelectQuery query = null;
                //check if the request is an LED
                bool isLed = false;
                if (identifier.Length >= 3)
                    isLed = identifier.Substring(0, 3).Equals("LED");

                //in case it is an LED, check if there is a query override (query done in manual routine)
                if (isLed && controlQueriesOverride.ContainsKey(identifier))//if it's an LED
                    query = controlQueriesOverride[identifier](area.User, area.User.CurrentModule, conditions, sorting, this);
                else
                {
                    query = querySeleccionaUm(conditions, sorting, area, pageSize);
                    if (isLed)
                    {
                        ControlQueryDefinition queryGenio = controlQueries[identifier];
                        //NH(02.08.2010) - Does not place the condition zzstate = 0
                        /*Não funcionava to os casos de inserção de registos através de um expõe table de um form em que a ficha ainda era Pseudo-nova.
                          Não carimbava a relação.
                        */
                        if (queryGenio.WhereConditions != null
                            && (queryGenio.WhereConditions.Criterias.Count > 1 || queryGenio.WhereConditions.SubSets.Count > 0))
                        {
                            CriteriaSet where = CriteriaSet.And();
                            for (int i = 1; i < queryGenio.WhereConditions.Criterias.Count; i++)
                            {
                                where.Criterias.Add(queryGenio.WhereConditions.Criterias[i]);
                            }
                            foreach (CriteriaSet subSet in queryGenio.WhereConditions.SubSets)
                            {
                                where.SubSets.Add(subSet);
                            }
                            query.WhereCondition.SubSet(where);
                        }
                    }

                    SelectQuery queryCount = QueryUtils.buildQueryCount(query);
                    DataMatrix nrLinhas = Execute(queryCount);
                    int nr = 0;
                    if (nrLinhas != null && nrLinhas.NumRows > 0)
                    {
                        nr = nrLinhas.GetInteger(0, 0);
                    }

                    if (pageSize > 0)
                    {
                        if (nr > 1 && !isLed)
                        {
                            throw new PersistenceException(null, "PersistentSupport.seleccionaUm", "There is more than one record in table " + area.ToString() + " satisfying the conditions " + conditions.ToString() + ".");
                        }
                        else if (nr == 0)//if there are no records
                        {
                            if (!isLed)//if it's not an LED should give error
                                throw new PersistenceException(null, "PersistentSupport.seleccionaUm", "There are no records in table " + area.ToString() + " satisfying the conditions " + conditions.ToString() + ".");
                            else
                            {   //if it is an LED should return the area with empty values
                                Hashtable hresVaz = new Hashtable();
                                for (int i = 0; i < query.SelectFields.Count; i++)
                                {
                                    hresVaz.Add(query.SelectFields[i].Alias, null);
                                }
                                fillAreaSelectOne(hresVaz, area);
                                return;
                            }
                        }
                    }
                }

                //if the override Qresult (manual routine) comes to empty
                ArrayList Qresult = executeReaderOneRow(query);
                Hashtable hres = new Hashtable();
                int countresult = Qresult.Count;
                if (countresult == 0)
                    countresult = query.SelectFields.Count;

                for (int i = 0; i < countresult; i++)
                {
                    if (Qresult.Count == 0)
                        hres.Add(query.SelectFields[i].Alias, null);
                    else
                        hres.Add(query.SelectFields[i].Alias, Qresult[i]);
                }

                fillAreaSelectOne(hres, area);
                return;
            }
            catch (PersistenceException ex)
            {
				if (ex.ExceptionSite == "PersistentSupport.seleccionaUm")
					throw;
                throw new PersistenceException(ex.UserMessage, "PersistentSupport.seleccionaUm",
					                           string.Format("Error selecting one record from area - [condicoes] {0}; [ordenacao] {1}; [area] {2}; [identificador] {3}: ", conditions.ToString(), sorting.ToString(), area.ToString(), identifier) + ex.Message, ex);
            }
            catch (Exception ex)
            {
				throw new PersistenceException(null, "PersistentSupport.seleccionaUm",
					                           string.Format("Error selecting one record from area - [condicoes] {0}; [ordenacao] {1}; [area] {2}; [identificador] {3}: ", conditions.ToString(), sorting.ToString(), area.ToString(), identifier) + ex.Message, ex);
            }
        }

        /// <summary>
        /// Place the data taken from the BD in the corresponding Area object
        /// </summary>
        /// <param name="resultado">Query qresult</param>
        /// <param name="area">Area to be filled</param>
        /// <returns>Filled area</returns>
        public void fillAreaSelectOne(Hashtable Qresult, IArea area)
        {
            if (Qresult.Count != 0)
            {
                int i = 0;
                //fill in the value of the primaryKey
                //if the primary key is in the query is the 1st value
                //to be able to be used to fill in other fields namely images
                if (area.Fields.ContainsKey(area.Alias + "." + area.PrimaryKeyName))
                {
                    area.insertNameValueField(area.Alias + "." + area.PrimaryKeyName
                        , Qresult[area.Alias + "." + area.PrimaryKeyName]);
                    i++;
                }

                //AV 20090309 foreign keys to a table docums
                if (area.Information.DocumsForeignKeys != null)
				{
                    for (int j = 0; j < area.Information.DocumsForeignKeys.Count; j++)
                    {
                        if (area.Fields.ContainsKey(area.Alias + "." + area.Information.DocumsForeignKeys[j]))
                        {
                            area.insertNameValueField(area.Alias + "." + area.Information.DocumsForeignKeys[j],
                                Qresult[area.Alias + "." + area.Information.DocumsForeignKeys[j]]);
                            i++;
                        }
                    }
				}

                IEnumerator enumerador = (IEnumerator)area.Fields.Keys.GetEnumerator();
                while (enumerador.MoveNext())
                {
                    RequestedField campoPedido = (RequestedField)area.Fields[enumerador.Current];
                    if (!enumerador.Current.Equals(area.Alias + "." + area.PrimaryKeyName)
                        && (area.Information.DocumsForeignKeys == null || !area.Information.DocumsForeignKeys.Contains(campoPedido.Name)))
                    {
                        object Qvalue = Qresult[campoPedido.FullName];
                        area.insertNameValueField(campoPedido.FullName, Qvalue);
                        i++;
                    }
                }

                return;
            }
            else
				throw new PersistenceException(null, "PersistentSupport.preencheAreaSeleccionaUm", "Argument 'resultado' is empty.");
        }

        /// <summary>
        /// Method that creates the query to respond to a GET1 request
        /// </summary>
        /// <param name="condicoes">conditions of the application</param>
        /// <param name="ordenacao">order of the application</param>
        /// <param name="area">order area</param>
        /// <returns>Returns QuerySelect with field values</returns>
        [Obsolete("Use SelectQuery querySeleccionaUm(CriteriaSet condicoes, IList<ColumnSort> ordenacao, IArea area) instead")]
        private QuerySelect querySeleccionaUm(string conditions, string sorting, IArea area)
        {
            //List of strings that correspond to tables with relationships that appear in the request
            List<string> tabelasAcima = new List<string>();

            foreach(RequestedField campoPedido in area.Fields.Values)
            {
                //RequestedField campoPedido = (RequestedField)enumCampos.Current;
                if (!campoPedido.BelongsArea && !campoPedido.WithoutArea)
                {
                    if (!tabelasAcima.Contains(campoPedido.Area))
                        tabelasAcima.Add(campoPedido.Area);
                }
            }
            //list of query relationships
            List<Relation> relations = QueryUtils.tablesRelationships(tabelasAcima, area);

            QuerySelect query = construcaoQuerySeleccionaUm(area, conditions, sorting, relations);
            return query;
        }

        private SelectQuery querySeleccionaUm(CriteriaSet conditions, IList<ColumnSort> sorting, IArea area, int pageSize = 1)
        {
            //List of strings that correspond to tables with relationships that appear in the request
            List<string> tabelasAcima = new List<string>();

            foreach (RequestedField campoPedido in area.Fields.Values)
            {
                //RequestedField campoPedido = (RequestedField)enumCampos.Current;
                if (!campoPedido.BelongsArea && !campoPedido.WithoutArea)
                {
                    if (!tabelasAcima.Contains(campoPedido.Area))
                        tabelasAcima.Add(campoPedido.Area);
                }
            }
            //list of query relationships
            List<Relation> relations = QueryUtils.tablesRelationships(tabelasAcima, area);

            SelectQuery query = construcaoQuerySeleccionaUm(area, conditions, sorting, relations);
            return query;
        }

        /// <summary>
        /// Helper method to create querySelect of the function type GET_UM
        /// </summary>
        /// <param name="area">table that where the record will be read</param>
        /// <param name="campos">fields that will be read</param>
        /// <param name="condicoes">query conditions</param>
        /// <param name="ordenacao">query ordering</param>
        /// <param name="relacoes">relationships to other table that are part of the query</param>
        /// <returns>A complete query to be executed</returns>
        [Obsolete("Use SelectQuery construcaoQuerySeleccionaUm(IArea area, CriteriaSet condicoes, IList<ColumnSort> ordenacao, List<Relation> relacoes) instead")]
        private QuerySelect construcaoQuerySeleccionaUm(IArea area, string conditions, string sorting, List<Relation> relations)
        {
            QuerySelect query = new QuerySelect(DatabaseType);

            string camposString = "";
            string fieldName;
            //if there is primary key in the query it must be the first field to be requested
            if (area.Fields.ContainsKey(area.Alias + "." + area.PrimaryKeyName))
                camposString = area.Alias + "." + area.PrimaryKeyName + ",";

            //AV 20090306 foreign keys to a table docums
            if (area.Information.DocumsForeignKeys != null)
                for (int i = 0; i < area.Information.DocumsForeignKeys.Count; i++)
                {
                    fieldName = area.Information.DocumsForeignKeys[i];
                    if (area.Fields.ContainsKey(area.Alias + "." + fieldName))
                    {
                        camposString += area.Alias + "." + fieldName + ",";
                    }
                }

            //fields of the area
            IEnumerator enumKeys = area.Fields.Keys.GetEnumerator();
            while (enumKeys.MoveNext())
            {
                if (!enumKeys.Current.Equals(area.Alias + "." + area.PrimaryKeyName))
                {
                    RequestedField campoPedido = (RequestedField)area.Fields[enumKeys.Current];
                    if (!campoPedido.WithoutArea && (area.Information.DocumsForeignKeys == null || !area.Information.DocumsForeignKeys.Contains(campoPedido.Name)))//if the field has area
                        camposString += campoPedido.FullName + ",";
                }
            }
            camposString = camposString.Remove(camposString.Length - 1, 1);

            query.addSelect(camposString);
            query.setFromTabDirect(relations, area);

            if (!conditions.Equals(""))
                query.addWhere(conditions, "AND");
            if (!sorting.Equals(""))
                query.addOrder(sorting);
            query.RecordCount = 1;
            return query;
        }

        private SelectQuery construcaoQuerySeleccionaUm(IArea area, CriteriaSet conditions, IList<ColumnSort> sorting, List<Relation> relations, int pageSize = 1)
        {
            SelectQuery query = new SelectQuery();

            //if there is primary key in the query it must be the first field to be requested
            if (area.Fields.ContainsKey(area.Alias + "." + area.PrimaryKeyName))
            {
                query.Select(area.Alias, area.PrimaryKeyName);
            }

            //AV 20090306 foreign keys to a table docums
            if (area.Information.DocumsForeignKeys != null)
            {
                for (int i = 0; i < area.Information.DocumsForeignKeys.Count; i++)
                {
                    string fieldName = area.Information.DocumsForeignKeys[i];
                    if (area.Fields.ContainsKey(area.Alias + "." + fieldName))
                    {
                        query.Select(area.Alias, fieldName);
                    }
                }
            }

            //fields of the area
            IEnumerator enumKeys = area.Fields.Keys.GetEnumerator();
            while (enumKeys.MoveNext())
            {
                if (!enumKeys.Current.Equals(area.Alias + "." + area.PrimaryKeyName))
                {
                    RequestedField campoPedido = (RequestedField)area.Fields[enumKeys.Current];
                    if (!campoPedido.WithoutArea && (area.Information.DocumsForeignKeys == null || !area.Information.DocumsForeignKeys.Contains(campoPedido.Name)))//if the field has area
                    {
                        query.Select(campoPedido.Area, campoPedido.Name);
                    }
                }
            }
            QueryUtils.setFromTabDirect(query, relations, area);

            if (conditions != null)
            {
                query.Where(conditions);
            }
            if (sorting != null && sorting.Count > 0)
            {
                foreach (ColumnSort sort in sorting)
                {
                    query.OrderBy(sort.Expression, sort.Order);
                }
            }
            query.PageSize(pageSize);
            return query;
        }

		/// <summary>
        /// Reorders a field within a subset from startPos to N maintaining the relative order of the records
        /// </summary>
        /// <param name="arearef">The table to reorder</param>
        /// <param name="orderField">The field to reorder</param>
        /// <param name="partition">The partition corresponding to the rows to be reordered</param>
        public void ReorderSequence(AreaRef arearef, FieldRef orderField, CriteriaSet partition, List<Relation> relations = null, int startPos = 1)
        {
            // UPDATE [GENNOV0].[dbo].[gencmpbd]
            // SET [num] = [renum_campo].[new_num]
            // FROM [GENNOV0].[dbo].[gencmpbd] [campo]
            // JOIN (SELECT  (ROW_NUMBER() OVER (ORDER BY [campo].[num]  ASC)) + startPos - 1 AS [new_num],  ([campo].[codcmpbd]) AS [pk]
            //          FROM [GENNOV0].[dbo].[gencmpbd] AS [campo]
            //          WHERE ([campo].[codtabel] = @param1)) AS [renum_campo]
            // ON ([renum_campo].[pk] = [campo].[codcmpbd])

            string pkName = Area.GetInfoArea(arearef.Alias).PrimaryKeyName;

			//RowNumber starts at 1 so, add startPos - 1 to RowNumber to start numbering at startPos
            SelectQuery sq = new SelectQuery()
                .Select(SqlFunctions.Add(SqlFunctions.RowNumber(orderField, SortOrder.Ascending, orderField), startPos - 1), "new_num")
                .Select(arearef.Alias, pkName, "pk")
                .From(arearef)
                .Where(partition);

            if(relations != null)
            {
                foreach (Relation r in relations)
                {
                    sq.Join(r.TargetTable, r.AliasTargetTab, TableJoinType.Left)
                        .On(CriteriaSet.And()
                            .Equal(r.AliasSourceTab, r.SourceRelField, r.AliasTargetTab, r.TargetRelField));
                }
            }

            UpdateQuery up = new UpdateQuery().Update(arearef)
                .Set(orderField.Field, new ColumnReference("renum_" + arearef.Alias, "new_num"))
                .Join(sq, "renum_" + arearef.Alias, TableJoinType.Inner).On(CriteriaSet.And().Equal("renum_" + arearef.Alias, "pk", arearef.Alias, pkName));

            Execute(up);
        }

		/// <summary>
        /// Get the highest value of a field
        /// </summary>
        /// <param name="area">The table</param>
        /// <param name="field">The field</param>
        /// <param name="partition">The partition corresponding to the rows</param>
        /// <param name="relations">Relations</param>
        public int GetMaxFieldValue(AreaRef area, FieldRef field, CriteriaSet partition, List<Relation> relations = null)
        {
            SelectQuery maxOrderQuery = new SelectQuery().Select(SqlFunctions.Max(field), "maxOrder").From(area).Where(partition);

            if (relations != null)
            {
                foreach (Relation r in relations)
                {
                    maxOrderQuery.Join(r.TargetTable, r.AliasTargetTab, TableJoinType.Left)
                        .On(CriteriaSet.And()
                            .Equal(r.AliasSourceTab, r.SourceRelField, r.AliasTargetTab, r.TargetRelField));
                }
            }

            DataMatrix maxOrderRows = Execute(maxOrderQuery);

            //No records or fields
            if (maxOrderRows.NumRows == 0 || maxOrderRows.NumCols == 0)
                throw new Exception("This table does not have any records that match the conditions given.");

            return maxOrderRows.GetInteger(0, 0);
        }

        /*********************************INSERIR DADOS*************************************/

        /// <summary>
        /// Function to introduce data in an area
        /// </summary>
        /// <param name="area">Area of the object you want to introduce</param>
        /// <param name="condicoes">insertion conditions</param>
        /// <param name="utilizador">user who is introducing the registration</param>
        /// <param name="isTabelaBase">to indicate whether it is a table base or not</param>
        /// <returns>Returns the inserted area</returns>
        public void insertPseud(IArea area)
        {
            InsertQuery query = new InsertQuery();
            QueryUtils.buildQueryInsert(query, area);

            // RR 24-02-2011 - the parameters of the query are passed to the function that will execute it
            int linha = Execute(query);

            //if it goes well, line!=0
            if (linha <= 0)
				throw new PersistenceException("Erro ao inserir dados na tabela.", "PersistentSupport.inserir", "The query to area " + area.ToString() + " returned value less than 1.");
        }

        /// <summary>
        /// Function to construct a query insert
        /// </summary>
        /// <param name="area">Registration area to introduce</param>
        /// <param name="utilizador">user login making insertion</param>
        /// <param name="codInt">internal code of the register to introduce</param>
        /// <returns>returns the insertion query</returns>
        public void fillAreaInsert(IArea area, string user, string codInt, string conditions, int zzStateValue)
        {
            Regex rx = new Regex("AND");
            string[] condicoesIns = rx.Split(conditions);

            for (int i = 0; i < condicoesIns.Length; i++)
            {
                if (!condicoesIns[i].Equals(""))
                {
                    string[] campoValor = condicoesIns[i].Trim().Split('=');
                    if (campoValor.Length != 2)
						throw new PersistenceException(null, "PersistentSupport.preencheAreaInserir", "The given conditions have a wrong format: " + conditions);
                    else
                    {
                        campoValor[0] = campoValor[0].Trim();
                        campoValor[1] = campoValor[1].Trim();
                        if (!campoValor[0].Equals(area.Alias + "." + area.PrimaryKeyName))
                        {
                            //campoValor[1] = campoValor[1].Substring(1, campoValor[1].Length - 2);
                            area.insertNameValueField(campoValor[0], campoValor[1]);
                        }
                    }
                }
            }
            //fill in the primary key
            area.insertNameValueField(area.Alias + "." + area.PrimaryKeyName, codInt);

            //fill the zzstate
            area.insertNameValueField(area.Alias + ".zzstate", zzStateValue);
        }

        /// <summary>
        /// Function that returns the internal code to insert a new record
        /// </summary>
        /// <param name="isTabelaBase">true if it is table hardcoded</param>
        /// <param name="area">Name of the area to which the record that will be inserted belongs.
        /// Does not assume an open connection and closes the connection</param>
        /// <returns>returns the new internal code</returns>
        //20051207 to as tables shadow
        public string codIntInsertion(bool isBaseTable, IArea area, bool shadow)
        {
            try
            {
                Field chaveinfo = area.DBFields[area.PrimaryKeyName];
                if(shadow)
                    return generatePrimaryKey(area.ShadowTabName, chaveinfo.FieldSize, area.Information.KeyType);
                else
                    return generatePrimaryKey(area.TableName, chaveinfo.FieldSize, area.Information.KeyType);
            }
            catch (PersistenceException ex)
            {
                throw new PersistenceException(ex.UserMessage, "PersistentSupport.codIntInsercao", "Error getting internal code to insert in area " + area.ToString() + ": " + ex.Message, ex);
            }
            catch (Exception ex)
            {
				throw new PersistenceException(null, "PersistentSupport.codIntInsercao", "Error getting internal code to insert in area " + area.ToString() + ": " + ex.Message, ex);
            }
        }

        /// <summary>
        /// Function that returns the internal code to insert a new record
        /// </summary>
        /// <param name="area">Name of the area to which the record that will be inserted belongs.
        /// Does not assume an open connection and closes the connection</param>
        /// <returns>returns the new internal code</returns>
        public string codIntInsertion(IArea area, bool shadow)
        {
            return codIntInsertion(false, area, shadow);
        }

        /// <summary>
        /// Gets a new primary key to a particular object
        /// </summary>
        /// <param name="id_objecto">The object to which you want to generate a key, typically the name of the table</param>
        /// <param name="tamanho">The size of the key to generate to the case of internal code</param>
        /// <param name="formato">The key format to generate</param>
        /// <returns>A single primary key</returns>
        public abstract string generatePrimaryKey(string id_object, int size, CodeType format);

        /// <summary>
        /// Method that generates a negative random number
        /// </summary>
        /// <param name="tam">number size to generate</param>
        /// <returns>the random number</returns>
        public int generateRandomNeg(int siz)
        {
            Random num = new Random();
            if (siz < 2)
				throw new PersistenceException(null, "PersistentSupport.geraAleatorioNeg", "The argument [tam] is less than 2.");
            int valorMaximo = 0;
            //siz-1 because we have to save a space to the least
            for (int i = 0; i < siz - 1; i++)
                valorMaximo -= -9 + valorMaximo * (10 ^ i);
            return valorMaximo;
        }

        public object insertValueDocums(IArea area, string fieldName, string fileName, string extension, byte[] file)
        {
            string tabelaDocums = "docums";
            object primaryKeyValue = area.returnValueField(area.Alias + "." + area.PrimaryKeyName);
            if (area.DBFields[area.PrimaryKeyName].FieldFormat.Equals(FieldFormatting.GUID))
                primaryKeyValue = primaryKeyValue.ToString().Replace("-", "");
            Field chaveDocums = CSGenioAdocums.GetInformation().DBFields["coddocums"];
            object valorChavePrimariaDocums = generatePrimaryKey(tabelaDocums, chaveDocums.FieldSize, CSGenioAdocums.GetInformation().KeyType);

            //RS(2010.09.16) The table docums starts to gardar several verses and the author of the document
            InsertQuery query = new InsertQuery()
                .Into(tabelaDocums)
                .Value("coddocums", valorChavePrimariaDocums)
                .Value("documid", valorChavePrimariaDocums)
                .Value("document", file)
                .Value("tabela", area.TableName)
                .Value("campo", fieldName)
                .Value("chave", primaryKeyValue)
                .Value("datacria", DateTime.Now)
                .Value("opercria", area.User.Name)
                .Value("nome", fileName)
                .Value("versao", "1")
                .Value("tamanho", file.Length)
                .Value("extensao", extension)
                .Value("opermuda", area.User.Name)
                .Value("datamuda", DateTime.Now)
                .Value("zzstate", 0);

            Execute(query);

            return valorChavePrimariaDocums;
        }

        /// <summary>
        /// Method that duplicates docums table records and updates foreign keys in duplicate area
        /// </summary>
        /// <param name="area">Area where the file fields will be replaced in BD</param>
        /// <param name="valorChavePrimaria">PrimaryKeyValue</param>
        /// <returns>area changed</returns>
        public string duplicateFilesDB(IArea area, object primaryKeyValue)
        {
            return duplicateFilesDB(area, primaryKeyValue, false);
        }

        /// <summary>
        /// Method that duplicates docums table records and updates foreign keys in duplicate area
        /// </summary>
        /// <param name="area">Area where the file fields will be replaced in BD</param>
        /// <param name="valorChavePrimaria">PrimaryKeyValue</param>
        /// <param name="paraCheckout">If the duplication is to checkout of the file or if it is a normal plug duplication</param>
        /// <param name="documField">Docum field used when forCheckout == true</param>
        /// <returns>area changed</returns>
        public string duplicateFilesDB(IArea area, object primaryKeyValue, bool forCheckout, string documField = "")
        {
            try
            {
                string tabelaDocums = "docums";
                object valorChavePrimariaDocums  ="";

                object valorChavePrimariaAux = primaryKeyValue;
                if (area.DBFields[area.PrimaryKeyName].FieldFormat.Equals(FieldFormatting.GUID))
                    valorChavePrimariaAux = valorChavePrimariaAux.ToString().Replace("-", "");

                if (area.Information.DocumsForeignKeys != null)
                {
                    var formatacaoChaveDocums = CSGenioAdocums.GetInformation().KeyType;
                    foreach(var documForeignKey in area.Information.DocumsForeignKeys)
                    {
                        RequestedField campoPedido = (RequestedField)area.Fields[area.Alias + "." + documForeignKey ];
                        if (campoPedido != null)//if (!fieldRequest.Value.Equals(")) // JMA (04-01-2011) This gave problems when there was no field and of course, it was null and burst.
                        {
                            // if the key value is blank, nothing is done because there is no document
                            // if this gets out of here to out, you can do -> Field.isEmptyValue()
                            if (string.IsNullOrEmpty(campoPedido.Value.ToString()) || (forCheckout && !campoPedido.FullName.Equals(documField)) )
                                continue;

                            valorChavePrimariaDocums = generatePrimaryKey(tabelaDocums, area.DBFields[documForeignKey].FieldSize, formatacaoChaveDocums);

                            SelectQuery qs = new SelectQuery()
                                .Select(CSGenioAdocums.FldDocumid)
                                .Select(CSGenioAdocums.FldDocument)
                                .Select(CSGenioAdocums.FldTabela)
                                .Select(CSGenioAdocums.FldCampo)
                                .Select(CSGenioAdocums.FldNome)
                                .Select(CSGenioAdocums.FldTamanho)
                                .Select(CSGenioAdocums.FldExtensao)
                                .From(tabelaDocums)
                                .Where(CriteriaSet.And()
                                    .Equal(CSGenioAdocums.FldDocumid, campoPedido.Value))
                                .OrderBy(CSGenioAdocums.FldVersao, SortOrder.Descending);

                            var matrix = this.Execute(qs);
                            if (matrix.NumRows > 0)
                            {
                                string documid = matrix.GetString(0, CSGenioAdocums.FldDocumid);
                                string extension = matrix.GetString(0, CSGenioAdocums.FldExtensao);

                                //Insert the record with the duplicated values
                                InsertQuery insert = new InsertQuery().Into(tabelaDocums);
                                insert.Value(CSGenioAdocums.FldCoddocums, valorChavePrimariaDocums);
                                insert.Value(CSGenioAdocums.FldDocumid, forCheckout ? documid : valorChavePrimariaDocums);
                                //This client stores documents in the database
                                insert.Value(CSGenioAdocums.FldDocument, matrix.GetBinary(0, CSGenioAdocums.FldDocument));
                                insert.Value(CSGenioAdocums.FldTabela, matrix.GetString(0, CSGenioAdocums.FldTabela));
                                insert.Value(CSGenioAdocums.FldCampo, matrix.GetString(0, CSGenioAdocums.FldCampo));
                                insert.Value(CSGenioAdocums.FldChave, valorChavePrimariaAux);
                                insert.Value(CSGenioAdocums.FldDatacria, DateTime.Now);
                                insert.Value(CSGenioAdocums.FldNome, matrix.GetString(0, CSGenioAdocums.FldNome));
                                // if it is to checkout the status is "CHECKOUT"
                                // if it is to duplicate the version resets to 1
                                insert.Value(CSGenioAdocums.FldVersao, forCheckout ? "CHECKOUT" : "1");
                                insert.Value(CSGenioAdocums.FldZzstate, 0);
                                insert.Value(CSGenioAdocums.FldOpercria, area.User.Name);
                                insert.Value(CSGenioAdocums.FldTamanho, matrix.GetString(0, CSGenioAdocums.FldTamanho));
                                insert.Value(CSGenioAdocums.FldExtensao, extension);
                                Execute(insert);

                                // if it's to checkout keeps the documid
                                // if it is to duplicate the plug the documid is equal to the primary key
                                object documsfk = forCheckout ? documid : valorChavePrimariaDocums;

                                area.insertNameValueField(campoPedido.FullName, documsfk);
                            }
                        }
                    }
                }
                return valorChavePrimariaDocums.ToString();
            }
            catch(Exception ex)
            {
                throw new FrameworkException($"Error duplicating documents on table {area.TableName}", "duplicateDocumentDB", ex.Message);
            }
        }

        /*********************************ELIMINAR DADOS*************************************/

        /// <summary>
        /// Method to eliminate a record
        /// </summary>
        /// <param name="area">Area to which the register belongs</param>
        /// <returns>returns the area after the record is deleted</returns>
        public void eliminate(IArea area)
        {
            object valorCodigoObj = area.returnValueField(area.Alias + "." + area.PrimaryKeyName);
            deleteRecord(area, (string)valorCodigoObj);
        }

        /// <summary>
        /// Function that allows you to eliminate a plug
        /// </summary>
        /// <param name="area"> Area to which the plug belongs that will be erased</param>
        /// <param name="valorCodigo">value of the internal code of the plug that will be erased</param>
        /// <returns>true if the plug is erased and false otherwise</returns>
        public bool deleteRecord(IArea area, string codeValue)
        {
            bool Qresult = false;
            DeleteQuery queryDelete = new DeleteQuery()
                .Delete(area.QSystem, area.TableName)
                .Where(CriteriaSet.And()
                    .Equal(area.TableName, area.PrimaryKeyName, codeValue));

            int linha = Execute(queryDelete);

            if (linha != 0)
                Qresult = true;
            else
				throw new PersistenceException("O registo não foi encontrado.", "PersistentSupport.apagarFicha", "Error deleting record with code " + codeValue + " from area " + area.ToString() + ": the query returned 0.");
            //here does not close connection, because I'm erasing cascading chips
            return Qresult;
        }

        /// <summary>
        /// Function that allows you to delete a relationship, that is, it puts the value of the foreign key to ""
        /// </summary>
        /// <param name="area">Area to which the token belongs whose relationship will be deleted</param>
        /// <param name="nomeCodigoEstrangeiro">Foreign key code name</param>
        /// <param name="valorCodigo">valro of the foreign key code</param>
        /// <returns>true if you can erase the relationship, false otherwise</returns>
        public bool deleteRelationship(IArea area, string foreignCodeName, string codeValue)
        {
            bool Qresult = false;
            string query = "UPDATE " + area.TableName + " SET " + foreignCodeName + "=" + DBConversion.FromKey("") + " WHERE " + foreignCodeName + "=" + DBConversion.FromKey(codeValue.ToString());
            int linha = executeNonQuery(query);

            //JV(2013.12.04)
            // This was throwing exceptions if the qresult of executeNonQuery was 0, which is wrong because it just means that there are no chips to clear.

            //here does not close connection, because I'm erasing cascading chips
            return Qresult;
        }

        public void deleteRecordDocums(object keyNameDocum,object keyValueDocums)
        {
            if (keyValueDocums != null && keyValueDocums.ToString() != String.Empty)
            {
                //RS(2010.09.16) The table docums starts to gardar several verses and the author of the document
                string tableName = "docums";
                DeleteQuery query = new DeleteQuery()
                    .Delete(tableName)
                    .Where(CriteriaSet.And()
                        .Equal(tableName, Convert.ToString(keyNameDocum), keyValueDocums));
                Execute(query);
            }
        }

        /*********************************ALTERAR DADOS*****************************************/

        /// <summary>
        /// Function that allows you to change a database record
        /// </summary>
        /// <param name="area">Area to which the registry belongs</param>
        /// <param name="condicao">Condition that allows you to identify which record</param>
        /// <returns>The Pair (status,message) about the change</returns>
        public void change(IArea area)
        {
            // RR 24/02/2011
            // here goes on to build the update query instead of calling the function
            // buildChange, to be able to access the parameters

            UpdateQuery query = new UpdateQuery();
            QueryUtils.fillQueryUpdate(query, area);

            int linha = Execute(query);
            if (linha == 0)
            {
				throw new PersistenceException("Erro na alteração do registo.", "PersistentSupport.alterar", "Error updating record from area " + area.ToString() + ": the query returned 0.");
            }
        }

        /// <summary>
        /// Method to construct a query to change the data
        /// </summary>
        /// <param name="area">Area a change</param>
        /// <returns>string with the query</returns>
        public UpdateQuery buildGenericQueryChange(IArea area)
        {
            UpdateQuery query = new UpdateQuery();
            QueryUtils.fillQueryUpdate(query, area);

            return query;
        }

        /// <summary>
        /// Method to change the value of a field in all records that check the condition
        /// </summary>
		/// <param name="sistema">Register table prefix schema changed</param>
        /// <param name="tabela">Name of the changed register table</param>
        /// <param name="campo">Field that will be changed</param>
        /// <param name="valor">New field value</param>
        /// <param name="condicao">Condition that identifies the records that will be changed</param>
        public void changeFieldValue(string system, string table, string Qfield, object Qvalue, CriteriaSet condition)
        {
            if (Log.IsDebugEnabled) Log.Debug(string.Format("Altera o valor do campo na tabela. [tabela] {0} [campo] {1} [valor] {2}", table, Qfield, Qvalue));

            UpdateQuery query = new UpdateQuery()
                .Update(system, table)
                .Set(Qfield, Qvalue)
                .Where(condition);

            Execute(query);
        }

        public void changeValueDocums(object keyValueDocums, byte[] file, string fileName, string extension, string Qversion, string operChange)
        {
            changeValueDocums(keyValueDocums, file, fileName, extension, Qversion, operChange, "DEFAULT");
        }

        public void changeValueDocums(object keyValueDocums, byte[] file, string fileName, string extension, string Qversion, string operChange, string alias)
        {
            if (Log.IsDebugEnabled)
                Log.Debug(string.Format("Altera o documento. [ficheiro] {0}", fileName));

            // RS(2010.09.16) The table docums starts to save several versions and the author of the document
            string tabelaDocums = "docums";

            UpdateQuery query = new UpdateQuery()
                .Update(tabelaDocums)
                .Set("document", file)
                .Set("docpath", DBNull.Value)
                .Set("nome", fileName)
                .Set("tamanho", file.Length)
                .Set("versao", Qversion)
                .Set("opermuda", operChange)
                .Set("extensao", extension)
                .Set("datamuda", DateTime.Now)
                .Where(CriteriaSet.And()
                    .Equal(tabelaDocums, "coddocums", keyValueDocums));

            Execute(query);
        }

        /********************************GET***********************************/
        /// <summary>
        /// Function to obtain a set of records
        /// </summary>
        /// <param name="identificador">identifier of the query that the case generates</param>
        /// <param name="listagem">Qlisting of desired objects</param>
        /// <param name="condicoes">conditions</param>
        /// <param name="nrRegistos">number of desired objects</param>
        /// <returns>a Qlisting with the objects filled in</returns>
        [Obsolete("Use Listing seleccionar(string identificador, Listing listagem, CriteriaSet condicoes, int nrRegistos) instead")]
        public Listing select(string identifier, Listing Qlisting, string conditions, int nrRecords, Boolean noLock)
        {
            try
            {
                if (Log.IsDebugEnabled) Log.Debug(string.Format("PersistentSupport.seleccionar. [id] {0}", identifier));

                QuerySelect querySelect = new QuerySelect(DatabaseType);
				string[] queryGenio = (string[])controlos[identifier];
                string query = "";
                bool distinct = false;
                if (queryGenio.Length > 3)
                    distinct = (queryGenio[3] == "true");
                if (controlosOverride.ContainsKey(identifier))
                {
                    querySelect = controlosOverride[identifier](Qlisting.User, Qlisting.Module, conditions, this);
                    //AV 2009/11/20 The query goes on to just get the ordering that comes from the genio
                    //when no one has been set on the override
                    if(querySelect.Order==null || querySelect.Order.Equals(""))
						querySelect.Order = new StringBuilder(Qlisting.Sort);
                    querySelect.RecordCount = nrRecords;
                }
                else
                {
                    querySelect.increaseQuery(queryGenio[0], queryGenio[1], queryGenio[2], DatabaseType, nrRecords, conditions, Qlisting.Sort, distinct);
                    querySelect.QPrimaryKey = querySelect.SelectFields[0];
                }
                if (querySelect.QPrimaryKey != null && querySelect.QPrimaryKey != "")
                    querySelect.buildQuery(querySelect.QPrimaryKey);
                else
                querySelect.buildQuery();
                query = querySelect.Query;
                DataMatrix ds = executeQuery(query);
                Qlisting.DataMatrix = ds.DbDataSet;
                Qlisting.LastFilled = ds.NumRows;
                if(Qlisting.obterTotal)
                    Qlisting.TotalRecords = DBConversion.ToInteger(executeScalar(querySelect.buildQueryCount()));
                return Qlisting;
            }
            catch (PersistenceException ex)
            {
                throw new PersistenceException(ex.UserMessage, "PersistentSupport.seleccionar",
					"Error selecting " + nrRecords.ToString() + " records with listing " + Qlisting.ToString() + " where " + conditions + ": " + ex.Message, ex);
            }
            catch (Exception ex)
            {
				throw new PersistenceException(null, "PersistentSupport.seleccionar",
					"Error selecting " + nrRecords.ToString() + " records with listing " + Qlisting.ToString() + " where " + conditions + ": " + ex.Message, ex);
            }
        }

        public Listing anotherSelect(string identifier, Listing Qlisting, CriteriaSet conditions, int nrRecords, int offset, Area area)
        {
            try
            {
                if (Log.IsDebugEnabled) Log.Debug(string.Format("PersistentSupport.seleccionar. [id] {0}", identifier));

                SelectQuery querySelect = new SelectQuery();
                ControlQueryDefinition queryGenio = controlQueries[identifier];

                if (controlQueriesOverride.ContainsKey(identifier))
                {
                    querySelect = controlQueriesOverride[identifier](Qlisting.User, Qlisting.Module, conditions, Qlisting.QuerySort, this);
                    //AV 2009/11/20 The query goes on to just get the ordering that comes from the genio
                    //when no one has been set on the override
                    if (querySelect.OrderByFields.Count == 0 && Qlisting.QuerySort != null)
                    {
                        foreach (ColumnSort sort in Qlisting.QuerySort)
                        {
                            querySelect.OrderByFields.Add(sort);
                        }
                    }
                    if (nrRecords > 0)
                    {
                        querySelect.PageSize(nrRecords);
                    }
                }
                else
                {
                    QueryUtils.increaseQuery(querySelect, queryGenio.SelectFields, queryGenio.FromTable, queryGenio.Joins, queryGenio.WhereConditions, nrRecords, conditions, Qlisting.QuerySort, queryGenio.Distinct);
                }
                querySelect.Offset(offset);
                QueryUtils.SetInnerJoins(Qlisting.RequestedFields, conditions, area, querySelect);

                DataMatrix ds = Execute(querySelect);
                Qlisting.DataMatrix = ds.DbDataSet;
                Qlisting.LastFilled = ds.NumRows;
                if (Qlisting.obterTotal)
                {
                    Qlisting.TotalRecords = DBConversion.ToInteger(ExecuteScalar(QueryUtils.buildQueryCount(querySelect)));
                }
                return Qlisting;
            }
            catch (PersistenceException ex)
            {
                throw new PersistenceException(ex.UserMessage, "PersistentSupport.anotherSelect",
					"Error selecting " + nrRecords.ToString() + " records with listing " + Qlisting.ToString() + " where " + conditions.ToString() + " inner join with area " + area.ToString() + ": " + ex.Message, ex);
            }
            catch (Exception ex)
            {
				throw new PersistenceException(null, "PersistentSupport.anotherSelect",
					"Error selecting " + nrRecords.ToString() + " records with listing " + Qlisting.ToString() + " where " + conditions.ToString() + " inner join with area " + area.ToString() + ": " + ex.Message, ex);
            }
        }

        /// <summary>
        /// Fills the foreign area with the information of the requested fields, by positioning the query in a baseArea.
        /// This allows to get information about upper tables.
        /// </summary>
        /// <param name="foreignArea">The area that we want to fill</param>
        /// <param name="baseArea">The base area of the query</param>
        /// <param name="conditions">Criteriaset</param>
        /// <param name="requestedFields">The requested fields</param>
        public void fillInfoForForeignKey(Area foreignArea, Area baseArea, CriteriaSet conditions, List<string> requestedFields)
        {
            try
            {
                SelectQuery query = new SelectQuery();
                foreach(string field in requestedFields) {
                    string[] tabelacampo = field.Split('.');
                    string table = tabelacampo[0];
                    string Qfield = tabelacampo[1];

                    var cp = Area.GetInfoArea(table).DBFields[Qfield];

                    query.Select(cp.Alias, Qfield);
                }

                query.From(baseArea.QSystem, baseArea.TableName, baseArea.TableName)
                    .Where(conditions);

                QueryUtils.SetInnerJoins(requestedFields.ToArray(), conditions, baseArea, query);

                DataMatrix mx = Execute(query);

                for (int i = 0; i < mx.NumRows; i++)
                {
                    // we assume all requested fields belong to the area!
                    for (int j = 0; j < mx.NumCols; j++)
                    {
                        // Don't take this condition off. Explanation: The user can remove columns from an area in the settings
                        // and the database still to be had on the corresponding table. If the user querys with "*", the
                        // below call would try to search in the corresponding Area structure all fields returned by SQL query.
                        // Since this field no longer exists in the structure, an exception would obviously arise from this.
                        if (foreignArea.DBFields.ContainsKey(query.SelectFields[j].Alias.Split('.')[1])) //I'm assuming that the keys are the long names and that's never going to change
                        {
                            foreignArea.insertNameValueField(query.SelectFields[j].Alias, mx.GetDirect(i, j));
                        }
                    }
                }
            }
            catch (PersistenceException ex)
            {
                closeConnection();
                throw new PersistenceException(ex.UserMessage, "PersistentSupport.fillInfoForForeignKey",
					"Error filling info on fields " + requestedFields + " from table " + baseArea.TableName + " where " + conditions.ToString() + " into table " + foreignArea.TableName + ": " + ex.Message, ex);
            }
            catch (Exception ex)
            {
                closeConnection();
				throw new PersistenceException(null, "PersistentSupport.fillInfoForForeignKey",
					"Error filling info on fields " + requestedFields + " from table " + baseArea.TableName + " where " + conditions.ToString() + " into table " + foreignArea.TableName + ": " + ex.Message, ex);
            }
        }

        public void PrepareQuerySelect(string identifier, string[] fieldsRequested, IList<ColumnSort> sorting, bool distinct, int numRegs, int offset, Area area)
        {
            SelectQuery querySelect = new SelectQuery();

            if (!controlQueries.ContainsKey(identifier))
            {
                // Set requested fields
                if (fieldsRequested != null && fieldsRequested.Length > 0)
                {
                    foreach (string field in fieldsRequested)
                    {
                        string[] tabelacampo = field.Split('.');
                        string table = tabelacampo[0];
                        string Qfield = tabelacampo[1];

                        var cp = Area.GetInfoArea(table).DBFields[Qfield];

                        querySelect.Select(cp.Alias, Qfield);
                    }
                }
                else
                {
                    foreach (Field field in area.DBFields.Values)
                    {
                        querySelect.Select(area.Alias, field.Name);
                    }
                }

                // Set order by
                querySelect.OrderByFields.Clear();
                if (sorting != null)
                {
                    foreach (ColumnSort sort in sorting)
                    {
                        querySelect.OrderByFields.Add(sort);
                    }
                }

                // Distinct set
                querySelect.Distinct(distinct);

                // Set pagination
                if (numRegs > 0)
                {
                    querySelect.PageSize(numRegs + 1);
                    querySelect.Offset(offset);
                }

                ControlQueryDefinition cqd = new ControlQueryDefinition(querySelect.SelectFields, querySelect.FromTable, querySelect.Joins, querySelect.WhereCondition);

                adicionaControlo(identifier, cqd);
            }
        }

        public Listing select(string identifier, Listing Qlisting, CriteriaSet conditions, int nrRecords, Boolean noLock)
        {
            try
            {
                if (Log.IsDebugEnabled) Log.Debug(string.Format("PersistentSupport.seleccionar. [id] {0}", identifier));

                SelectQuery querySelect = new SelectQuery();
				querySelect.noLock= noLock;
                ControlQueryDefinition queryGenio = controlQueries[identifier];
                if (controlQueriesOverride.ContainsKey(identifier))
                {
                    querySelect = controlQueriesOverride[identifier](Qlisting.User, Qlisting.Module, conditions, Qlisting.QuerySort, this);
                    //AV 2009/11/20 The query goes on to just get the ordering that comes from the genio
                    //when no one has been set on the override
                    if (querySelect.OrderByFields.Count == 0 && Qlisting.QuerySort != null)
                    {
                        foreach (ColumnSort sort in Qlisting.QuerySort)
                        {
                            querySelect.OrderByFields.Add(sort);
                        }
                    }
                    if (nrRecords > 0)
                    {
                        querySelect.PageSize(nrRecords);
                    }
                }
                else
                {
                    QueryUtils.increaseQuery(querySelect, queryGenio.SelectFields, queryGenio.FromTable, queryGenio.Joins, queryGenio.WhereConditions, nrRecords, conditions, Qlisting.QuerySort, queryGenio.Distinct);
                }

                DataMatrix ds = Execute(querySelect);
                Qlisting.DataMatrix = ds.DbDataSet;
                Qlisting.LastFilled = ds.NumRows;
                if(Qlisting.obterTotal)
                {
                    Qlisting.TotalRecords = DBConversion.ToInteger(ExecuteScalar(QueryUtils.buildQueryCount(querySelect)));
                }
                return Qlisting;
            }
            catch (PersistenceException ex)
            {
                throw new PersistenceException(ex.UserMessage, "PersistentSupport.seleccionar",
					"Error selecting " + nrRecords.ToString() + " records with listing " + Qlisting.ToString() + " where " + conditions + ": " + ex.Message, ex);
            }
            catch (Exception ex)
            {
				throw new PersistenceException(null, "PersistentSupport.seleccionar",
					"Error selecting " + nrRecords.ToString() + " records with listing " + Qlisting.ToString() + " where " + conditions + ": " + ex.Message, ex);
            }
        }

        /********************************GETNIVEL***********************************/
        /// <summary>
        /// Function to obtain a set of records
        /// </summary>
        /// <param name="areaNivel">area used in the level of the tree being requested</param>
        /// <param name="camposPedido">tree columns</param>
        /// <param name="listagem">Qlisting of desired objects</param>
        /// <param name="condicoes">conditions</param>
        /// <param name="condChavePai">condition to identify which branch has expanded</param>
        /// <returns>a Qlisting with the objects filled in</returns>
        public Listing selectLevel(IArea areaLevel, IList<SelectField> fieldsRequested, Listing Qlisting, CriteriaSet conditions, string parentKeyCond)
        {
            //Last updated by [CJP] at [30.09.2014] - Support for new Qweb_ctldados.js
            try
            {
                Log.Debug("Selecciona registos para um nível da árvore.");

                SelectQuery querySel = new SelectQuery();
                StringBuilder fields = new StringBuilder();

                //Assume that the parent-owned condition is the first condition
                string[] keyValue = parentKeyCond.Replace("[","").Split('=');

                string[] nomeChave;
                string chavePai;
                bool paiIsNivel = false;
                if (keyValue[0] != "")
                {
                    nomeChave = keyValue[0].Split('.');
                    if (nomeChave[0] != areaLevel.Alias)
                    {
                        chavePai = areaLevel.ParentTables[nomeChave[0]].SourceRelField;
                    }
                    else
                    {
                        paiIsNivel = true;
                        chavePai = nomeChave[1];
                    }
                }
                else
                {
                    chavePai = "";
                }

                querySel.From(areaLevel.QSystem, areaLevel.TableName, areaLevel.Alias)
                    .Where(CriteriaSet.And());
                if (areaLevel.Information.TreeTable == null) //it's not table in tree
                {
                    if (parentKeyCond != null && !keyValue[1].Equals("''")) //exists a previous level
                    {
                        querySel.WhereCondition.Equal(areaLevel.Alias, chavePai, keyValue[1].Trim('\''));
                    }

                }
                else //table in tree
                {
                    //WARNING - In Queries, only the field designation can be
                    //          Insert usValue, one must enter [table designation]. [field designation]
                    string[] level = areaLevel.Information.TreeTable.RecordLevelField.Split('.');
                    if (!keyValue[1].Equals("''")) //exists a previous level
                    {
                        if (paiIsNivel)
                        {
                            string sigla = areaLevel.Information.TreeTable.DesignationField;
                            string[] pai = areaLevel.Information.TreeTable.ParentTableField.Split('.');
                            areaLevel.insertNameValueField(keyValue[0], null);
                            areaLevel.insertNameValueField(sigla, null);
                            areaLevel.insertNameValueField(level[0] + "." + level[1], null);

                            //Devlove the father's record, from his primary key
                            getRecord(areaLevel, keyValue[1].Trim('\''));
                            querySel.WhereCondition.Equal(areaLevel.Alias, level[1], Convert.ToInt32(areaLevel.returnValueField(level[0] + "." + level[1])) + 1);
                            querySel.WhereCondition.Equal(areaLevel.Alias, pai[1], areaLevel.returnValueField(sigla));
                        }
                        else
                        {
                            querySel.WhereCondition.Equal(areaLevel.Alias, chavePai, keyValue[1].Trim('\''));
                            querySel.WhereCondition.Equal(areaLevel.Alias, level[1], 1);
                        }
                    }
                    else //this is the first level of the tree
                    {
                        querySel.WhereCondition.Equal(areaLevel.Alias, level[1], 1);
                    }
                }

                foreach (SelectField field in fieldsRequested)
                {
                    querySel.SelectFields.Add(field);
                }
                foreach (ColumnSort sort in Qlisting.QuerySort)
                {
                    querySel.OrderByFields.Add(sort);
                }

                //Removes the first condition from the query (the condition of the parent key)
                conditions.SubSets.RemoveAt(0);
                querySel.WhereCondition.SubSet(conditions);

                DataMatrix ds = Execute(querySel);
                Qlisting.DataMatrix = ds.DbDataSet;
                Qlisting.LastFilled = ds.NumRows;
                return Qlisting;
            }
            catch (PersistenceException ex)
            {
                throw new PersistenceException(ex.UserMessage, "PersistentSupport.seleccionarNivel",
											   "Error selecting records - " + string.Format("[areaNivel] {0}; [camposPedido] {1}; [listagem] {2}; [condicoes] {3}; [condChavePai] {4}: ",
																							areaLevel.ToString(), fieldsRequested.ToString(), Qlisting.ToString(), conditions.ToString(), parentKeyCond) + ex.Message, ex);
            }
            catch (Exception ex)
            {
                throw new PersistenceException(null, "PersistentSupport.seleccionarNivel",
											   "Error selecting records - " + string.Format("[areaNivel] {0}; [camposPedido] {1}; [listagem] {2}; [condicoes] {3}; [condChavePai] {4}: ",
																							areaLevel.ToString(), fieldsRequested.ToString(), Qlisting.ToString(), conditions.ToString(), parentKeyCond) + ex.Message, ex);
            }
        }

        /********************************GET+***********************************/
        /// <summary>
        /// Function to obtain a set of records
        /// </summary>
        /// <param name="identificador">identifier of the query that the case generates</param>
        /// <param name="listagem">Qlisting of desired objects</param>
        /// <param name="condicoes">conditions</param>
        /// <param name="nrRegistos">number of desired objects</param>
		/// <param name="ultimaLida">last row read</param>
		/// <param name="chavePrimaria">primary key of the last sheet read</param>
        /// <returns>a Qlisting with the objects filled in</returns>
        public Listing selectMore(string identifier, Listing Qlisting, CriteriaSet conditions, int nrRecords, int lastRead, string primaryKey)
        {
            try
            {
                if (Log.IsDebugEnabled) Log.Debug(string.Format("Selecciona mais registos. [id] {0}", identifier));
                ControlQueryDefinition queryGenio = controlQueries[identifier];
                SelectQuery querySelect = new SelectQuery();

                if (controlQueriesOverride.ContainsKey(identifier))
                {
                    querySelect = controlQueriesOverride[identifier](Qlisting.User, Qlisting.Module, conditions, Qlisting.QuerySort, this);
                    //AV 2009/11/20 The query goes on to just get the ordering that comes from the genio
                    //when no one has been set on the override
                    if (querySelect.OrderByFields.Count == 0)
                    {
                        foreach (ColumnSort sort in Qlisting.QuerySort)
                        {
                            querySelect.OrderByFields.Add(sort);
                        }
                    }
                    if (nrRecords > 0)
                    {
                        querySelect.PageSize(nrRecords);
                    }
                }
                else
                {
                    QueryUtils.increaseQuery(querySelect, queryGenio.SelectFields, queryGenio.FromTable, queryGenio.Joins, queryGenio.WhereConditions, nrRecords, conditions, Qlisting.QuerySort, queryGenio.Distinct);
                }
                querySelect.Offset(lastRead);

                DataMatrix ds = Execute(querySelect);
                Qlisting.DataMatrix = ds.DbDataSet;
                Qlisting.LastFilled = ds.NumRows;
                if(Qlisting.obterTotal)
                {
                    Qlisting.TotalRecords = DBConversion.ToInteger(ExecuteScalar(QueryUtils.buildQueryCount(querySelect)));
                }
                return Qlisting;
            }
            catch (PersistenceException ex)
            {
                throw new PersistenceException(ex.UserMessage, "PersistentSupport.seleccionarMais",
											   "Error selecting records - " + string.Format("[identificador] {0}; [listagem] {1}; [condicoes] {2}; [nrRegistos] {3}; [ultimaLida] {4}; [chavePrimaria] {5}: ",
																							identifier, Qlisting.ToString(), conditions.ToString(), nrRecords.ToString(), lastRead.ToString(), primaryKey) + ex.Message, ex);
            }
            catch (Exception ex)
            {
                throw new PersistenceException(null, "PersistentSupport.seleccionarMais",
											   "Error selecting records - " + string.Format("[identificador] {0}; [listagem] {1}; [condicoes] {2}; [nrRegistos] {3}; [ultimaLida] {4}; [chavePrimaria] {5}: ",
																							identifier, Qlisting.ToString(), conditions.ToString(), nrRecords.ToString(), lastRead.ToString(), primaryKey) + ex.Message, ex);
            }
        }

        /// <summary>
        /// Method that returns the number of records in a query
        /// </summary>
        /// <param name="identificador">query identifier</param>
        /// <param name="listagem">Qlisting with Qresult data</param>
        /// <param name="condicoes">conditions</param>
        /// <returns>nr of records</returns>
        public int count(string identifier, Listing Qlisting, CriteriaSet conditions)
        {
            try
            {
                if (Log.IsDebugEnabled) Log.Debug(string.Format("Conta o número de registos. [id] {0}", identifier));

                ControlQueryDefinition queryGenio = controlQueries[identifier];
                SelectQuery querySelect = new SelectQuery();

                if (controlQueriesOverride.ContainsKey(identifier))
                {
                    querySelect = controlQueriesOverride[identifier](Qlisting.User, Qlisting.Module, conditions, null, this);
                }
                else
                {
                    QueryUtils.increaseQuery(querySelect, queryGenio.SelectFields, queryGenio.FromTable, queryGenio.Joins, queryGenio.WhereConditions, 1, conditions, null, queryGenio.Distinct);
                }

                DataMatrix mx = Execute(QueryUtils.buildQueryCount(querySelect));
                return mx.GetInteger(0, 0);
            }
            catch (PersistenceException ex)
            {
                throw new PersistenceException(ex.UserMessage, "PersistentSupport.contar",
											   string.Format("Error counting records - [identificador] {0}; [listagem] {1}; [condicoes] {2}: ", identifier, Qlisting.ToString(), conditions.ToString()) + ex.Message, ex);
            }
            catch (Exception ex)
            {
                throw new PersistenceException(null, "PersistentSupport.contar",
											   string.Format("Error counting records - [identificador] {0}; [listagem] {1}; [condicoes] {2}: ", identifier, Qlisting.ToString(), conditions.ToString()) + ex.Message, ex);
            }
        }

        /*****************************************GETP***********************************************/
		/// <summary>
        /// Function to obtain a set of records
        /// </summary>
		/// <param name="utilizador">user who triggered the request</param>
		/// <param name="modulo">module that triggered the request</param>
		/// <param name="area">Qlisting area</param>
        /// <param name="ordenacao">columns to sort</param>
		/// <param name="valorChavePrimaria">value of the primary key</param>
        /// <param name="condicoes">conditions</param>
		/// <param name="identificador">identifier of the query that the case generates</param>
        /// <returns>a Qlisting with the objects filled in</returns>
        public virtual int getRecordPos(User user, string module, IArea area, IList<ColumnSort> sorting, string primaryKeyValue, CriteriaSet conditions, string identifier)
        {
            try
            {
                if (Log.IsDebugEnabled) Log.Debug(string.Format("getRecordPos. [id] {0}", identifier));

                ControlQueryDefinition queryGenio = controlQueries[identifier];
                SelectQuery querySelect = new SelectQuery();

                if (controlQueriesOverride.ContainsKey(identifier))
                {
                    querySelect = controlQueriesOverride[identifier](user, module, conditions, sorting, this);
                }
                else
                {
                    QueryUtils.increaseQuery(querySelect, queryGenio.SelectFields, queryGenio.FromTable, queryGenio.Joins, queryGenio.WhereConditions, 0, conditions, null, queryGenio.Distinct);
                }

                SelectQuery posQuery = new SelectQuery();

                var colSortPK = new ColumnSort(new ColumnReference(area.Alias, area.PrimaryKeyName), SortOrder.Ascending);
                if (!sorting.Contains(colSortPK))
                    sorting.Add(colSortPK);
                var orderby = new ColumnSort[sorting.Count];
                for(int i = 0; i < sorting.Count; i++) { orderby[i] = sorting[i]; }// Can be replaced with Linq

                querySelect.Select(SqlFunctions.RowNumber(orderby), "order");

                posQuery.Select("subq", "order");
                posQuery.From(querySelect, "subq");
                posQuery.Where(CriteriaSet.And().Equal("subq", area.Alias + "." +area.PrimaryKeyName, primaryKeyValue));

                DataMatrix mx = Execute(posQuery);
                return mx.NumRows == 0 ? 0 : mx.GetInteger(0, 0);
            }
            catch (PersistenceException ex)
            {
                throw new PersistenceException(ex.UserMessage, "PersistentSupport.getRecordPos",
											   string.Format("Error getting record position - [utilizador] {0}; [modulo] {1}; [area] {2}; [ordenacao] {3}; [valorChavePrimaria] {4}; [condicoes] {5}; [identificador] {6}: ",
											                 user.ToString(), module, area.ToString(), sorting.ToString(), primaryKeyValue, conditions.ToString(), identifier) + ex.Message, ex);
            }
            catch (Exception ex)
            {
                throw new PersistenceException(null, "PersistentSupport.getRecordPos",
											   string.Format("Error getting record position - [utilizador] {0}; [modulo] {1}; [area] {2}; [ordenacao] {3}; [valorChavePrimaria] {4}; [condicoes] {5}; [identificador] {6}: ",
											                 user.ToString(), module, area.ToString(), sorting.ToString(), primaryKeyValue, conditions.ToString(), identifier) + ex.Message, ex);
			}
        }

        /// <summary>
        /// Gets the query of the row number for getting position of the record in the listing so that it can determine the page where it is located
        /// </summary>
        /// <param name="areaBase">Area base of the list</param>
        /// <param name="pkValue">Primary key value of the record to find</param>
        /// <param name="orderby">Order by clause</param>
        /// <param name="where">Where clause</param>
        /// <param name="ephs">EPH condition</param>
        /// <returns>-1 or the row number if found</returns>
        public SelectQuery getQueryPagingPos(AreaInfo areaBase, string pkValue, List<ColumnSort> orderby, CriteriaSet where, CriteriaSet ephs)
        {
            if (areaBase == null) return null;

            if (orderby == null) orderby = new List<ColumnSort>();

            var pk = new ColumnReference(areaBase.Alias, areaBase.PrimaryKeyName);
            var colSortPK = new ColumnSort(pk, SortOrder.Ascending);

            var containsPK = orderby.Contains(colSortPK);
            var sortsCount = orderby.Count + (containsPK ? 0 : 1);

            var sorts = new ColumnSort[sortsCount];
            orderby.CopyTo(sorts);
            if (!containsPK) sorts[sortsCount - 1] = colSortPK;

            where.SubSet(ephs);

            var subQuery = new SelectQuery()
                            .Select(SqlFunctions.RowNumber(sorts), "rn")
                            .From(areaBase.TableName, areaBase.Alias)
                            .Where(where);
            subQuery.noLock = true;

            // Joins
            var requestedFields = new string[] { };
            if (orderby.Count > 0)
            {
                requestedFields = new string[orderby.Count];
                for (int i = 0; i < orderby.Count; i++)
                {
                    var reqField = (ColumnReference)orderby[i].Expression;
                    requestedFields[i] = reqField.TableAlias + "." + reqField.ColumnName;
                }
            }
            QueryUtils.SetInnerJoins(requestedFields, where, Area.createArea(areaBase.Alias, null, null), subQuery);

            return subQuery;
        }

        /// <summary>
        /// Gets the position of the record in the listing so that it can determine the page where it is located
        /// </summary>
        /// <param name="areaBase">Area base of the list</param>
        /// <param name="pkValue">Primary key value of the record to find</param>
        /// <param name="orderby">Order by clause</param>
        /// <param name="where">Where clause</param>
        /// <param name="ephs">EPH condition</param>
        /// <returns>-1 or the row number if found</returns>
        public int getPagingPos(AreaInfo areaBase, string pkValue, List<ColumnSort> orderby, CriteriaSet where, CriteriaSet ephs, IList<TableJoin> Joins = null, FieldRef firstVisibleColumn = null)
        {
            // 'orderby' may arrive null.
            if (orderby == null)
                orderby = new List<ColumnSort>();

            // No user-selected sorting method
            if (orderby.Count == 0)
            {
                // Condition for field type added because sorting by an image field causes an error
                if (firstVisibleColumn != null
                    && CSGenio.business.Area.GetFieldInfo(firstVisibleColumn).FieldType != FieldType.IMAGEM_JPEG
                    && CSGenio.business.Area.GetFieldInfo(firstVisibleColumn).FieldType != FieldType.GEOGRAPHY
                    && CSGenio.business.Area.GetFieldInfo(firstVisibleColumn).FieldType != FieldType.GEO_SHAPE
                    && CSGenio.business.Area.GetFieldInfo(firstVisibleColumn).FieldType != FieldType.GEOMETRIC)
				{
                    ColumnSort sortFirstVisibleColumn = new ColumnSort(new ColumnReference(firstVisibleColumn), SortOrder.Ascending);
                    orderby.Add(sortFirstVisibleColumn);
                }
            }

            SelectQuery subQuery = getQueryPagingPos(areaBase, pkValue, orderby, where, ephs);
            if (subQuery == null) return -1;

			if (Joins != null)
                subQuery.Join(Joins);

            // The select of the primary key is isolated for allow reuse the getQueryPagingPos if it is needed.
            var pk = new ColumnReference(areaBase.Alias, areaBase.PrimaryKeyName);
            subQuery.Select(pk, "pk");

            var query = new SelectQuery()
                .Select("x", "rn", "rowNumber")
                    .From(subQuery, "x")
                        .Where(CriteriaSet.And().Equal("x", "pk", pkValue));

            return Convert.ToInt32(this.ExecuteScalar(query) ?? -1);
        }

        /// <summary>
        /// Method that returns the hashtable with the querys generated by the case
        /// </summary>
        /// <returns></returns>
        [Obsolete("Use IDictionary<string, ControlQueryDefinition> getControlQueries() instead")]
        public static Hashtable getControls()
        {
            return controlos;
        }

        /// <summary>
        /// Method that returns the hashtable with the querys generated by the case
        /// </summary>
        /// <returns></returns>
        [Obsolete("Use IDictionary<string, overrideDbeditQuery> getControlQueriesOverride() instead")]
        public static Dictionary<string, overrideDbedit> getControlsOverrides()
        {
            return controlosOverride;
        }

        /// <summary>
        /// Method that returns the hashtable with the querys generated by the case
        /// </summary>
        /// <returns></returns>
        public static IDictionary<string, ControlQueryDefinition> getControlQueries()
        {
            return controlQueries;
        }

        /// <summary>
        /// Method that returns the hashtable with the querys generated by the case
        /// </summary>
        /// <returns></returns>
        public static IDictionary<string, overrideDbeditQuery> getControlQueriesOverride()
        {
            return controlQueriesOverride;
        }

        /// <summary>
        /// Query for formula to concatenate rows
        /// </summary>
        /// <param name="tableName">Source table name</param>
        /// <param name="argLG">List Aggregate Argument</param>
        /// <param name="relation">Relation</param>
        /// <param name="relValue">Foreign key</param>
        /// <remarks>Moved into the Persistent Support with the purpose of override to support MySQL syntax</remarks>
        /// <returns>Final concatenated text.</returns>
        public virtual string getLGQuery(string tableName, ListAggregateArgument argLG, Relation relation, object relValue)
        {
            return "dbo.GetListAggregate '" + relation.SourceRelField + "', '" + relValue.ToString() + "', '" + tableName + "', '" + argLG.ArgField + "', '" + argLG.SortField + "', '" + argLG.SeparatorField + "'";
        }

        /*****************************Tabelas shadow***********************************/

        /// <summary>
        /// Copys the record into the shadow table
        /// </summary>
        /// <param name="area">Area of the record</param>
		/// <param name="user">Name of the user</param>
		/// <param name="functionType">Function type</param>
        public void requestTabShadow(IArea area, string user, FunctionType functionType)
        {
            try
            {
                //Create the primary key
                string cod = codIntInsertion(area, true);

                //build the insert query
                InsertQuery query = new InsertQuery();
                QueryUtils.buildQueryInsertShadow(query, area, functionType, user);

                //shadow primary key
                query.Value(area.ShadowTabKeyName, QueryUtils.ToValidDbValue(cod, area.DBFields[area.PrimaryKeyName]));

                //insert into the database
                Execute(query);
            }
            catch (PersistenceException ex)
            {
                throw new PersistenceException(ex.UserMessage, "PersistentSupport.pedidoTabSombra",
				                               string.Format("Error duplicating record - [area] {0}; [utilizador] {1}; [tipoFuncao] {2}: ", area.ToString(), user, functionType.ToString()) + ex.Message, ex);
            }
            catch (Exception ex)
            {
                throw new PersistenceException(null, "PersistentSupport.pedidoTabSombra",
				                               string.Format("Error duplicating record - [area] {0}; [utilizador] {1}; [tipoFuncao] {2}: ", area.ToString(), user, functionType.ToString()) + ex.Message, ex);
            }
        }

        /// <summary>
        /// You get a list of records from a single area that follow a condition
        /// </summary>
        /// <typeparam name="A">The area on which the list is based. You must inherit from the table Area</typeparam>
        /// <param name="condicao">The where condition to which records must comply. null to return all fields</param>
        /// <param name="utilizador">The user execution context</param>
        /// <param name="campos">The list of fields to get. null to get all fields</param>
        /// <returns>A list of all records found</returns>
        [Obsolete("Use List<A> searchListWhere<A>(CriteriaSet condicao, User utilizador, string[] campos) instead")]
        public virtual List<A> searchListWhere<A>(string condition, User user, string[] fields) where A : IArea
        {
            try
            {
                List<A> Qresult = new List<A>();
                string query = "";

                //only then can I invoke a static method of class A
                var mi = typeof(A).GetMethod("GetInformation", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
                AreaInfo ai = (AreaInfo)mi.Invoke(null, null);

                QuerySelect qs = new QuerySelect(DatabaseType);
                if (fields != null && fields.Length>0)
                    qs.addSelect(fields);
                else
                {
                    qs.Select = QueryUtils.fieldsToSQL((A)Activator.CreateInstance(typeof(A), user));
                }
                qs.setFromWithAlias(ai.TableName, ai.Alias);
                if (!String.IsNullOrEmpty(condition))
                    qs.Where = new StringBuilder(condition);
                qs.buildQuery();
                query = qs.Query;

                IDbCommand comando = CreateCommand(query);
                IDataReader dr = comando.ExecuteReader();
                string nomeArea = ai.Alias;
                while (dr.Read())
                {
                    A area = (A)Activator.CreateInstance(typeof(A), user);
                    for (int i = 0; i < dr.FieldCount; i++)
                    {
                        // Don't take this condition off. Explanation: The user can remove columns from an area in the settings
                        // and the database still to be had on the corresponding table. If the user querys with "*", the
                        // below call would try to search in the corresponding Area structure all fields returned by SQL query.
                        // Since this field no longer exists in the structure, an exception would obviously arise from this.

                        if (area.DBFields.ContainsKey(dr.GetName(i).ToLower())) //I'm assuming that the keys are the long names and that's never going to change
                            area.insertNameValueField(nomeArea + "." + dr.GetName(i).ToLower(), dr.GetValue(i));
                    }
                    Qresult.Add(area);
                }
                dr.Close();
                return Qresult;
            }
            catch (PersistenceException ex)
            {
                throw new PersistenceException(ex.UserMessage, $"PersistentSupport.searchListWhere for {typeof(A).Name}",
				                               string.Format("Error getting records - [condicao] {0}; [utilizador] {1}; [campos] {2}: ", condition, user.ToString(), fields) + ex.Message, ex);
            }
            catch (Exception ex)
            {
                throw new PersistenceException(null, $"PersistentSupport.searchListWhere for {typeof(A).Name} ",
				                               string.Format("Error getting records - [condicao] {0}; [utilizador] {1}; [campos] {2}: ", condition, user.ToString(), fields) + ex.Message, ex);
            }
        }

        public virtual List<A> searchListWhere<A>(CriteriaSet condition, User user, string[] fields) where A : IArea
        {
            return searchListWhere<A>(condition, user, fields, false);
        }

        public virtual List<A> searchListWhere<A>(CriteriaSet condition, User user, string[] fields, bool distinct) where A : IArea
        {
            return searchListWhere<A>(condition, user, fields, distinct, false);
        }

        public virtual List<A> searchListWhere<A>(CriteriaSet condition, User user, string[] fields, bool distinct, bool noLock) where A : IArea
        {
            try
            {
                List<A> Qresult = new List<A>();

                //only then can I invoke a static method of class A
                var mi = typeof(A).GetMethod("GetInformation", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
                AreaInfo ai = (AreaInfo)mi.Invoke(null, null);

                A area = (A)Activator.CreateInstance(typeof(A), user);

                SelectQuery qs = new SelectQuery();

                qs.Distinct(distinct);

                if (fields != null && fields.Length > 0)
                {
                    foreach (string field in fields)
                    {
                        qs.Select(ai.Alias, field);
                    }
                }
                else
                {
                    foreach (Field field in area.DBFields.Values)
                    {
                        qs.Select(ai.Alias, field.Name);
                    }
                }

                if (condition != null)
                {
                    qs.Where(condition);
                }

                List<string> tabelasAcima = new List<string>();
                if (condition != null)
                {
                    QueryUtils.checkConditionsForForeignTables(condition, area as Area, tabelasAcima);
                }

                List<Relation> relations = QueryUtils.tablesRelationships(tabelasAcima, area);
                QueryUtils.setFromTabDirect(qs, relations, area);

                qs.noLock = noLock;

                DataMatrix mx = Execute(qs);
                for (int i = 0; i < mx.NumRows; i++)
                {
                    area = (A)Activator.CreateInstance(typeof(A), user);
                    for (int j = 0; j < mx.NumCols; j++)
                    {
                        // Don't take this condition off. Explanation: The user can remove columns from an area in the settings
                        // and the database still to be had on the corresponding table. If the user querys with "*", the
                        // below call would try to search in the corresponding Area structure all fields returned by SQL query.
                        // Since this field no longer exists in the structure, an exception would obviously arise from this.
                        if (area.DBFields.ContainsKey(qs.SelectFields[j].Alias.Split('.')[1])) //I'm assuming that the keys are the long names and that's never going to change
                        {
                            area.insertNameValueField(qs.SelectFields[j].Alias, mx.GetDirect(i, j));
                        }
                    }
                    Qresult.Add(area);
                }

                return Qresult;
            }
            catch (PersistenceException ex)
            {
                throw new PersistenceException(ex.UserMessage, $"PersistentSupport.searchListWhere for {typeof(A).Name}",
				                               string.Format("Error getting records - [condicao] {0}; [utilizador] {1}; [campos] {2}; [distinct] {3}: ", condition?.ToString(), user.ToString(), fields, distinct) + ex.Message, ex);
            }
            catch (Exception ex)
            {
                throw new PersistenceException(null, $"PersistentSupport.searchListWhere for {typeof(A).Name}",
				                               string.Format("Error getting records - [condicao] {0}; [utilizador] {1}; [campos] {2}; [distinct] {3}: ", condition?.ToString(), user.ToString(), fields, distinct) + ex.Message, ex);
            }
        }

        /// <summary>
        /// Generics invocation of the searchListWhere method
        /// </summary>
        /// <typeparam name="A"></typeparam>
        /// <param name="condition">The condition.</param>
        /// <param name="user">The user.</param>
        /// <param name="fields">The fields.</param>
        /// <param name="distinct">if set to <c>true</c> [distinct].</param>
        /// <param name="noLock">if set to <c>true</c> [no lock].</param>
        /// <returns>
        ///   <br />
        /// </returns>
        public virtual List<Area> genericSearchListWhere<A>(CriteriaSet condition, User user, string[] fields, bool distinct, bool noLock) where A : IArea
        {
            return searchListWhere<A>(condition, user, fields, distinct, noLock).Cast<Area>().ToList();
        }

        /// <summary>
        /// Run a select query over specified listing, while applying the given conditions.
        /// </summary>
        /// <typeparam name="A">The IArea which this search will be applied to</typeparam>
        /// <param name="condicao">The CriteriaSet holding the conditions</param>
        /// <param name="listing">The ListingMVC</param>
        public virtual void searchListAdvancedWhere<A>(CriteriaSet condition, ListingMVC<A> listing) where A : IArea
        {
            try
            {
                int totalRec = 0;
                SelectQuery qs = null;
                List<A> Qresult = null;

                Type funcObj = typeof(GenioServer.framework.OverrideQuery);
                MethodInfo funcOver = null;

                if(!string.IsNullOrEmpty(listing.identifier))
                    funcOver = funcObj.GetMethod(listing.identifier);

                if (funcOver != null)
                {
                    if (funcOver.ContainsGenericParameters)
                        funcOver = funcOver.MakeGenericMethod(typeof(A));

                    object[] parameters = new object[4];
                    parameters[0] = listing;
                    if(listing.PagingPosEPHs != null)
                        condition.SubSet(listing.PagingPosEPHs);
                    parameters[1] = condition;//CriteriaSet
                    parameters[2] = this;//PersistentSupport
                    parameters[3] = 0; //this will be an output parameter after invoke of the reflection?s method

                    GenioServer.framework.OverrideQuery ovr = new GenioServer.framework.OverrideQuery();
                    Qresult = (List<A>)funcOver.Invoke(ovr, parameters);
                    totalRec = (int) parameters[3];
                }
                else
                {
                    qs = getSelectQueryFromListingMVC(condition, listing);
                    DataMatrix mx = Execute(qs);
                    Qresult = mx.GetList<A>(listing.User);
                }

                listing.Rows = Qresult;// One more record is always selected to check if exist more pages.

                //RS(16.06.2017) If we fetch all the records there is no need to execute a seperate count query
                // The same can be done in the first page if we return less records then the page size
                if (listing.NumRegs <= 0 || (listing.Offset == 0 && listing.Rows.Count < listing.NumRegs))
                    listing.TotalRecords = listing.Rows.Count;
                else if (listing.GetTotal && funcOver == null)
                    listing.TotalRecords = DBConversion.ToInteger(ExecuteScalar(QueryUtils.buildQueryCount(qs)));
                else
                    listing.TotalRecords = totalRec;
            }
            catch (PersistenceException ex)
            {
                throw new PersistenceException(ex.UserMessage, $"PersistentSupport.searchListWhere for {typeof(A).Name}",
                                               string.Format("Error getting records - [condicao] {0}; [listing] {1}: ", condition?.ToString(), listing.ToString()) + ex.Message, ex);
            }
            catch (Exception ex)
            {
                throw new PersistenceException(null, $"PersistentSupport.searchListWhere for {typeof(A).Name}",
                                               string.Format("Error getting records - [condicao] {0}; [listing] {1}: ", condition?.ToString(), listing.ToString()) + ex.Message, ex);
            }
        }

        /// <summary>
        /// Builds the SelectQuery necessary to search in the given listing, while applying the necessary conditions.
        ///
        /// If the base area of the search, does not have any relationship to one of its related areas (specified in the fields or in the conditions),
        /// the base search must change, and the unrelated area will be the area to search. This is necessary for queries on upper tables,
        /// but applying conditions from lower tables, these lower tables are the one that have relantionships to the upper, and therefore the inner joins will be
        /// applied.
        /// </summary>
        /// <typeparam name="A">The IArea which this search will be applied to</typeparam>
        /// <param name="condicao">The CriteriaSet holding the conditions</param>
        /// <param name="listing">The ListingMVC</param>
        /// <returns>SelectQuery</returns>
        public SelectQuery getSelectQueryFromListingMVC<A>(CriteriaSet condition, ListingMVC<A> listing) where A : IArea
        {
            A area = (A)Activator.CreateInstance(typeof(A), listing.User);

            SelectQuery qs = new SelectQuery();

			//NH(2016.09.26) - Set the nolock option
			qs.noLock = listing.NoLock;

            qs.Join(listing.Joins);

            // Sets the fields of the SelectQuery
            setRequestsFields<A>(listing, area, qs);

            // Sets the conditions of the SelectQuery
            setWhereCondition(condition, qs);

            // Checks for foreign tables in fields and conditions
            List<string> relatedTables = checkRelations<A>(condition, listing, area);

            // If the base area does not have relationship with all other foreign areas, then the base area should be changed
            bool areaHasRelationsWithAllOtherAreas = checkPathToRelations(relatedTables, area);

            if(!areaHasRelationsWithAllOtherAreas)
                setFromReversed<A>(relatedTables, area, qs);
            else
                setFrom<A>(relatedTables, area, qs);

            // Sets the ordering, distinct if any, and pagination of the selectQuery
            setOrderDistinctAndPagination<A>(listing, qs);

            return qs;
        }

        /// <summary>
        /// Sets the SelectQuery required fields, which are provided in the listing. If there are no requested fields, sets all fields
        /// from the base area to be returned.
        /// </summary>
        /// <typeparam name="A">The IArea which this search will be applied to</typeparam>
        /// <param name="listing">The ListingMVC</param>
        /// <param name="area">The base area</param>
        /// <param name="qs">SelectQuery</param>
        private static void setRequestsFields<A>(ListingMVC<A> listing, A area, SelectQuery qs) where A : IArea
        {
            if (listing.RequestFields != null && listing.RequestFields.Length > 0)
            {
                foreach (string field in listing.RequestFields)
                {
                    string[] tabelacampo = field.Split('.');
                    string table = tabelacampo[0];
                    string Qfield = tabelacampo[1];

                    var cp = Area.GetInfoArea(table).DBFields[Qfield];

                    qs.Select(cp.Alias, Qfield);
                }
            }
            else
            {
                foreach (Field field in area.DBFields.Values)
                {
                    qs.Select(area.Information.Alias, field.Name);
                }
            }
        }

        /// <summary>
        /// Sets the where condition in a select query, if any.
        /// </summary>
        /// <param name="condicao">The CriteriaSet condition</param>
        /// <param name="qs">The SelectQuery</param>
        private static void setWhereCondition(CriteriaSet condition, SelectQuery qs)
        {
            if (condition != null)
            {
                qs.Where(condition);
            }
        }

        /// <summary>
        /// Checks on conditions and fields used on the selectquery for tables diferent from the base area
        /// </summary>
        /// <typeparam name="A">The IArea which this search will be applied to</typeparam>
        /// <param name="condicao">The CriteriaSet holding the conditions</param>
        /// <param name="listing">The ListingMVC</param>
        /// <param name="area">The base area</param>
        /// <returns>A list of strings with all related areas</returns>
        private List<string> checkRelations<A>(CriteriaSet condition, ListingMVC<A> listing, A area) where A : IArea
        {
            List<string> otherTables = new List<string>();
            if (listing.RequestFields != null)
                QueryUtils.checkFieldsForForeignTables(listing.RequestFieldsAsStringArray, area as Area, otherTables);
            if (condition != null)
                QueryUtils.checkConditionsForForeignTables(condition, area as Area, otherTables);
            return otherTables;
        }

        /// <summary>
        /// Checks if all the given relations are related to the base area, if it is returns true, false otherwise
        /// </summary>
        /// <typeparam name="A">The IArea which this search will be applied to</typeparam>
        /// <param name="relatedTables">List of other tables</param>
        /// <param name="area"></param>
        /// <returns>True or false</returns>
        private bool checkPathToRelations<A>(List<string> relatedTables, A area)where A : IArea
        {
            foreach (string otherTable in relatedTables)
            {
                List<Relation> relations = area.Information.GetRelations(otherTable);
                if (relations == null || relations.Count == 0)
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Sets the from table on the select query, and also all the necessary joins to perform the query
        /// </summary>
        /// <typeparam name="A">The IArea which this search will be applied to</typeparam>
        /// <param name="relatedTables">List of other tables</param>
        /// <param name="area">The base area</param>
        /// <param name="qs">The select query</param>
        private void setFrom<A>(List<string> relatedTables, A area, SelectQuery qs) where A : IArea
        {
            List<Relation> relations = QueryUtils.tablesRelationships(relatedTables, area);

            QueryUtils.setFromTabDirect(qs, relations, area);
        }

        /// <summary>
        /// At this stage there should be only 1 area that on the other tables, but the base area does
        /// not have any relationship with it. But, the other area has a relationship to the base area,
        /// which means that, that one area should be considered for the base area for select query.
        /// All other relationships between these 2 areas should be applied.
        /// </summary>
        /// <typeparam name="A">The IArea which this search will be applied to</typeparam>
        /// <param name="relatedTables">List of other tables</param>
        /// <param name="area">The base area</param>
        /// <param name="qs">The select query</param>
        private void setFromReversed<A>(List<string> otherTables, A area, SelectQuery qs) where A : IArea
        {
            if(otherTables.Count == 1)
            {
                string principalArea = otherTables[0];
                //Type anotherArea = Type.GetType("CSGenio.business.CSGenioA" + principalArea + ", GenioServer");
                //IArea anotherIArea = Activator.CreateInstance(anotherArea, area.User) as IArea;
                IArea anotherIArea = Area.createArea(principalArea, area.User, area.User.CurrentModule);

                List<Relation> relations = invertRelacoesTabelas(area, anotherIArea);
                QueryUtils.setFromTabDirect(qs, relations, anotherIArea);
            }
            else
                setFrom<A>(otherTables, area, qs);
        }

        /// <summary>
        /// Returns all the relationships between the otherArea and area
        /// </summary>
        /// <param name="area">The base area</param>
        /// <param name="anotherIArea">The other area</param>
        /// <returns>List of Relation between anotherArea and area</returns>
        private List<Relation> invertRelacoesTabelas(IArea area, IArea anotherIArea)
        {
            List<Relation> relations = new List<Relation>();

            AreaInfo ai = anotherIArea.Information;
            List<Relation> upperRelations = ai.GetRelations(area.Alias);
            if (upperRelations != null)
            {
                foreach (Relation rel in upperRelations)
                    if (!relations.Contains(rel))
                    {
                        relations.Add(rel);
                    }
            }

            return relations;
        }

        /// <summary>
        /// Sets the order fields, distinct results and pagination to a selectQuery
        /// </summary>
        /// <typeparam name="A">The IArea which this search will be applied to</typeparam>
        /// <param name="listing">The ListingMVC</param>
        /// <param name="qs">SelectQuery</param>
        private static void setOrderDistinctAndPagination<A>(ListingMVC<A> listing, SelectQuery qs) where A : IArea
        {
            qs.OrderByFields.Clear();
            if (listing.Sorts != null)
            {
                foreach (ColumnSort sort in listing.Sorts)
                {
                    qs.OrderByFields.Add(sort);
                }
            }

            qs.Distinct(listing.Distinct);

            if (listing.NumRegs > 0)
            {
                qs.PageSize(listing.NumRegs + 1);
                qs.Offset(listing.Offset);
            }
        }

        /// <summary>
        /// Gets data from a record from the primary key
        /// </summary>
        /// <param name="area">The area to be filled with the values</param>
        /// <param name="valorCodigoInterno">The value of the primary key with which we position the record</param>
        /// <param name="campos">The fields to fill in the area. Null to get all fields</param>
        /// <returns>True if the record was correctly positioned, false otherwise</returns>
        public virtual bool getRecord(IArea area, object internalCodeValue, string[] fields)
        {
            try
            {
                bool result = false;

                SelectQuery select = new SelectQuery();
                if (fields != null && fields.Length > 0)
                {
                    foreach (string field in fields)
                    {
                        select.Select(area.Alias, field);
                    }
                }
                else
                {
                    foreach (Field field in area.DBFields.Values)
                    {
                        select.Select(area.Alias, field.Name);
                    }
                }
                select.From(area.QSystem, area.TableName, area.Alias);
                select.Where(CriteriaSet.And()
                    .Equal(area.Alias, area.PrimaryKeyName, internalCodeValue));

                DataMatrix mx = Execute(select);
                if (mx.NumRows > 0)
                {
                    result = true;

                    for (int i = 0; i < mx.NumCols; i++)
                    {
                        // Don't take this condition off. Explanation: The user can remove columns from an area in the settings
                        // and the database still to be had on the corresponding table. If the user querys with "*", the
                        // below call would try to search in the corresponding Area structure all fields returned by SQL query.
                        // Since this field no longer exists in the structure, an exception would obviously arise from this.
                        if (area.DBFields.ContainsKey(select.SelectFields[i].Alias.Split('.')[1])) //I'm assuming that the keys are the long names and that's never going to change
                        {
                            area.insertNameValueField(select.SelectFields[i].Alias, mx.GetDirect(0, i));
                        }
                    }
                }

                return result;
            }
			catch (GenioException ex)
            {
                throw new PersistenceException(ex.UserMessage, "PersistentSupport.getRecord",
				                               "Error selecting fields " + fields + " from table " + area.TableName + " where code is " + internalCodeValue.ToString() + ": " + ex.Message, ex);
            }
            catch (Exception ex)
            {
                // closeConnection(); to have a uniform behavior with other catch of other sp functions and not close the connection for no apparent reason to this.
                throw new PersistenceException(null, "PersistentSupport.getRecord",
				                               "Error selecting fields " + fields + " from table " + area.TableName + " where code is " + internalCodeValue.ToString() + ": " + ex.Message, ex);
            }
        }

        /// <summary>
        /// Function that executes a query that returns a field
        /// </summary>
        /// <param name="query">query to run</param>
        /// <returns>the object returned by the query</returns>
        public virtual object executeScalar(string query, int timeout)
        {
            try
            {
                if (Log.IsDebugEnabled) Log.Debug(string.Format("[QueryEscalar] {0}.", query));

                IDbCommand comando = CreateCommand(query);

                return comando.ExecuteScalar();
            }
            catch (GenioException ex)
            {
                throw new PersistenceException(ex.UserMessage, "PersistentSupport.executaEscalar",
                                               "Error executing query '" + query +
                                               //(parameters == null ? "" : "' with parameters " + parameters.ToString()) +
                                                ": " + ex.Message, ex);
            }
            catch (Exception ex)
            {
                throw new PersistenceException(null, "PersistentSupport.executaEscalar",
                                               "Error executing query '" + query +
                                               //(parameters == null ? "" : "' with parameters " + parameters.ToString()) +
                                                ": " + ex.Message, ex);
            }
        }

        /// <summary>
        /// Function that executes a query that returns a field
        /// </summary>
        /// <param name="query">query to run</param>
        /// <returns>the object returned by the query</returns>
        public virtual object executeScalar(string query)
        {
			return executeScalar(query, null);
        }

        /// <summary>
        /// Function that executes a query with parameters that returns a field
        /// </summary>
        /// <param name="query">query to run</param>
        /// <param name="parameters">query parameters</param>
        /// <returns>the object returned by the query</returns>
        public virtual object executeScalar(string query, List<IDbDataParameter> parameters)
        {
            try
            {
                if (Log.IsDebugEnabled) Log.Debug(string.Format("[QueryEscalar] {0}.", query));

                IDbCommand comando = CreateCommand(query, parameters);
                return comando.ExecuteScalar();
            }
			catch (GenioException ex)
            {
                throw new PersistenceException(ex.UserMessage, "PersistentSupport.executaEscalar",
				                               "Error executing query '" + query +
											   (parameters == null ? "" : "' with parameters " + parameters.ToString())
											   + ": " + ex.Message, ex);
            }
            catch (Exception ex)
            {
                throw new PersistenceException(null, "PersistentSupport.executaEscalar",
				                               "Error executing query '" + query +
											   (parameters == null ? "" : "' with parameters " + parameters.ToString())
											   + ": " + ex.Message, ex);
            }
        }

		public virtual object executeScalar(SelectQuery query)
        {
            return ExecuteScalar(query);
        }

        public virtual object ExecuteScalar(SelectQuery query)
        {
            try
            {
                DataMatrix mx = Execute(query);
                if (mx == null || mx.NumRows == 0)
                {
                    return null;
                }

                return mx.GetDirect(0, 0);
            }
			catch (GenioException ex)
            {
                throw new PersistenceException(ex.UserMessage, "PersistentSupport.ExecuteScalar",
				                               "Error executing query '" + query.ToString() + "': " + ex.Message, ex);
            }
            catch (Exception ex)
            {
                throw new PersistenceException(null, "PersistentSupport.ExecuteScalar",
				                               "Error executing query '" + query.ToString() + "': " + ex.Message, ex);
            }
        }

		public virtual object ExecuteScalar(string query)
        {
            return executeScalar(query);
        }

        /// <summary>
        /// Function that executes a query that returns a column of records
        /// </summary>
        /// <param name="query">query to be executed</param>
        /// <returns>an arraylist with the returned values</returns>
        [Obsolete("Use ArrayList executaReaderUmaColuna(SelectQuery query) instead")]
        public virtual ArrayList executeReaderOneColumn(string query)
        {
            try
            {
                if (Log.IsDebugEnabled) Log.Debug(string.Format("[QueryUmaColuna] {0}.", query));

                ArrayList Qresult = new ArrayList();
                IDbCommand comando = CreateCommand(query);
                IDataReader sqlDR = comando.ExecuteReader();
                while (sqlDR.Read())
                {
                    Qresult.Add(sqlDR.GetValue(0));
                }
                sqlDR.Close();
                return Qresult;
            }
			catch (GenioException ex)
            {
                throw new PersistenceException(ex.UserMessage, "PersistentSupport.executaReaderUmaColuna",
				                               "Error executing query '" + query + "': " + ex.Message, ex);
            }
            catch (Exception ex)
            {
                throw new PersistenceException(null, "PersistentSupport.executaReaderUmaColuna",
				                               "Error executing query '" + query + "': " + ex.Message, ex);
            }
        }

        public ArrayList executeReaderOneColumn(SelectQuery query)
        {
            try
            {
                if (Log.IsDebugEnabled) Log.Debug(string.Format("[QueryUmaColuna] {0}.", query));

                ArrayList Qresult = new ArrayList();
                DataMatrix mx = Execute(query);
                for (int i = 0; i < mx.NumRows; i++)
                {
                    Qresult.Add(mx.GetDirect(i, 0));
                }
                return Qresult;
            }
			catch (GenioException ex)
            {
                throw new PersistenceException(ex.UserMessage, "PersistentSupport.executaReaderUmaColuna",
				                               "Error executing query '" + query.ToString() + "': " + ex.Message, ex);
            }
            catch (Exception ex)
            {
                throw new PersistenceException(null, "PersistentSupport.executaReaderUmaColuna",
				                               "Error executing query '" + query.ToString() + "': " + ex.Message, ex);
            }
        }

        /// <summary>
        /// Function that executes a query that returns a database record
        /// </summary>
        /// <param name="query">query to be executed</param>
        /// <returns>an arraylist with the returned values</returns>
        [Obsolete("Use ArrayList executaReaderUmaLinha(SelectQuery query) instead")]
        public virtual ArrayList executeReaderOneRow(string query)
        {
            try
            {
                if (Log.IsDebugEnabled) Log.Debug(string.Format("[QueryUmaLinha] {0}.", query));

                ArrayList Qresult = new ArrayList();
                IDbCommand comando = CreateCommand(query);
                IDataReader dr = comando.ExecuteReader();
                if (dr.Read())
                {
                    int nrColunas = dr.FieldCount;
                    for (int i = 0; i < nrColunas; i++)
                        Qresult.Add(dr.GetValue(i));
                }
                dr.Close();
                return Qresult;
            }
			catch (GenioException ex)
            {
                throw new PersistenceException(ex.UserMessage, "PersistentSupport.executaReaderUmaLinha",
				                               "Error executing query '" + query + "': " + ex.Message, ex);
            }
            catch (Exception ex)
            {
                throw new PersistenceException(null, "PersistentSupport.executaReaderUmaLinha",
				                               "Error executing query '" + query + "': " + ex.Message, ex);
            }
        }

        public ArrayList executeReaderOneRow(SelectQuery query)
        {
            try
            {
                if (Log.IsDebugEnabled) Log.Debug(string.Format("[QueryUmaLinha] {0}.", query));

                ArrayList Qresult = new ArrayList();
                DataMatrix mx = Execute(query);
                if (mx != null && mx.NumRows > 0)
                {
                    for (int i = 0; i < mx.NumCols; i++)
                    {
                        Qresult.Add(mx.GetDirect(0, i));
                    }
                }
                return Qresult;
            }
			catch (GenioException ex)
            {
                throw new PersistenceException(ex.UserMessage, "PersistentSupport.executaReaderUmaLinha",
				                               "Error executing query '" + query.ToString() + "': " + ex.Message, ex);
            }
            catch (Exception ex)
            {
                throw new PersistenceException(null, "PersistentSupport.executaReaderUmaLinha",
				                               "Error executing query '" + query.ToString() + "': " + ex.Message, ex);
            }
        }

        /// <summary>
        /// Function that runs a query update, insert, or delete
        /// </summary>
        /// <remarks>
        /// RR 24-02-2011 - this function now receives the list of
        /// parameters to bind before running a query
        /// </remarks>
        /// <param name="query">Query to run</param>
        /// <returns>value returned by executeNonQuery</returns>
        [Obsolete("Já não existe necessidade de usar esta função directamente. Usar as classes de Quidgest.Persistence.")]
        public virtual int executeNonQuery(string query, List<Parameter> parameters)
        {
            try
            {
                if (Log.IsDebugEnabled) Log.Debug(string.Format("[NonQuery] {0}.", query));

                IDbCommand comando = CreateCommand(query);

                // RR 24-02-2011 - binds the parameters with the values
                foreach (Parameter p in parameters)
                {
                    IDbDataParameter pcmd = comando.CreateParameter();
                    pcmd.DbType = p.Type;
                    pcmd.ParameterName = Dialect.GetParameterBindIdentifier(p.Name);
                    // RR 25/02/2011
                    // here is used the entire maximum to keep this generic code
                    // the value of the size has to be passed to the provider, but when you try to
                    // introduce byte empty array, was giving problems because the size is 0
                    // with -1 worked only on SQL Server, so it works in all cases
                    // See the following links, to get an idea of the problem:
                    // http://stackoverflow.com/questions/596257/what-sqldbtype-maps-to-varcharmax
                    // http://stackoverflow.com/questions/4221196/how-to-prepare-an-ado-net-statement-which-includes-an-ntext-clob-parameter
                    pcmd.Size = Int32.MaxValue;
                    pcmd.Value = p.Value;
                    pcmd.Direction = ParameterDirection.Input;
                    comando.Parameters.Add(pcmd);
                }

                if (parameters.Count > 0)
                    comando.Prepare();

                return comando.ExecuteNonQuery();
            }
			catch (GenioException ex)
            {
                throw new PersistenceException(ex.UserMessage, "PersistentSupport.executaNonQuery",
				                               "Error executing query '" + query + "' with parameters " + parameters.ToString() + ": " + ex.Message, ex);
            }
            catch (Exception ex)
            {
                throw new PersistenceException(null, "PersistentSupport.executaNonQuery",
				                               "Error executing query '" + query + "' with parameters " + parameters.ToString() + ": " + ex.Message, ex);
            }
        }

        /// <summary>
        /// Function that runs a query update, insert, or delete
        /// </summary>
        /// <param name="query">Query to run</param>
        /// <returns>value returned by executeNonQuery</returns>
        public virtual int executeNonQuery(string query)
        {
#pragma warning disable 618 //TODO ficam marcados assim explicitamente to saber quer tem de ser mudado
            return executeNonQuery(query, new List<Parameter>());
#pragma warning restore 618
        }

        /// <summary>
        /// Function that performs a stored procedure that returns a field, with a default timeout of 300
        /// </summary>
        /// <param name="query">a stored procedure to run</param>
        /// <returns>value returned by executeNonQuery</returns>
        public virtual int executeStoredProcedure(string query)
        {
            int timeout = 300;
            return executeStoredProcedure(query, timeout);
        }

        /// <summary>
        /// Function that performs a stored procedure that returns a field
        /// </summary>
        /// <param name="query">a stored procedure to run</param>
        /// <param name="timeout">timeout value in ms</param>
        /// <returns>value returned by executeNonQuery</returns>
        public virtual int executeStoredProcedure(string query, int timeout)
        {
            try
            {
                if (Log.IsDebugEnabled) Log.Debug(string.Format("[StoredProcedure] {0}.", query));

                IDbCommand comando = CreateCommand(query);
                //In the case of a stored procedure there will only be time out after 5 min
                comando.CommandTimeout = timeout;
                return comando.ExecuteNonQuery();
            }
			catch (GenioException ex)
            {
				throw new PersistenceException(ex.UserMessage, "PersistentSupport.executaStoredProcedure",
				                               "Error executing stored procedure '" + query + "' with timeout " + timeout.ToString() + ": " + ex.Message, ex);
            }
            catch (Exception ex)
            {
				throw new PersistenceException(ex.Message, "PersistentSupport.executaStoredProcedure",
				                               "Error executing stored procedure '" + query + "' with timeout " + timeout.ToString() + ": " + ex.Message, ex);
            }
        }

        /// <summary>
        /// Create a command from a query using the current transaction if it exists
        /// </summary>
        /// <param name="query">The query with which to construct the command</param>
        /// <returns>The database command</returns>
        protected virtual IDbCommand CreateCommand(string query)
        {
            return CreateCommand(query, null);
        }

        /// <summary>
        /// Create a command from a query using the current transaction if it exists
        /// </summary>
        /// <param name="query">The query with which to construct the command</param>
        /// <param name="parameters">List of parameters to the command</param>
        /// <returns>The database command</returns>
        protected virtual IDbCommand CreateCommand(string query, IList<IDbDataParameter> parameters)
        {
            IDbCommand comando = Connection.CreateCommand();
            if (Transaction != null)
                comando.Transaction = Transaction;
            comando.CommandText = query;
            AddParameters(comando, parameters);

            //CHN + MA timeout read by XML file
            if (Configuration.ExistsProperty("CommandTimeout"))
            {
                comando.CommandTimeout = Convert.ToInt32(Configuration.GetProperty("CommandTimeout"));
            }

            //CHN applies sp global connection timeout (if set)
            if(Timeout>0)
                comando.CommandTimeout = Timeout;

            return comando;
        }

        public abstract IDbDataParameter CreateParameter();

        public virtual IDbDataParameter CreateParameter(object value)
        {
            var p = CreateParameter();
            p.Value = value ?? DBNull.Value;
			if (!Configuration.IsDbUnicode && value != null && value.GetType() == typeof(string))
                p.DbType = DbType.AnsiString;
            return p;
        }

        public virtual IDbDataParameter CreateParameter(string name, object value)
        {
            IDbDataParameter p = CreateParameter(value);
            p.ParameterName = (Dialect.UseNamedPrefixInParameter ? Dialect.NamedPrefix : "") + name;
            return p;
        }

        public virtual void AddParameters(IDbCommand command, IList<IDbDataParameter> parameters)
        {
            if (parameters != null)
            {
                foreach (IDbDataParameter param in parameters)
                {
                    command.Parameters.Add(param);
                }
            }
        }

        /// <summary>
        /// Function that executes a query that returns a database record
        /// </summary>
        /// <param name="query">query to be executed</param>
        /// <returns>Area with filled values</returns>
        public virtual void getRecord(IArea area, object internalCodeValue, bool forUpdate=false)
        {
            try
            {
                Log.Debug(string.Format("Executa query getRecord. [valorCodigoInterno] {0}", DBConversion.FromKey(internalCodeValue.ToString())));

                SelectQuery select = new SelectQuery();

                if (forUpdate)
                    select.updateLock = true;

                foreach (Field Qfield in area.DBFields.Values)
                {
                    select.Select(area.Alias, Qfield.Name);
                }

                select.From(area.QSystem, area.TableName, area.Alias);
                select.Where(CriteriaSet.And()
                    .Equal(area.Alias, area.PrimaryKeyName, internalCodeValue));

                DataMatrix mx = Execute(select);

                for (int i = 0; i < mx.NumCols; i++)
                {
                    area.insertNameValueField(select.SelectFields[i].Alias, mx.GetDirect(0, i));
                }

                return;
            }
			catch (GenioException ex)
            {
				if (ex.UserMessage == null)
					throw new PersistenceException("Erro ao obter registo.", "PersistentSupport.getRecord",
				                                   "Error selecting record from table " + area.TableName + " where code is " + internalCodeValue.ToString() + ": " + ex.Message, ex);
				else
					throw new PersistenceException("Erro ao obter registo: " + ex.UserMessage, "PersistentSupport.getRecord",
				                                   "Error selecting record from table " + area.TableName + " where code is " + internalCodeValue.ToString() + ": " + ex.Message, ex);
            }
            catch (Exception ex)
            {
                // closeConnection(); to have a uniform behavior with other catch of other sp functions and not close the connection for no apparent reason to this.
				throw new PersistenceException("Erro ao obter registo.", "PersistentSupport.getRecord",
				                               "Error selecting record from table " + area.TableName + " where code is " + internalCodeValue.ToString() + ": " + ex.Message, ex);
            }
        }

        /// <summary>
        /// Function that executes a query that returns a database record
        /// </summary>
        /// <param name="query">query to be executed</param>
        /// <returns>Area with filled values</returns>
        public virtual void getRecord(string query, IList<IDbDataParameter> parameters, IArea area)
        {
            try
            {
                Log.Debug("PersistentSupport.getRecord");

                if (Log.IsDebugEnabled) Log.Debug(string.Format("[QueryReader] {0}.", query));

                IDbCommand comando = CreateCommand(query, parameters);
                IDataReader dr = comando.ExecuteReader();
                if (dr.Read())
                {
                    int nrColunas = dr.FieldCount;
                    for (int i = 0; i < nrColunas; i++)
                    {
                        // Don't take this condition off. Explanation: The user can remove columns from an area in the settings
                        // and the database still to be had on the corresponding table. If the user querys with "*", the
                        // below call would try to search in the corresponding Area structure all fields returned by SQL query.
                        // Since this field no longer exists in the structure, an exception would obviously arise from this.

                        if (area.DBFields.ContainsKey(dr.GetName(i).ToLower())) //I'm assuming that the keys are the long names and that's never going to change
                            area.insertNameValueField(area.Alias + "." + dr.GetName(i).ToLower(), dr.GetValue(i));
                    }
                }
                dr.Close();
                return;
            }
			catch (GenioException ex)
            {
				if (ex.UserMessage == null)
					throw new PersistenceException("Erro ao obter registo.", "PersistentSupport.getRecord",
												   "Error executing query '" + query + "' on area " + area.ToString() + ": " + ex.Message, ex);
				else
					throw new PersistenceException("Erro ao obter registo: " + ex.UserMessage, "PersistentSupport.getRecord",
												   "Error executing query '" + query + "' on area " + area.ToString() + ": " + ex.Message, ex);
            }
            catch (Exception ex)
            {
                // closeConnection(); to have a uniform behavior with other catch of other sp functions and not close the connection for no apparent reason to this.
                throw new PersistenceException("Erro ao obter registo.", "PersistentSupport.getRecord",
				                               "Error executing query '" + query + "' on area " + area.ToString() + ": " + ex.Message, ex);
            }
        }

        /// <summary>
        /// Function that executes a query
        /// </summary>
        /// <param name="query">query to be executed</param>
        /// <returns>the dataSet with the data</returns>
        public virtual DataMatrix executeQuery(string query)
        {
            try
            {
                if (Log.IsDebugEnabled) Log.Debug(string.Format("[QueryDataSet] {0}.", query));

                //initialize the data set
                DataSet ds = new DataSet();

                //run the Qresult query
                IDbDataAdapter da = CreateAdapter(query);
                da.Fill(ds);
                return new DataMatrix(ds);
            }
            catch (Exception ex)
            {
				throw new PersistenceException(null, "PersistentSupport.executaQuery",
				                               "Error executing query '" + query + "': " + ex.Message, ex);
            }
        }

		/// <summary>
        /// Function that executes a query
        /// </summary>
        /// <param name="query">Query</param>
        /// <param name="parameters">List of parameters</param>
        /// <returns></returns>
        public DataMatrix executeQuery(string query, List<IDbDataParameter> parameters)
        {
            try
            {
                if (Log.IsDebugEnabled) Log.Debug(string.Format("[QueryDataSet] {0}.", query));

                IDbDataAdapter adapter = CreateAdapter(query);
                AddParameters(adapter.SelectCommand, parameters);

                DataSet ds = new DataSet();
                adapter.Fill(ds);

                return new DataMatrix(ds);
            }
            catch (Exception ex)
            {
                throw new PersistenceException(null, "PersistentSupport.executaQuery",
                                               "Error executing query '" + query + "': " + ex.Message, ex);
            }
        }


        /// <summary>
        /// Function that executes a query
        /// </summary>
        /// <param name="query">query to be executed</param>
        /// <param name="paramList">query parameters</param>
        /// <returns>the dataSet with the data</returns>
        public virtual DataMatrix executeQuery(string query, IDictionary<string, ParameterQuery> paramList)
        {
            try
            {
                if (Log.IsDebugEnabled) Log.Debug(string.Format("[QueryDataSet] {0}.", query));

                //initialize the data set
                DataSet ds = new DataSet();

                //run the Qresult query
                List<IDbDataParameter> parameters = new List<IDbDataParameter>();

                foreach (var param in paramList)
                    parameters.Add(CreateParameter(param.Key,param.Value.Value));

                IDbDataAdapter adapter = CreateAdapter(query);

                AddParameters(adapter.SelectCommand, parameters);

                adapter.Fill(ds);

                return new DataMatrix(ds);
            }
            catch (Exception ex)
            {
				throw new PersistenceException(null, "PersistentSupport.executaQuery",
				                               "Error executing query '" + query + "' with parameters " + paramList.ToString() + ": " + ex.Message, ex);
            }
        }

        /// <summary>
        /// Function that executes a query
        /// </summary>
        /// <param name="query">query to be executed</param>
        /// <returns>the dataSet with the data</returns>
        [Obsolete("Please use executaQuery instead")]
        public virtual DataSet executeReaderDataSet(string query)
        {
            try
            {
                if (Log.IsDebugEnabled) Log.Debug(string.Format("[QueryDataSet] {0}.", query));

                //initialize the data set
                DataSet ds = new DataSet();

                //run the Qresult query
                IDbDataAdapter da = CreateAdapter(query);
                da.Fill(ds);
                return ds;
            }
            catch (Exception ex)
            {
                throw new PersistenceException(null, "PersistentSupport.executaReaderDataSet",
				                               "Error executing query '" + query + "': " + ex.Message, ex);
            }
        }


        /*********MÉTODOS ABSTRACT*****************************************************/

        /// <summary>
        /// Instantiates a new Sql adapter
        /// </summary>
        /// <param name="query">The adapter boot query</param>
        /// <returns>A sql adapter</returns>
        public abstract IDbDataAdapter CreateAdapter(string query);

        /// <summary>
        /// Returns a connection to the server, without the database. Useful for when the database doesn't exist.
        /// </summary>
        public abstract IDbConnection GetConnectionToServer();


        /// <summary>
        /// Checks if a database exists
        /// </summary>
        /// <param name="database">The database name. If empty the default for this Persistent Support will be used.</param>
        /// <returns>True if the database exists</returns>
        public abstract bool CheckIfDatabaseExists(string database="");

        /// <summary>
        /// Drops the specified database
        /// </summary>
        /// <param name="schema">Database name</param>
        public abstract void Drop(string schema);

        /*********FIM DOS MÉTODOS ABSTRACT*****************************************************/

        /**************************** Ficheiros no disco **************************************/
        #region Get/Set do File no disco
        /// <summary>
        /// Burn the file to disk
        /// </summary>
        /// <param name="valorChaveDocums">Docums PK</param>
        /// <param name="domain">Domain table name"</param>
        /// <param name="ficheiro">Byte array corresponding to file</param>
        /// <param name="extensao">File extension</param>
        /// <returns>The path "relative" to file. ([DOMAIN]\[N_FOLDER]\File)</returns>
        private string saveFileToDisk(object keyValueDocums, Byte[] file, string domain, string extension)
        {
            const int MAX_FILES = 7500;
            // Validation of parameters
            if (string.IsNullOrEmpty(Configuration.PathDocuments))
                throw new PersistenceException("Não é possível gravar o ficheiro.", "PersistentSupport.saveFileToDisk", "The file path is not defined.");
            if (string.IsNullOrEmpty(domain) || file == null)
                throw new PersistenceException("Não é possível gravar o ficheiro.", "PersistentSupport.saveFileToDisk", "Arguments [DOMAIN] and/or [ficheiro] are null.");

            // Assign a unique identifier to the file
            string primaryKey = Convert.ToString(keyValueDocums);
            if (string.IsNullOrEmpty(primaryKey)) primaryKey = Guid.NewGuid().ToString();
            else primaryKey = primaryKey.TrimStart(' ');
            string fileName = string.Format("{0}.{1}", primaryKey, extension);


            // Target path construction (PathDocuments\[DOMAIN]\[YEAR]\[N_FOLDER])
            string basePath = System.IO.Path.Combine(Configuration.PathDocuments, domain.ToUpper());
            System.IO.Directory.CreateDirectory(basePath);
            string subFolder = string.Empty;
            string folderPath = string.Empty;
            int year = DateTime.Now.Year;

            SelectQuery query = new SelectQuery()
                .Select(SqlFunctions.Count("1"), "count")
                .From("docums")
                .Where(CriteriaSet.And()
                 .Equal(CSGenioAdocums.FldTabela, domain)
                 .Equal(CSGenioAdocums.FldZzstate, 0)
                 .Equal(SqlFunctions.Year(CSGenioAdocums.FldDatacria), year));
            query.noLock = true;

            int int_folder = (DBConversion.ToInteger(executeScalar(query)) / MAX_FILES) + 1;
            do
            {
                subFolder = System.IO.Path.Combine(string.Format("{0}", year), string.Format("{0}", int_folder));
                folderPath = System.IO.Path.Combine(basePath, subFolder);
                System.IO.Directory.CreateDirectory(folderPath);
                int_folder++;
            } while (System.IO.Directory.GetFiles(folderPath).Length > MAX_FILES);


            // File writing on disk
            string path = System.IO.Path.Combine(folderPath, fileName);
            System.IO.File.WriteAllBytes(path, file);

            // Return of the path "relative" to file. ([DOMAIN]\[N_FOLDER]\File)
            return System.IO.Path.Combine(domain.ToUpper(), System.IO.Path.Combine(subFolder, fileName));
        }
        /// <summary>
        /// MH (15/03/2017) - Get the file burned to disk
        /// </summary>
        /// <param name="filepath">The path "relative" to file. ([AREA]\[N_FOLDER]\File)</param>
        /// <returns></returns>
        public static Byte[] getFileFromDisk(string filepath)
        {
            // Validation of parameters
            if (string.IsNullOrEmpty(Configuration.PathDocuments))
                throw new PersistenceException("Não é possível obter o ficheiro: " + filepath, "PersistentSupport.getFileFromDisk", "The file path is not defined.");

            if (!string.IsNullOrEmpty(filepath))
            {
                string path = System.IO.Path.Combine(Configuration.PathDocuments, filepath);
                if (System.IO.File.Exists(path))
                    return System.IO.File.ReadAllBytes(path);
            }

            return new Byte[0];
        }

        /// <summary>
        /// MH (15/03/2017) - Get the file burned to the disc.
        /// Alternative version of get
        /// </summary>
        /// <param name="coddocums">Docums registry key value</param>
        /// <returns></returns>
        public Byte[] _getFileFromDisk(string coddocums)
        {
            // Validation of parameters
            if (string.IsNullOrEmpty(Configuration.PathDocuments))
                throw new PersistenceException("Não é possível obter o ficheiro.", "PersistentSupport._getFileFromDisk", "The file path is not defined.");

            try
            {
                string tableName = "docums";
                SelectQuery qs = new SelectQuery()
                .Select(tableName, "document")
                .From(tableName)
                .Where(CriteriaSet.And()
                    .Equal(tableName, "coddocums", coddocums))
                .PageSize(1);

                ArrayList results = executeReaderOneRow(qs);
                return PersistentSupport.getFileFromDisk(DBConversion.ToString(results[0]));
            }
            catch (Exception en)
            {
                throw new BusinessException("Não é possível obter o ficheiro.", "PersistentSupport._getFileFromDisk", "Error getting file " + coddocums + ": " + en.Message, en);
            }
        }

        private string changeFileOnDisk(object keyValueDocums, Byte[] file, string area, string extension)
        {
            // Validation of parameters
            if (string.IsNullOrEmpty(Configuration.PathDocuments))
                throw new PersistenceException("Não é possível alterar o ficheiro.", "PersistentSupport.changeFileOnDisk", "The file path is not defined.");
            if (keyValueDocums == null || file == null)
                throw new PersistenceException("Não é possível alterar o ficheiro.", "PersistentSupport.changeFileOnDisk", "Arguments area valorChaveDocums and/or ficheiro are null.");

            try
            {
                // Read from BD the current path to file
                string tableName = "docums";
                SelectQuery qs = new SelectQuery()
                    .Select(tableName, "document")
                    .From(tableName)
                    .Where(CriteriaSet.And()
                        .Equal(tableName, "coddocums", keyValueDocums));

                ArrayList results = executeReaderOneRow(qs);
                string path = System.IO.Path.Combine(Configuration.PathDocuments, DBConversion.ToString(results[0]));

                if (System.IO.File.Exists(path))
                    System.IO.File.Delete(path);
                return saveFileToDisk(keyValueDocums, file, area, extension);
            }
            catch (Exception en)
            {
				throw new PersistenceException("Não é possível alterar o ficheiro.", "PersistentSupport.changeFileOnDisk",
				                               string.Format("Error changing document - [valorChaveDocums] {0}; [area] {1}; [extensao] {2}", keyValueDocums.ToString(), area, extension) + en.Message, en);
            }
        }

        private string duplicateFileOnDisk(object keyValueDocums, string sourceFilePath, string area, string extension)
        {
            return saveFileToDisk(keyValueDocums, getFileFromDisk(sourceFilePath), area, extension);
        }

        private void removeFileFromDisk(object keyNameDocum, object keyValueDocums)
        {
            // Validation of parameters
            if (string.IsNullOrEmpty(Configuration.PathDocuments))
                throw new PersistenceException("Não é possível apagar o ficheiro.", "PersistentSupport.removeFileFromDisk", "The file path is not defined.");
            // Read from BD the current path to file
            try
            {
                string tableName = "docums";
                SelectQuery qs = new SelectQuery()
                    .Select(tableName, "docpath")
                    .From(tableName)
                    .Where(CriteriaSet.And()
                        .Equal(tableName, Convert.ToString(keyNameDocum), keyValueDocums));

                ArrayList results = executeReaderOneRow(qs);
                string filePath = DBConversion.ToString(results[0]);
                string path = System.IO.Path.Combine(Configuration.PathDocuments, filePath);
                if(System.IO.File.Exists(path)) System.IO.File.Delete(path);
            }
            catch (Exception en)
            {
                throw new PersistenceException("Não é possível apagar o ficheiro.", "PersistentSupport.removeFileFromDisk",
				                               string.Format("Error removing document - [nomeChaveDocum] {0}; [valorChaveDocums] {1}", keyNameDocum.ToString(), keyValueDocums.ToString()) + en.Message, en);
            }
        }
        #endregion
    }
}
