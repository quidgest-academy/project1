using NUnit.Framework;

using CSGenio.persistence;
using GenioServer.security;
using CSGenio.framework;
using CSGenio.business;
using Quidgest.Persistence.GenericQuery;

namespace WebTest
{    
    /// <summary>
    ///This is a test class for Test and is intended
    ///to contain all Test Unit Tests
    ///</summary>
    [SetUpFixture]
    public class TestNegocio
    {
	    [OneTimeSetUp]
        public static void AssemblyInit()
        {
            // Initalization code goes here
            PersistenceFactoryExtension.Use();
            CSGenio.persistence.PersistentSupport.SetControlQueries(
                GenioServer.persistence.PersistentSupportExtra.ControlQueries, 
                GenioServer.persistence.PersistentSupportExtra.ControlQueriesOverride);
            GenioServer.framework.OverrideQueryDeclaring.Use();
            //Dependency injection
            UserFactory.BusinessManager = new UserBusinessService();

        }

		// USE /[MANUAL PRO TESTNEGOCIO]/
    }
}
