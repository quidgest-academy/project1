using CSGenio;
using CSGenio.framework;
using GenioServer.framework;
using NUnit.Framework;
using CSGenio.config;

namespace DbAdmin.IntegrationTest
{
    public class ConfigurationXmlFile
    {
        private static string workspace = Path.Combine(Path.GetTempPath(), "IntegrationTests");

        [SetUp]
        public void Setup()
        {
            if (Directory.Exists(workspace))
            {
                Directory.Delete(workspace, recursive: true);
            }
            Directory.CreateDirectory(workspace);
        }


        [Test]
        public void CheckIfNotExists()
        {
            FileConfigurationManager manager = new FileConfigurationManager("C:\\invalidPath");

            bool result = manager.Exists();

            Assert.IsFalse(result);

        }

        [Test]
        public void CreateNewConfigFile()
        {
            var workingSpaceDir = Path.Combine(workspace, "newFile");
            Directory.CreateDirectory(workingSpaceDir);
            FileConfigurationManager manager = new FileConfigurationManager(workingSpaceDir);

            manager.CreateNewConfig();

            string destination = Path.Combine(workingSpaceDir, "Configuracoes.xml");
            Assert.IsTrue(File.Exists(destination));

            var readConfig = manager.GetExistingConfig();
            Assert.AreEqual(ConfigXMLMigration.CurConfigurationVerion.ToString(), readConfig.ConfigVersion);
        }

        [Test]
        public void CreateOverExistingConfig()
        {
            var workingSpaceDir = Path.Combine(workspace, "createExisting");
            Directory.CreateDirectory(workingSpaceDir);
            FileConfigurationManager manager = new FileConfigurationManager(workingSpaceDir);
            manager.CreateNewConfig();

            //Act and Assert
             Assert.Throws<FrameworkException>(()=>
                manager.CreateNewConfig()
             );

        }

        [Test]
        public void StoreConfig()
        {
            //Arrange
            var workingSpaceDir = Path.Combine(workspace, "storeFile");
            Directory.CreateDirectory(workingSpaceDir);
            FileConfigurationManager manager = new FileConfigurationManager(workingSpaceDir);
            var config = manager.CreateNewConfig();

            //Act
            config.ConfigVersion = "3";
            manager.StoreConfig(config);

            //Assert
            string destination = Path.Combine(workingSpaceDir, "Configuracoes.xml");
            Assert.IsTrue(File.Exists(destination));
            var readConfig = manager.GetExistingConfig();      
            Assert.AreEqual("3", readConfig.ConfigVersion);
        }




    }
}
