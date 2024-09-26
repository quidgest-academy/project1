using System;
using System.Data;
using System.Data.SqlClient;
using CSGenio.framework;
using CSGenio.business;
using Quidgest.Persistence.Dialects;
using Quidgest.Persistence.GenericQuery;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace CSGenio.persistence
{
    /// <summary>
    /// Summary description for PersistentSupportSQLServer2000.
    /// </summary>

    public class PersistentSupportSQLServer2000 : PersistentSupport
    {
        public override IDbDataParameter CreateParameter()
        {
            return new SqlParameter();
        }

        /// <summary>
        /// Contructor
        /// </summary>
        public PersistentSupportSQLServer2000()
        {
			Dialect = new SqlServer2000Dialect();
        }

        protected override void BuildConnection(DataSystemXml dataSystem, string login, string password, int connectionTimeout = 0)
        {
            SqlConnectionStringBuilder csb = new SqlConnectionStringBuilder();

            string port = string.Empty;
            if (!string.IsNullOrEmpty(dataSystem.Port))
                port = "," + dataSystem.Port;

            csb.DataSource = dataSystem.Server + port;
            csb.InitialCatalog = IsMaster ? "Master" : dataSystem.Schemas[0].Schema;
            if (login == null || password == null)
            {
                csb.UserID = dataSystem.LoginDecode();
                csb.Password = dataSystem.PasswordDecode();
            }
            else
            {
                csb.UserID = login;
                csb.Password = password;
            }
            if (!string.IsNullOrEmpty(ClientId))
                csb.WorkstationID = ClientId;
            if (ReadOnly)
                csb.ApplicationIntent = ApplicationIntent.ReadOnly;
            if (dataSystem.Schemas[0].ConnWithDomainUser)
                csb.IntegratedSecurity = true;
            if (dataSystem.Schemas[0].ConnEncrypt)
            {
                csb.Encrypt = true;
                csb.TrustServerCertificate = true;
            }
            if (connectionTimeout > 0)
                csb.ConnectTimeout = connectionTimeout;

            Connection = new SqlConnection(csb.ToString());
        }

        protected override string TransformSchemaName(string schema)
        {
            return schema + ".dbo";
        }

        /// <summary>
        /// Instancia um novo adaptador de Sql
        /// </summary>
        /// <param name="query">A query de inicialização do adaptador</param>
        /// <returns>Um adaptador de sql</returns>
        public override IDbDataAdapter CreateAdapter(string query)
        {
            IDbCommand command = CreateCommand(query);
            return new SqlDataAdapter((SqlCommand)command);
        }

        /// <summary>
        /// Obtem uma nova key primária to um determinado objecto
        /// </summary>
        /// <param name="id_objecto">O objecto to o qual se quer gerar uma key, tipicamente o name da table</param>
        /// <param name="tamanho">O size da key a gerar to o caso de codigo internos</param>
        /// <param name="formato">O format de key a gerar</param>
        /// <returns>Uma key primária única</returns>
        public override string generatePrimaryKey(string id_object, int size, CodeType format)
        {
			if (format.Equals(CodeType.GUID_KEY))
                return Guid.NewGuid().ToString();

            string sql = "dbo.updateCod '" + id_object + "', 1";
            Int64 codigoNovo = (Int64)executeScalar(sql);
            if (codigoNovo < 1)
            {
                throw new PersistenceException(null, "PersistentSupportSQLServer2000.generatePrimaryKey", 
				                               "The primary key generated for object with id " + id_object + ", with size " + size + " and format " + format.ToString() + " is invalid: " + codigoNovo.ToString());
                // closeConnection();
                // return null;
            }

			if (format.Equals(CodeType.STRING_KEY))
            {
                return codigoNovo.ToString().PadLeft(size);
            }

            return codigoNovo.ToString();
        }

        /// <inheritdoc/>
        public override string Backup(string schema, string location = "")
        {
            try
            {
                openConnection();

                IDbCommand c = Connection.CreateCommand();

                string backupsFolder = string.IsNullOrEmpty(location)
                    ? GetDefaultBackupsLocation()
                    : location;

                // Ensure the backups folder exists.
                // TODO: We could probably move the responsibility of creating this folder elsewhere
                // and throw an exception here if it does not exist.
                Directory.CreateDirectory(backupsFolder);

                string fileName = $"{schema}_{DateTime.Now:yyyy_MM_dd_HHmm}.bak",
                    fullPath = Path.Combine(backupsFolder, fileName);

                c.CommandText = "BACKUP DATABASE @databasename TO DISK = @path WITH INIT, COPY_ONLY";
                c.Parameters.Add(new SqlParameter("@databasename", schema));
                c.Parameters.Add(new SqlParameter("@path", fullPath));

                // Last updated by [CJP] at [2016.07.27]
                // Remove timeout from command, in order to complete the database backup
                c.CommandTimeout = 0;

                c.ExecuteNonQuery();

                closeConnection();

                return fullPath;
            }
            catch (Exception e)
            {
                throw new PersistenceException("Erro ao criar o backup.", "PersistentSupportSQLServer2000.Backup", "Error while backing up the database: " + e.Message, e);
            }
        }

        /// <summary>
        /// Drop database
        /// </summary>
        /// <param name="schema">Database name</param>
        public override void Drop(string schema)
        {
            if (string.IsNullOrEmpty(schema))
                throw new ArgumentNullException("schema", "This argument is Mandatory") ;

            try
            {
                //Open a connection to the server
                var connection = GetConnectionToServer();
                connection.Open();
                IDbCommand c = connection.CreateCommand();
                IDbCommand c1 = connection.CreateCommand();

                c.CommandText = "declare @dynsql nvarchar(1000) = N'USE Master ALTER DATABASE ' + QUOTENAME(@databaseName) + N' SET Single_User WITH Rollback Immediate' EXEC(@dynsql)";
                c.Parameters.Add(new SqlParameter("@databasename", schema));
                c.ExecuteNonQuery();

                c1.CommandText = "declare @dynsql nvarchar(1000) = N'USE Master DROP DATABASE ' + QUOTENAME(@databaseName) EXEC(@dynsql)";
                c1.Parameters.Add(new SqlParameter("@databasename", schema));
                c1.ExecuteNonQuery();
                connection.Close();

            }
            catch (Exception e)
            {
                throw new PersistenceException("Error droping the database.", "PersistentSupportSQLServer2000.Drop", "Error while droping the database: " + e.Message, e);
            }
        }

        /// <inheritdoc/>
        public override void Restore(string schema, string path)
        {
            bool targetDbExists = false;

            try
            {
                targetDbExists = CheckIfDatabaseExists(schema);

                openConnection();

                if (targetDbExists)
                    SetDatabaseSingleUserMode(schema);

                ExecuteRestoreCommand(schema, path);
            }
            catch (Exception e)
            {
                throw new PersistenceException("Erro ao restaurar a base de dados.", "PersistentSupportSQLServer2000.Restore", "Error restoring the database: " + e.Message, e);
            }
            finally
            {
                if (targetDbExists)
                    SetDatabaseMultiUserMode(schema);

                closeConnection();
            }
        }

        private void SetDatabaseSingleUserMode(string schema)
        {
            IDbCommand setSingleUserModeCmd = Connection.CreateCommand();
            setSingleUserModeCmd.CommandText = @"
                declare @dynsql nvarchar(1000) = N'USE Master ALTER DATABASE ' + QUOTENAME(@databaseName) + N' SET Single_User WITH Rollback Immediate' 
                EXEC(@dynsql)";
            setSingleUserModeCmd.Parameters.Add(new SqlParameter("@databaseName", schema));
            setSingleUserModeCmd.ExecuteNonQuery();
        }

        private void ExecuteRestoreCommand(string schema, string path)
        {
            IDbCommand cmd = Connection.CreateCommand();
            cmd.CommandText = @"
                USE Master;
                DECLARE @dataFileName NVARCHAR(256);
                DECLARE @logFileName NVARCHAR(256);
                DECLARE @defaultDataPath NVARCHAR(512);
                DECLARE @defaultLogPath NVARCHAR(512);
                DECLARE @filelist TABLE
                (
                    LogicalName NVARCHAR(256),
                    PhysicalName NVARCHAR(512),
                    [Type] VARCHAR(1), 
                    [FileGroupName] VARCHAR(128), 
                    [Size] VARCHAR(128),
                    [MaxSize] VARCHAR(128), 
                    [FileId] VARCHAR(128), 
                    [CreateLSN] VARCHAR(128), 
                    [DropLSN] VARCHAR(128), 
                    [UniqueId] VARCHAR(128), 
                    [ReadOnlyLSN] VARCHAR(128), 
                    [ReadWriteLSN] VARCHAR(128),
                    [BackupSizeInBytes] VARCHAR(128), 
                    [SourceBlockSize] VARCHAR(128), 
                    [FileGroupId] VARCHAR(128), 
                    [LogGroupGUID] VARCHAR(128), 
                    [DifferentialBaseLSN] VARCHAR(128), 
                    [DifferentialBaseGUID] VARCHAR(128), 
                    [IsReadOnly] VARCHAR(128), 
                    [IsPresent] VARCHAR(128), 
                    [TDEThumbprint] VARCHAR(128),
                    [SnapshotUrl] VARCHAR(128)
                );

                INSERT INTO @filelist
                EXEC('RESTORE FILELISTONLY FROM DISK = ''' + @path + '''');

                SELECT @dataFileName = LogicalName FROM @filelist WHERE Type = 'D';
                SELECT @logFileName = LogicalName FROM @filelist WHERE Type = 'L';  

                SELECT @defaultDataPath = CAST(SERVERPROPERTY('InstanceDefaultDataPath') AS NVARCHAR(512));
                SELECT @defaultLogPath = CAST(SERVERPROPERTY('InstanceDefaultLogPath') AS NVARCHAR(512));
                
                DECLARE @restoreQuery NVARCHAR(1000);
                SET @restoreQuery = 
                    'RESTORE DATABASE ' + @databaseName + ' FROM DISK = ''' + @path + '''' +
                    ' WITH REPLACE, MOVE ''' + @dataFileName + ''' TO ''' + @defaultDataPath + '\' + @databaseName + '.mdf'', MOVE ''' +
                    @logFileName + ''' TO ''' + @defaultLogPath + '\' + @databaseName + '.ldf''';

                EXEC sp_executesql @restoreQuery;
            ";
            cmd.Parameters.Add(new SqlParameter("@path", path));
            cmd.Parameters.Add(new SqlParameter("@databaseName", schema));
            cmd.CommandTimeout = 0;
            cmd.ExecuteNonQuery();
        }

        private void SetDatabaseMultiUserMode(string schema)
        {
            IDbCommand setMultiUserModeCmd = Connection.CreateCommand();
            setMultiUserModeCmd.CommandText = @"
                declare @dynsql nvarchar(1000) = N'USE Master ALTER DATABASE ' + QUOTENAME(@databaseName) + N' SET Multi_User' 
                EXEC(@dynsql)";
            setMultiUserModeCmd.Parameters.Add(new SqlParameter("@databaseName", schema));
            setMultiUserModeCmd.ExecuteNonQuery();
        }
		
		/// <summary>
        /// Transfer log data from the system DB to the system log DB
        /// Called from log database PersistentSupport
        /// Uses SQL Bulk Copy (destination database must be SQL Server)
        /// </summary>
        /// <param name="all">True to transfer all log data</param>
        public override void transferMSMQLog(bool all)
        {
            // Get system database PersistentSupport
            PersistentSupport systemSp = PersistentSupport.getPersistentSupport(this.SchemaMapping.Name);
            CriteriaSet filterMQQueues = CriteriaSet.And();
            if (!all && Configuration.MaxLogRowDays > 0)
            {
                DateTime lastDate = DateTime.Today.AddDays(-Configuration.MaxLogRowDays);
                filterMQQueues.LesserOrEqual("PROMQQueues_History", "DATASTATUS", lastDate);
            }

            try
            { 
                // Open connections
                systemSp.openConnection();
                this.openConnection();

                // Open system databas transaction
                systemSp.openTransaction();

                // Open log database transaction
                using (System.Data.SqlClient.SqlTransaction logTransaction = (System.Data.SqlClient.SqlTransaction)this.Connection.BeginTransaction())
                {
                    try
                    {
                        // MQQueues row selection query
                        SelectQuery query = new SelectQuery()
                            .Select("PROMQQueues_History", "CODMQQUEUES", "CODMQQUEUES")
                            .Select("PROMQQueues_History", "QUEUEID", "QUEUEID")
							.Select("PROMQQueues_History", "CHANNELID", "CHANNELID")
                            .Select("PROMQQueues_History", "ANO", "ANO")
                            .Select("PROMQQueues_History", "USERNAME", "USERNAME")
                            .Select("PROMQQueues_History", "TABELA", "TABELA")
                            .Select("PROMQQueues_History", "TABELACOD", "TABELACOD")
                            .Select("PROMQQueues_History", "QUEUEKEY", "QUEUEKEY")
                            .Select("PROMQQueues_History", "QUEUE", "QUEUE")
                            .Select("PROMQQueues_History", "MQSTATUS", "MQSTATUS")
                            .Select("PROMQQueues_History", "DATASTATUS", "DATASTATUS")
                            .Select("PROMQQueues_History", "DATACRIA", "DATACRIA")
                            .Select("PROMQQueues_History", "OPERACAO", "OPERACAO")
                            .Select("PROMQQueues_History", "RESPOSTA", "RESPOSTA")
                            .Select("PROMQQueues_History", "SENDNUMBER", "SENDNUMBER")
                            .Select("PROMQQueues_History", "ZZSTATE", "ZZSTATE")
                            .From("PROMQQueues_History")
                            .Where(filterMQQueues)
                            .OrderBy("PROMQQueues_History", "DATACRIA", Quidgest.Persistence.GenericQuery.SortOrder.Ascending);

                        // Create MQQueues row selection query data reader
                        IDbCommand command = systemSp.GetSelectCommand(query);
                        using (IDataReader dr = command.ExecuteReader())
                        {
                            // Create bulk copy object
                            using (System.Data.SqlClient.SqlBulkCopy bulkCopy = new System.Data.SqlClient.SqlBulkCopy(
                                (System.Data.SqlClient.SqlConnection)this.Connection,
                                System.Data.SqlClient.SqlBulkCopyOptions.KeepIdentity,
                                logTransaction))
                            {
                                bulkCopy.DestinationTableName = "dbo.PROMQQueues_History";
                                bulkCopy.ColumnMappings.Add("CODMQQUEUES", "CODMQQUEUES");
                                bulkCopy.ColumnMappings.Add("QUEUEID", "QUEUEID");
								bulkCopy.ColumnMappings.Add("CHANNELID", "CHANNELID");
                                bulkCopy.ColumnMappings.Add("ANO", "ANO");
                                bulkCopy.ColumnMappings.Add("USERNAME", "USERNAME");
                                bulkCopy.ColumnMappings.Add("TABELA", "TABELA");
                                bulkCopy.ColumnMappings.Add("TABELACOD", "TABELACOD");
                                bulkCopy.ColumnMappings.Add("QUEUEKEY", "QUEUEKEY");
                                bulkCopy.ColumnMappings.Add("QUEUE", "QUEUE");
                                bulkCopy.ColumnMappings.Add("MQSTATUS", "MQSTATUS");
                                bulkCopy.ColumnMappings.Add("DATASTATUS", "DATASTATUS");
                                bulkCopy.ColumnMappings.Add("DATACRIA", "DATACRIA");
                                bulkCopy.ColumnMappings.Add("OPERACAO", "OPERACAO");
                                bulkCopy.ColumnMappings.Add("RESPOSTA", "RESPOSTA");
                                bulkCopy.ColumnMappings.Add("SENDNUMBER", "SENDNUMBER");
                                bulkCopy.ColumnMappings.Add("ZZSTATE", "ZZSTATE");

                                bulkCopy.BulkCopyTimeout = 0;
                                bulkCopy.BatchSize = 10000;
                                bulkCopy.WriteToServer(dr);
                                bulkCopy.Close();
                            }
                        }

                        // Delete log rows on system database
                        DeleteQuery delete = new DeleteQuery()
                            .Delete("PROMQQueues_History")
                            .Where(filterMQQueues);

                        systemSp.Execute(delete);

                        // Commit transaction
                        logTransaction.Commit();
                    }
                    catch (Exception e)
                    {
                        logTransaction.Rollback();
                        systemSp.rollbackTransaction();
                        throw new PersistenceException("Erro durante a transferência de logs.", "PersistentSupportSQLServer2000.transferMSMQLog", "Error transfering MSMQ log data from the database to the log: " + e.Message, e);
                    }

                
                }
                // Close connections
                systemSp.closeTransaction();
                systemSp.closeConnection();
                this.closeConnection();
            }
            catch (Exception e)
            {
                systemSp.rollbackTransaction();
                systemSp.closeConnection();
                this.closeConnection();
                throw new PersistenceException("Erro durante a transferência de logs.", "PersistentSupportSQLServer2000.transferMSMQLog", "Error transfering MSMQ log data from the database to the log: " + e.Message, e);
            }
        }
		
        /// <summary>
        /// Transfer log data from the system DB to the system log DB
        /// Called from log database PersistentSupport
        /// Uses SQL Bulk Copy (destination database must be SQL Server)
        /// </summary>
        /// <param name="all">True to transfer all log data</param>
        /// <param name="job">The transfer log job.</param>
        public override void transferLog(bool all, ExecuteQueryCore.TransferLogOperation job)
        {
            // Get system database PersistentSupport
            PersistentSupport systemSp = PersistentSupport.getPersistentSupport(this.SchemaMapping.Name);

            string tableLogs = "log" + Configuration.Program + "all";

            // Filter rows by date (specified in configuration file)
            CriteriaSet filter = CriteriaSet.And();
            CriteriaSet filterMem = CriteriaSet.And();
            if (!all && Configuration.MaxLogRowDays > 0)
            {
                DateTime lastDate = DateTime.Today.AddDays(-Configuration.MaxLogRowDays);
                filter.LesserOrEqual(tableLogs, "date", lastDate);
                filterMem.LesserOrEqual(CSGenioAmem.FldAltura, lastDate);
            }

            try
            {
                // Open connections
                systemSp.openConnection();
                this.openConnection();

                // Open system database transaction
                systemSp.openTransaction();

                // Open log database transaction
                using (System.Data.SqlClient.SqlTransaction logTransaction = (System.Data.SqlClient.SqlTransaction)this.Connection.BeginTransaction())
                {                    
                    try 
	                {
						int[] totalRows = new[] { 0, 0 };

                        // logGENall Row count
                        SelectQuery query = new SelectQuery()
                            .Select(SqlFunctions.Count("*"), "count")
                            .From(tableLogs)
                            .Where(filter);

                        DataMatrix values = systemSp.Execute(query);
                        if (values.NumRows != 0)
                            totalRows[0] = values.GetInteger(0, 0);

                        // MEM Row count
                        query = new SelectQuery()
                            .Select(SqlFunctions.Count("*"), "count")
                            .From(Area.AreaMEM)
                            .Where(filterMem);

                        values = systemSp.Execute(query);
                        if (values.NumRows != 0)
                            totalRows[1] = values.GetInteger(0, 0);

                        // Total rows to be copied
                        job.Total = totalRows[0] + totalRows[1];
						
                        // ----------------------------------------------
                        // LogGENall transfer
                        // ----------------------------------------------

                        if (totalRows[0] != 0)
                        {
							job.CurrentTable = tableLogs;
							
                            // Log row selection query
                            query = new SelectQuery()
                                .Select(tableLogs, "cod", "COD")
                                .Select(tableLogs, "date", "DATE")
                                .Select(tableLogs, "who", "WHO")
                                .Select(tableLogs, "op", "OP")
                                .Select(tableLogs, "logtable", "LOGTABLE")
                                .Select(tableLogs, "logfield", "LOGFIELD")
                                .Select(tableLogs, "val", "VAL")
                                .From(tableLogs)
                                .Where(filter)
                                .OrderBy(2, Quidgest.Persistence.GenericQuery.SortOrder.Ascending);

                            // Create log row selection query data reader
                            IDbCommand command = systemSp.GetSelectCommand(query);
                            using (IDataReader dr = command.ExecuteReader())
                            {

                                // Create bulk copy object
                                using (System.Data.SqlClient.SqlBulkCopy bulkCopy = new System.Data.SqlClient.SqlBulkCopy(
                                    (System.Data.SqlClient.SqlConnection) this.Connection, 
                                    System.Data.SqlClient.SqlBulkCopyOptions.KeepIdentity,
                                    logTransaction))
                                {
                                    bulkCopy.DestinationTableName = tableLogs;
                                    bulkCopy.ColumnMappings.Add("COD", "COD");
                                    bulkCopy.ColumnMappings.Add("DATE", "DATE");
                                    bulkCopy.ColumnMappings.Add("WHO", "WHO");
                                    bulkCopy.ColumnMappings.Add("OP", "OP");
                                    bulkCopy.ColumnMappings.Add("LOGTABLE", "LOGTABLE");
                                    bulkCopy.ColumnMappings.Add("LOGFIELD", "LOGFIELD");
                                    bulkCopy.ColumnMappings.Add("VAL", "VAL");
									bulkCopy.BulkCopyTimeout = 0;
                                    bulkCopy.BatchSize = 10000;

                                    // Capture the progress of the event
                                    bulkCopy.NotifyAfter = 10000;
                                    bulkCopy.SqlRowsCopied += (sender, eventArgs) =>
                                    {
                                        job.Copied += bulkCopy.NotifyAfter;
									};

                                    // Write from the source to the destination.
                                    bulkCopy.WriteToServer(dr);
									job.Copied = totalRows[0];
                                }
                            }

                            // Delete log rows on system database
                            DeleteQuery delete = new DeleteQuery()
                                .Delete(tableLogs)
                                .Where(filter);

                            systemSp.Execute(delete);
                        }

                        // ----------------------------------------------
                        // MEM transfer
                        // ----------------------------------------------

                        if (totalRows[1] != 0)
                        {
							job.CurrentTable = Configuration.Program + "mem";

                            // MEM row selection query
                            query = new SelectQuery()
								.Select(CSGenioAmem.FldCodmem, "CODMEM")
								.Select(CSGenioAmem.FldLogin, "LOGIN")
								.Select(CSGenioAmem.FldAltura, "ALTURA")
								.Select(CSGenioAmem.FldRotina, "ROTINA")
								.Select(CSGenioAmem.FldObs, "OBS")
								.Select(CSGenioAmem.FldHostid, "HOSTID")
								.Select(CSGenioAmem.FldClientid, "CLIENTID")
								.Select(CSGenioAmem.FldZzstate, "ZZSTATE")
								.From(Area.AreaMEM)
								.Where(filterMem)
								.OrderBy(CSGenioAmem.FldAltura, Quidgest.Persistence.GenericQuery.SortOrder.Ascending);

                            // Create MEM row selection query data reader
                            IDbCommand command = systemSp.GetSelectCommand(query);
                            using (IDataReader dr = command.ExecuteReader())
                            {
                                // Create bulk copy object
                                using (System.Data.SqlClient.SqlBulkCopy bulkCopy = new System.Data.SqlClient.SqlBulkCopy(
                                    (System.Data.SqlClient.SqlConnection)this.Connection,
                                    System.Data.SqlClient.SqlBulkCopyOptions.KeepIdentity,
                                    logTransaction))
                                {
                                    bulkCopy.DestinationTableName = Area.AreaMEM.Table;
                                    bulkCopy.ColumnMappings.Add("CODMEM", "CODMEM");
                                    bulkCopy.ColumnMappings.Add("LOGIN", "LOGIN");
                                    bulkCopy.ColumnMappings.Add("ALTURA", "ALTURA");
                                    bulkCopy.ColumnMappings.Add("ROTINA", "ROTINA");
                                    bulkCopy.ColumnMappings.Add("OBS", "OBS");
                                    bulkCopy.ColumnMappings.Add("HOSTID", "HOSTID");
									bulkCopy.ColumnMappings.Add("CLIENTID", "CLIENTID");
                                    bulkCopy.ColumnMappings.Add("ZZSTATE", "ZZSTATE");
									bulkCopy.BulkCopyTimeout = 0;
                                    bulkCopy.BatchSize = 10000;

                                    // Capture the progress of the event
                                    bulkCopy.NotifyAfter = 10000;
                                    bulkCopy.SqlRowsCopied += (sender, eventArgs) =>
                                    {
                                        job.Copied += bulkCopy.NotifyAfter;
									};

                                    // Write from the source to the destination.
                                    bulkCopy.WriteToServer(dr);
									job.Copied = job.Total;
                                    job.Completed = true;
                                }
                            }

                            // Delete log rows on system database
                            DeleteQuery delete = new DeleteQuery()
                                .Delete(Area.AreaMEM)
                                .Where(filterMem);

                            systemSp.Execute(delete);
                        }
						
                        // Commit transaction
                        logTransaction.Commit();

	                }
					catch (GenioException ex)
					{
						logTransaction.Rollback();
                        systemSp.rollbackTransaction();
						
						job.Completed = true;
						job.ErrorMessage = "Erro durante a transferência de logs: " + ex.UserMessage;
						
						if (ex.ExceptionSite == "PersistentSupportSQLServer2000.transferLog")
							throw;
						if (ex.UserMessage == null)
							throw new PersistenceException("Erro durante a transferência de logs.", "PersistentSupportSQLServer2000.transferLog", "Error transfering log data from the database to the log: " + ex.Message, ex);
						else
							throw new PersistenceException("Erro durante a transferência de logs: " + ex.UserMessage, "PersistentSupportSQLServer2000.transferLog", "Error transfering log data from the database to the log: " + ex.Message, ex);
					}
	                catch (Exception e)
	                {
		                logTransaction.Rollback();
                        systemSp.rollbackTransaction();
						
						job.Completed = true;
                        job.ErrorMessage = "Erro durante a transferência de logs: " + e.Message;
						
		                throw new PersistenceException("Erro durante a transferência de logs.", "PersistentSupportSQLServer2000.transferLog", "Error transfering log data from the database to the log: " + e.Message, e);
	                }
                }

                // Close connections
                systemSp.closeTransaction();
				systemSp.closeConnection();
                this.closeConnection();
            }
            catch (Exception e)
            {
                // Rollback transactions
				// [RC] 06/06/2017 Someone forgot to actually close the transaction?
                systemSp.rollbackTransaction();
                systemSp.closeConnection();
                this.closeConnection();
                throw new PersistenceException("Erro durante a transferência de logs.", "PersistentSupportSQLServer2000.transferLog", "Error transfering log data from the database to the log: " + e.Message, e);
            }
        }           
        
        public override int getRecordPos(User user, string module, IArea area, IList<ColumnSort> sorting, string primaryKeyValue, CriteriaSet conditions, string identifier)
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
                    QueryUtils.increaseQuery(querySelect, queryGenio.SelectFields, queryGenio.FromTable, queryGenio.Joins, queryGenio.WhereConditions, 1, conditions, null, queryGenio.Distinct);
                }

                QueryUtils.setWhereGetPos(querySelect, sorting, area, primaryKeyValue);

                DataMatrix mx = Execute(QueryUtils.buildQueryCount(querySelect));
                return mx.GetInteger(0, 0);
            }
            catch (PersistenceException ex)
            {
                throw new PersistenceException(ex.UserMessage, "PersistentSupportSQLServer2000.getRecordPos",
											   string.Format("Error getting record position - [utilizador] {0}; [modulo] {1}; [area] {2}; [ordenacao] {3}; [valorChavePrimaria] {4}; [condicoes] {5}; [identificador] {6}: ",
											                 user.ToString(), module, area.ToString(), sorting.ToString(), primaryKeyValue, conditions, identifier) + ex.Message, ex);
            }
            catch (Exception ex)
            {
                throw new PersistenceException(null, "PersistentSupportSQLServer2000.getRecordPos",
											   string.Format("Error getting record position - [utilizador] {0}; [modulo] {1}; [area] {2}; [ordenacao] {3}; [valorChavePrimaria] {4}; [condicoes] {5}; [identificador] {6}: ",
											                 user.ToString(), module, area.ToString(), sorting.ToString(), primaryKeyValue, conditions, identifier) + ex.Message, ex);
            }
        }

        /// <summary>
        /// Returns a connection to the server, without the database. Useful for when the database doesn't exist.
        /// </summary>
        public override IDbConnection GetConnectionToServer()
        {
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(Connection.ConnectionString);
            
            if(String.IsNullOrEmpty(builder.Password))
            {
                //After a connection is opened the first time, it's password is cleared.
                //If we have no password either it was opened before (we don't need a new connection), or it was never given, and it will fail either way.
                return this.Connection;
            }

            builder.InitialCatalog = "";
            SqlConnection connection = new SqlConnection(builder.ConnectionString);
            return connection;
        }

        
        /// <summary>
        /// Checks if a database exists
        /// </summary>
        /// <param name="database">The database name</param>
        /// <returns>True if the database exists</returns>
        public override bool CheckIfDatabaseExists(string database)
        {

            if (String.IsNullOrEmpty(database))
                database = this.Connection.Database;

            //If a connection was already opened, we already know that it exists
            if (database == this.Connection.Database && this.Connection.State == ConnectionState.Open)
                return true;

            string query = $"SELECT database_id FROM sys.databases WHERE Name = '{database}'";

            //Open a new one to the server
            var connection = (SqlConnection) GetConnectionToServer();
            connection.Open();
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                object result = command.ExecuteScalar();

                // If result is not null, the database exists
                connection.Close();
                return result != null;
            }
        }
    }
}
