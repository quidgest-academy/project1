using CSGenio.business;
using CSGenio.business.async;
using CSGenio.framework;
using CSGenio.persistence;
using NUnit.Framework;
using Quidgest.Persistence.GenericQuery;

namespace DbAdmin.IntegrationTest
{
    /// <summary>
    /// All testes in this class assume that:
    /// 1. There are now pending processes in the database
    /// 2. There ins't aren't concurrent calls to the scheduler
    /// </summary>
    public class SchedulerTest 
    {
        private PersistentSupport sp;
        private User _user;


        [SetUp] 
        public void SetUp() 
        {
            PersistenceFactoryExtension.Use();
            sp = PersistentSupport.getPersistentSupport(Configuration.DefaultYear);
            _user = SysConfiguration.CreateWebAdminUser(Configuration.DefaultYear, "TestUser");
            
            //Register any test class here, since they can't be found by the default JobFinder which only searches in GenioServer assembly
            var jobFinder = new TestJobFinder();
            jobFinder.RegisterType(typeof(TestSuccessProcess));
            jobFinder.RegisterType(typeof(TestAsyncProcess));
            SchedulerBroker.SetupBroker(jobFinder);
            
        }

        [Test]
        public void CheckNoJobExists()
        {
            SchedulerBroker scheduler = SchedulerBroker.GetBroker();

            var work = scheduler.GetWork(_user);

            Assert.IsNull(work);            
        }

        [Test]
        public void JobExecutes()
        {
            var job = new TestSuccessProcess();
            var jobId = job.Schedule(sp, _user);

            Assert.That(jobId, Is.Not.Null);
            Assert.That(jobId, Is.Not.Empty);

            SchedulerBroker scheduler = SchedulerBroker.GetBroker();
            GenioWork work = (GenioWork) scheduler.GetWork(_user);
            Assert.IsNotNull(work);
            Assert.AreEqual(ArrayS_prstat.E_AG_3, work.Process.ValRtstatus);            
            Assert.That(jobId, Is.EqualTo(work.Process.QPrimaryKey));

            work.DoWork(_user);
            Assert.AreEqual(ArrayS_prstat.E_T_4, work.Process.ValRtstatus);
        }

        [Test]
        public void JobExecutesAsync()
        {
            var job = new TestAsyncProcess();
            var jobId = job.Schedule(sp, _user);

            Assert.That(jobId, Is.Not.Null);
            Assert.That(jobId, Is.Not.Empty);

            SchedulerBroker scheduler = SchedulerBroker.GetBroker();
            GenioWork work = (GenioWork)scheduler.GetWork(_user);
            Assert.IsNotNull(work);
            Assert.AreEqual(ArrayS_prstat.E_AG_3, work.Process.ValRtstatus);
            Assert.That(jobId, Is.EqualTo(work.Process.QPrimaryKey));

            work.DoWork(_user);
            Assert.AreEqual(ArrayS_prstat.E_T_4, work.Process.ValRtstatus);
        }
    }
}
