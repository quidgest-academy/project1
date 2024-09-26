using CSGenio.framework;
using CSGenio;
using System.Text;
using GenioServer.framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using CSGenio.business;
using System.Globalization;
using CSGenio.config;

namespace DbAdmin
{
    /*
    *   SysConfiguration holds all the library methods related to the database configuration management.
    *   Operations like reading and writing configs from configuracoes.xml
    */
    public class SysConfiguration(IConfigurationManager configManager)
    {
        public void SaveDatabaseConfig(string username, string password, string server, string serverType, string schema, string port = "", bool encryptConn = false, bool withDomainUser = false, string year = "")
        {
            if (!configManager.Exists())
                configManager.CreateNewConfig();

            var conf = configManager.GetExistingConfig();
            
            if(string.IsNullOrEmpty(year))
                year = Configuration.DefaultYear;

            DataSystemXml db = new DataSystemXml
            {
                Login = Convert.ToBase64String(Encoding.Unicode.GetBytes(username)),
                Password = Convert.ToBase64String(Encoding.Unicode.GetBytes(password)),

                Server = server,
                Port = port,
                Type = serverType,
                Name = year
            };

            DataXml datasystem = new DataXml
            {
                Id = Configuration.Program,
                Schema = schema,
                ConnEncrypt = encryptConn,
                ConnWithDomainUser = withDomainUser
            };

            db.Schemas = new List<DataXml>
            {
                datasystem,
            };

            //If there is a log database already configured, keep it
            if (conf.DataSystems.Count > 0 && conf.DataSystems[0].DataSystemLog != null)
                db.DataSystemLog = conf.DataSystems[0].DataSystemLog;
            
            int indexDS = conf.DataSystems.FindIndex(confDS => confDS.Name == db.Name);
            if (indexDS != -1)
                conf.DataSystems[indexDS] = db;
            else
                conf.DataSystems.Add(db);
            

            // In case the default Qyear changes, reorder databases
            if (conf.anoDefault != Configuration.DefaultYear)
            {
                List<DataSystemXml> tempDataSystems = new List<DataSystemXml>();
                DataSystemXml tempDefDS = conf.DataSystems.FirstOrDefault(ds => ds.Name == Configuration.DefaultYear);
                if (tempDefDS != null)
                    tempDataSystems.Add(tempDefDS);
                tempDataSystems.AddRange(conf.DataSystems.Where(ds => ds.Name != Configuration.DefaultYear));
                conf.DataSystems = tempDataSystems;
            }

            conf.anoDefault = Configuration.DefaultYear;

            UserCfgEl user = new UserCfgEl();
            user.Name = "guest";
            user.Type = UserType.Guest;
            user.AutoLogin = true;

            if (conf.MessageQueueing != null && conf.MessageQueueing.Queues.Count == 0)
            {
                Queue queue = new Queue
                {
                    queue = "queue",
                    path = "/path/path",
                    Qyear = "ano",
                    Unicode = false,
                    UsesMsmq = false
                };
                conf.MessageQueueing.Queues.Add(queue);
            }

            if (conf.Elasticsearch.Colours.Count == 0)
            {
                CoreXml core = new CoreXml
                {
                    Index = "index_name",
                    Name = "id",
                    Area = "AREA",
                    Urlfscrawler = "http://localhost:8080/fscrawler",
                    Url = "http://localhost:9200",
                    Username = "myusername",
                    Password = ""
                };
                conf.Elasticsearch = new ElasticsearchXml
                {
                    Colours = new List<CoreXml>()
                };
                conf.Elasticsearch.Colours.Add(core);
            }

            //Set version
            if(string.IsNullOrEmpty(conf.ConfigVersion))
                conf.ConfigVersion = ConfigXMLMigration.CurConfigurationVerion.ToString();

            //Save configuration
            configManager.StoreConfig(conf);

            // Reload Configuration static instance in server with the new Configuracoes.xml data
            Configuration.ReadConfiguration(conf);
        }

        public void SaveLogDatabaseConfig(string username, string password, string server, string serverType, string schema, string port = "", bool encryptConn = false, bool withDomainUser = false, string year = "")
        {
            if (!configManager.Exists())
                configManager.CreateNewConfig();

            var conf = configManager.GetExistingConfig();

            if (string.IsNullOrEmpty(year))
                year = Configuration.DefaultYear;

            //Get DataSystemXml that corresponds to the year we want
            DataSystemXml db = ReadDatabaseConfig(year);

            if (db == null)
                throw new BusinessException("No DataSystemXml with that year was found, please make sure there is a database configured with this year first!", "SysConfiguration.SaveLogDatabaseConfig",
                    "No DataSystemXml with that year was found, please make sure there is a database configured with this year first!");

            db.DataSystemLog = new DataSystemXml()
            {
                Schemas = new List<DataXml>{
                    new DataXml {
                        Id = Configuration.Program,
                        Schema = schema,
                        ConnEncrypt = encryptConn,
                        ConnWithDomainUser = withDomainUser
                    }
                },
                Login = Convert.ToBase64String(Encoding.Unicode.GetBytes(username)),
                Password = Convert.ToBase64String(Encoding.Unicode.GetBytes(password)),
                Server = server,
                Port = port,
                Type = serverType,
                Name = year
            };

            int indexDS = conf.DataSystems.FindIndex(confDS => confDS.Name == db.Name);
            if (indexDS != -1)
                conf.DataSystems[indexDS] = db;
            else
                conf.DataSystems.Add(db);

            //Set version
            if(string.IsNullOrEmpty(conf.ConfigVersion))
                conf.ConfigVersion = ConfigXMLMigration.CurConfigurationVerion.ToString();

            //Save configuration
            configManager.StoreConfig(conf);

            // Reload configuration
            Configuration.ReadConfiguration(conf);
        }

        public DataSystemXml ReadDatabaseConfig(string year = "", string schema = "")
        {
            ConfigurationXML conf = configManager.GetExistingConfig();            

            foreach(DataSystemXml dataSystem in conf.DataSystems)
            {
                //If there is a year and it does not match, continue
                if(!string.IsNullOrEmpty(year) && dataSystem.Name != year)
                    continue;
                
                //If there is a schema and it does not match it, continue as well
                if(!string.IsNullOrEmpty(schema))
                {
                    bool matchesSchema = false;
                    foreach(DataXml dataSchema in dataSystem.Schemas)
                    {
                        if (dataSchema.Schema == schema)
                            matchesSchema = true;
                    }

                    if (!matchesSchema)
                        continue;
                }
                return dataSystem;
            }
            return null;
        }

		//module parameter set to default as the system alias 
        public static User CreateWebAdminUser(string year = null, string userName = "WebAdmin", string module = "PRO")
        {
            User user = new User(userName, "", "");
			user.Year = year ?? Configuration.DefaultYear;
            user.AddModuleRole(module,  Role.ADMINISTRATION);
            user.CurrentModule = module;
            user.Language = CultureInfo.CurrentCulture.Name.Replace("-", "").ToUpper();
            return user;
        }
    }
}