using Akka.Persistence.SqlServer;
using GridDomain.Node;
using NUnit.Framework;

namespace GridDomain.Tests.Acceptance.Persistence
{
    [TestFixture]
    public class SqlPersistenceConfigurationApplyTest
    {
        [SetUp]
        public void Given_system_with_sql_persistence()
        {
            _autoTestAkkaConfiguration = new AutoTestAkkaConfiguration();
            var akka = ActorSystemFactory.CreateActorSystem(_autoTestAkkaConfiguration);
            _sqlPersistence = SqlServerPersistence.Get(akka);
        }

        private AutoTestAkkaConfiguration _autoTestAkkaConfiguration;
        private SqlServerPersistence _sqlPersistence;

        [Test]
        public void Then_jornal_connection_string_should_be_configured()
        {
            Assert.AreEqual(_autoTestAkkaConfiguration.Persistence.JournalConnectionString.Replace(@"\\", @"\"),
                _sqlPersistence.JournalSettings.ConnectionString);
        }

        [Test]
        public void Then_jornal_table_should_be_configured()
        {
            Assert.AreEqual(_autoTestAkkaConfiguration.Persistence.JournalTableName,
                _sqlPersistence.JournalSettings.TableName);
        }

        [Test]
        public void Then_snapshot_connection_string_should_be_configured()
        {
            Assert.AreEqual(_autoTestAkkaConfiguration.Persistence.SnapshotConnectionString.Replace(@"\\", @"\"),
                _sqlPersistence.SnapshotSettings.ConnectionString);
        }

        [Test]
        public void Then_snapshot_table_should_be_configured()
        {
            Assert.AreEqual(_autoTestAkkaConfiguration.Persistence.SnapshotTableName,
                _sqlPersistence.SnapshotSettings.TableName);
        }
    }
}