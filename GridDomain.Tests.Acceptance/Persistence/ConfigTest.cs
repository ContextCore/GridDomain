using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Persistence.SqlServer;
using GridDomain.Node;
using NUnit.Framework;

namespace GridDomain.Tests.Acceptance.Persistence
{
    [TestFixture]
    public class SqlPersistenceConfigTest
    {
        private AutoTestAkkaConfiguration _autoTestAkkaConfiguration;
        private SqlServerPersistence _sqlPersistence;

        [SetUp]
        public void Given_system_with_sql_persistence()
        {
            _autoTestAkkaConfiguration = new AutoTestAkkaConfiguration();
            var  akka = ActorSystemFactory.CreateActorSystem(_autoTestAkkaConfiguration);
            _sqlPersistence = SqlServerPersistence.Get(akka);
        }

        [Test]
        public void Then_jornal_table_should_be_configured()
        { 
            Assert.AreEqual(_autoTestAkkaConfiguration.Persistence.JournalTableName, _sqlPersistence.JournalSettings.TableName);
        }

        [Test]
        public void Then_jornal_connection_string_should_be_configured()
        {
            Assert.AreEqual(_autoTestAkkaConfiguration.Persistence.JournalConnectionString, _sqlPersistence.JournalSettings.ConnectionString);
        }

        [Test]
        public void Then_snapshot_table_should_be_configured()
        {
            Assert.AreEqual(_autoTestAkkaConfiguration.Persistence.SnapshotTableName, _sqlPersistence.SnapshotSettings.TableName);
        }

        [Test]
        public void Then_snapshot_connection_string_should_be_configured()
        {
            Assert.AreEqual(_autoTestAkkaConfiguration.Persistence.SnapshotConnectionString, _sqlPersistence.SnapshotSettings.ConnectionString);
        }

    }
}
