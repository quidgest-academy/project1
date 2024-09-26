using CommandLine;
using DbAdmin;
using System;
using System.IO;

namespace AdminCLI
{
    [Verb("backup", HelpText = "Performs a backup of a database")]
    class BackupOptions
    {
        [Option('u', "username", Required = true, HelpText = "Database instance username")]
        public string Username { get; set; }

        [Option('p', "password", Required = true, HelpText = "Database instance password")]
        public string Password { get; set; }

        [Option("year", Required = true, HelpText = "Database year")]
        public string Year { get; set; }

        [Option("location", HelpText = "Folder where the backup is meant to be saved")]
        public string Location { get; set; }
    }

    [Verb("restore", HelpText = "Restores a database backup")]
    class RestoreOptions
    {
        [Option('u', "username", Required = true, HelpText = "Database instance username")]
        public string Username { get; set; }

        [Option('p', "password", Required = true, HelpText = "Database instance password")]
        public string Password { get; set; }

        [Option("year", Required = true, HelpText = "Database year")]
        public string Year { get; set; }

        [Option("file", Required = true, HelpText = "Database backup file name")]
        public string Filename { get; set; }

        [Option("location", HelpText = "Backup save folder location")]
        public string Location { get; set; }
    }

    [Verb("delete-backup", HelpText = "Removes a database backup")]
    class RemoveBackupOptions
    {
        [Option("path", Required = true, HelpText = "Backup save path")]
        public string Path { get; set; }
    }

    partial class AdminCLI
    {
        /// <summary>
        /// Backsups a database
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        private static int Backup(BackupOptions options)
        {
            try
            {
                string filepath = DBMaintenance.BackupDatabase(options.Year, options.Username, options.Password, options.Location);
                Console.WriteLine("File backed up successfully: " + filepath);
                
                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine("The following error has ocurred while backing up the database: \n" + ex.Message);
                return 1;
            }
        }

        /// <summary>
        /// Restores a database
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        private static int Restore(RestoreOptions options)
        {
            try
            {
                string location = options.Location;
                if (string.IsNullOrWhiteSpace(location))
                    location = Path.Combine(Environment.CurrentDirectory, "dbs", "backup");

                DBMaintenance.RestoreDatabase(options.Year, options.Username, options.Password, location, options.Filename);
                Console.WriteLine("The desired database backup was restored successfully.");
            }
            catch(Exception e) 
            {
                Console.WriteLine("The following error has ocurred while restoring the database: \n" + e.Message);
                return 1;
            }

            return 0;
        }

        /// <summary>
        /// Removes a database backup
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        private static int RemoveBackup(RemoveBackupOptions options)
        {
            try
            {
                DBMaintenance.DeleteBackup(options.Path);
                Console.WriteLine("The desired database backup was deleted successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("The following error has ocurred while removing the database backup: \n" + ex.Message);
                return 1;
            }
            
            return 0;
        }
    }
}
