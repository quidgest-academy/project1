using CommandLine;
using CSGenio.persistence;
using CSGenio.config;
using DbAdmin;
using GenioServer.security;
using System;

namespace AdminCLI
{
    static partial class AdminCLI
    {
        private static DBMaintenance dBMaintenance;
        private static SysConfiguration sysConfiguration;

        static void Main(string[] args)
        {
            //Starting procedure for SP
            Init();
            
            //Parse console arguments
            try
            {
                var parsedArgs = CommandLine.Parser.Default.ParseArguments<ReindexOptions, ListReindexScriptsOptions, WriteConfigurationOptions, 
                    ReadConfigurationOptions, BackupOptions, RestoreOptions, RemoveBackupOptions>(args);

                parsedArgs.MapResult(
                    (ReindexOptions opts) => Reindex(opts),
                    (ListReindexScriptsOptions opts) => ListReindexScripts(opts),
                    (WriteConfigurationOptions opts) => WriteConfiguration(opts),
                    (ReadConfigurationOptions opts) => ReadConfiguration(opts),
                    (BackupOptions opts) => Backup(opts),
                    (RestoreOptions opts) => Restore(opts),
                    (RemoveBackupOptions opts) => RemoveBackup(opts),
                    errs => 1);
            }
            catch (Exception e) {
                Console.WriteLine("The following error has ocurred: " + e.Message);
            }
        }

        private static void Init()
        {
            // Inject SP data 
            PersistenceFactoryExtension.Use();
            PersistentSupport.SetControlQueries(
                GenioServer.persistence.PersistentSupportExtra.ControlQueries,
                GenioServer.persistence.PersistentSupportExtra.ControlQueriesOverride);
            
            GenioServer.framework.OverrideQueryDeclaring.Use();
            
            //Dependency injection
            UserFactory.BusinessManager = new UserBusinessService();

            //Initialize library classes
            dBMaintenance = new DBMaintenance(AppDomain.CurrentDomain.BaseDirectory);
            var fileConfigManager = new FileConfigurationManager(CSGenio.framework.Configuration.GetConfigPath());
            sysConfiguration = new SysConfiguration(fileConfigManager);
        }
    }
}