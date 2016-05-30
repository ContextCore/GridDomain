using System;
using Akka.Actor;
using Akka.Persistence.SqlServer;
using Akka.TestKit.NUnit;
using NUnit.Framework;

namespace ClusterSqlPersistenceIssue
{
    [TestFixture]
    public class Sql_journal_availability_for_persistent_actor_for_test_akka_system : TestKit
    {
        private static readonly string StandaloneConfiguration = @"
akka {

        stdout-loglevel = WARNING
        loglevel = WARNING
        log-config-on-start = on
        debug {
                       receive = on
                       autoreceive = on
                       lifecycle = on
                       event-stream = on
                       unhandled = on
        }

        actor.provider = ""Akka.Remote.RemoteActorRefProvider, Akka.Remote""
          
        remote {
                    helios.tcp {
                               transport-class = ""Akka.Remote.Transport.Helios.HeliosTcpTransport, Akka.Remote""
                               transport-protocol = tcp
                               port = 8080
                    }
                    hostname = LocalSystem
            }

      persistence {
                    publish-plugin-commands = on

                    journal
                    {
                          plugin = ""akka.persistence.journal.sql-server""
                          sql-server {
                                     class = ""Akka.Persistence.SqlServer.Journal.SqlServerJournal, Akka.Persistence.SqlServer""
                                     plugin-dispatcher = ""akka.actor.default-dispatcher""
                                     connection-string =  ""Data Source=(localdb)\\v11.0;Database=AutoTestAkka;Integrated Security = true""
                                     connection-timeout = 30s
                                     schema-name = dbo
                                     table-name = ""Journal""
                                     auto-initialize = on
                                     timestamp-provider = ""Akka.Persistence.Sql.Common.Journal.DefaultTimestampProvider, Akka.Persistence.Sql.Common""
                                     metadata-table-name = ""Metadata""
                          }
                     }        

                     snapshot-store {
                                    plugin =  ""akka.persistence.snapshot-store.sql-server""
                                    sql-server {
                                                class = ""Akka.Persistence.SqlServer.Snapshot.SqlServerSnapshotStore, Akka.Persistence.SqlServer""
                                                plugin-dispatcher = ""akka.actor.default-dispatcher""
                                                connection-string = ""Data Source=(localdb)\\v11.0;Database=AutoTestAkka;Integrated Security = true""
                                                connection-timeout = 30s
                                                schema-name = dbo
                                                table-name = ""Snapshots""
                                                auto-initialize = on
                                    }
                     }
        }
}";
        private SqlServerPersistence _sqlServerPersistence;

        public Sql_journal_availability_for_persistent_actor_for_test_akka_system():base(StandaloneConfiguration)
        {
            
        }
        [SetUp]
        public void Given_system_with_sql_persistence()
        {
            _sqlServerPersistence = SqlServerPersistence.Get(Sys);
        }

        [Test]
        public void Then_jornal_table_should_be_configured()
        {
            Assert.AreEqual("Journal", _sqlServerPersistence.JournalSettings.TableName);
        }

        [Test]
        public void Then_jornal_connection_string_should_be_configured()
        {
            Assert.AreEqual("Data Source=(localdb)\\v11.0;Database=AutoTestAkka;Integrated Security = true", _sqlServerPersistence.JournalSettings.ConnectionString);
        }

        [Test]
        public void Then_snapshot_table_should_be_configured()
        {
            Assert.AreEqual("Snapshots", _sqlServerPersistence.SnapshotSettings.TableName);
        }

        [Test]
        public void Then_snapshot_connection_string_should_be_configured()
        {
            Assert.AreEqual("Data Source=(localdb)\\v11.0;Database=AutoTestAkka;Integrated Security = true", _sqlServerPersistence.SnapshotSettings.ConnectionString);
        }


        [Test]
        public void Sql_journal_is_available_for_test_akka_standalone_config()
        {
            var actor = Sys.ActorOf(Props.Create<SqlJournalPingActor>(TestActor));
            CHeckPersist(actor);
        }

        private void CHeckPersist(IActorRef actor)
        {

            var sqlJournalPing = new SqlJournalPing() { Payload = "testPayload" };
            actor.Ask(sqlJournalPing);
            ExpectMsg<Persisted>(m => m.Payload == sqlJournalPing.Payload, TimeSpan.FromSeconds(5));
        }
    }
}