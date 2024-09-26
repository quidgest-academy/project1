using CSGenio.business;
using CSGenio.framework;
using CSGenio.framework.TableConfiguration;
using CSGenio.persistence;
using Quidgest.Persistence.GenericQuery;
using System.Text.Json;
using System.Text.Json.Serialization;
using System;
using System.Linq;

namespace CSGenio.core.persistence
{

    /// <summary>
    /// Class that gets and sets table configuration information in the database
    /// </summary>
    public static class TableConfigurationIO
    {
        /*
		 * Parse string-encoded table configuration data to an object.
		 */
        public static TableConfiguration ParseTableConfigData(string encodedString)
        {
            // Set options to allow converting numbers to strings (used in advanced filters, column filters, searchbar filters)
            JsonSerializerOptions serializationOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                NumberHandling = JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString
            };

            TableConfiguration tableConfiguration;

            try
            {
                tableConfiguration = JsonSerializer.Deserialize<TableConfiguration>(encodedString, serializationOptions);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                tableConfiguration = new TableConfiguration();
            }

            return tableConfiguration;
        }

        /*
		 * Get a table configuration record from the database.
		 */
        public static CSGenioAtblcfg GetTableConfigPKRecord(PersistentSupport sp, User user, string uuid, string configPK)
        {
            //Get saved configuration
            return CSGenioAtblcfg.searchList(sp, user, CriteriaSet.And()
                .Equal(CSGenioAtblcfg.FldCodpsw, user.Codpsw)
                .Equal(CSGenioAtblcfg.FldUuid, uuid)
                .Equal(CSGenioAtblcfg.FldCodtblcfg, configPK)
                .Equal(CSGenioAtblcfg.FldZzstate, 0))
                .FirstOrDefault();
        }

        /*
		 * Get a table configuration record from the database.
		 */
        public static CSGenioAtblcfg GetTableConfigNameRecord(PersistentSupport sp, User user, string uuid, string configName)
        {
            //Get saved configuration
            return CSGenioAtblcfg.searchList(sp, user, CriteriaSet.And()
                .Equal(CSGenioAtblcfg.FldCodpsw, user.Codpsw)
                .Equal(CSGenioAtblcfg.FldUuid, uuid)
                .Equal(CSGenioAtblcfg.FldName, configName)
                .Equal(CSGenioAtblcfg.FldZzstate, 0))
                .FirstOrDefault();
        }

        /*
		 * Get a table configuration from the database.
		 */
        public static TableConfiguration GetTableConfig(PersistentSupport sp, User user, string uuid, string configName)
        {
            // Get record from the database
            CSGenioAtblcfg configRecord = GetTableConfigNameRecord(sp, user, uuid, configName);

            // If configuration does not exist
            if (configRecord == null)
                return null;

            // Parse to object
            TableConfiguration tableConfig = ParseTableConfigData(configRecord.ValConfig);

            // Add configuration name
            tableConfig.Name = configRecord.ValName;

            return tableConfig;
        }

        /*
		 * Get default table configuration from the database.
		 */
        public static TableConfiguration GetTableDefaultConfig(PersistentSupport sp, User user, string uuid)
        {
            //Get record of what view is the default
            CSGenioAtblcfgsel userTableConfigSelectedInfo = CSGenioAtblcfgsel.searchList(sp, user, CriteriaSet.And()
                .Equal(CSGenioAtblcfgsel.FldCodpsw, user.Codpsw)
                .Equal(CSGenioAtblcfgsel.FldUuid, uuid)
                .Equal(CSGenioAtblcfgsel.FldZzstate, 0))
                .FirstOrDefault();

            //If record doesn't exist
            if (userTableConfigSelectedInfo == null)
                return null;

            // Get record from the database
            CSGenioAtblcfg configRecord = GetTableConfigPKRecord(sp, user, uuid, userTableConfigSelectedInfo.ValCodtblcfg);

            // If configuration does not exist
            if (configRecord == null)
                return null;

            // Parse to object
            TableConfiguration tableConfig = ParseTableConfigData(configRecord.ValConfig);

            // Add configuration name
            tableConfig.Name = configRecord.ValName;

            return tableConfig;
        }

        /*
		 * Determine which table configuration to use.
		 */
        public static TableConfiguration DetermineTableConfig(PersistentSupport sp, User user, string uuid, TableConfiguration currentTableConfig, string configName = "", bool loadDefaultView = false)
        {
            // Default to the current table configuration
            TableConfiguration tableConfig = currentTableConfig;

            // If loading the default configuration
            if (!string.IsNullOrEmpty(uuid) && loadDefaultView)
                tableConfig = GetTableDefaultConfig(sp, user, uuid);
            // If loading a saved table configuration
            else if (!string.IsNullOrEmpty(uuid) && !string.IsNullOrEmpty(configName))
                tableConfig = GetTableConfig(sp, user, uuid, configName);

            if (tableConfig == null)
                tableConfig = new TableConfiguration();

            return tableConfig;
        }
    }
}
