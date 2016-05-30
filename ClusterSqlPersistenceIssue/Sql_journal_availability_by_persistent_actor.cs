using System;
using Akka.Actor;
using Akka.TestKit.NUnit;
using NUnit.Framework;

namespace ClusterSqlPersistenceIssue
{
    [TestFixture]
    public class Sql_journal_availability_by_persistent_actor : TestKit
    {
        private static readonly string _standalone_configuration = @"
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

        [Test]
        public void Sql_journal_is_available_for_akka_cluster_config()
        {
            
            string conf = @"akka {

      stdout-loglevel = WARNING
      loglevel = WARNING
      log-config-on-start = on
   
       actor {
             serializers {
                         wire = ""Akka.Serialization.WireSerializer, Akka.Serialization.Wire""
             }

        serialization-bindings {
                                    ""System.Object"" = wire
    }

    loggers = [""Akka.Logger.NLog.NLogLogger, Akka.Logger.NLog""]
    debug {
                   receive = on
                   autoreceive = on
                   lifecycle = on
                   event-stream = on
                   unhandled = on
    }

}

actor.provider = ""Akka.Cluster.ClusterActorRefProvider, Akka.Cluster""
            cluster {
                            seed-nodes = [""akka.tcp://LocalSystem@127.0.0.1:8080""]
            }
        remote {
               #cluster.sharding.journal-plugin-id = akka.persistence.journal.sql-server
               #cluster.sharding.snapshot-plugin-id = akka.persistence.snapshot-store.sql-server
          
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
                    sql -server
    {
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
            #shard-journal {
            #            plugin = ""akka.persistence.shard-journal.sql-server""
            #            sql-server {
            #                 class = ""Akka.Persistence.SqlServer.Journal.SqlServerJournal, Akka.Persistence.SqlServer""
            #                   connection-string =  ""Data Source=(localdb)\\v11.0;Database=AutoTestAkka;Integrated Security = true""
            #                   schema-name = dbo
            #                   auto-initialize = on
            #            }
            #}
 
            snapshot -store {
                           plugin =  ""akka.persistence.snapshot-store.sql-server""
                           sql -server {
                                      class = ""Akka.Persistence.SqlServer.Snapshot.SqlServerSnapshotStore, Akka.Persistence.SqlServer""
                                      plugin-dispatcher = ""akka.actor.default-dispatcher""
                                      connection-string = ""Data Source=(localdb)\\v11.0;Database=AutoTestAkka;Integrated Security = true""
                                      connection-timeout = 30s
                                      schema-name = dbo
                                      table-name = ""Snapshots""
                                      auto -initialize = on
                           }
            }
        }
}";
            var actorSystem = ActorSystem.Create("test", conf);
            var actor = actorSystem.ActorOf(Props.Create<SqlJournalPingActor>(TestActor));
            CHeckPersist(actor);
        }

        [Test]
        public void Sql_journal_is_available_for_akka_standalone_config()
        {
            var actorSystem = ActorSystem.Create("test1", _standalone_configuration);
            //  var plugin = Akka.Persistence.Persistence.Instance.Apply(actorSystem).JournalFor(null);
            // plugin.Ask(new object());
            var actor = actorSystem.ActorOf(Props.Create<SqlJournalPingActor>(TestActor));
            CHeckPersist(actor);
        }


        [Test]
        public void Sql_journal_is_available_for_test_akka_standalone_config()
        {
           // var actorSystem = Sys.Create("test1", _standalone_configuration);
            //  var plugin = Akka.Persistence.Persistence.Instance.Apply(actorSystem).JournalFor(null);
            // plugin.Ask(new object());
            var actor = Sys.ActorOf(Props.Create<SqlJournalPingActor>(TestActor));
            CHeckPersist(actor);
        }

        private void CHeckPersist(IActorRef actor)
        {

            var sqlJournalPing = new SqlJournalPing() { Payload = "testPayload" };
            actor.Ask(sqlJournalPing);

            //EventFilter.Info("indexing users").ExpectOne(() =>
            //{
            //    _identity.Tell(new UserIdentityActor.IndexUsers());
            //});

            ExpectMsg<Persisted>(m => m.Payload == sqlJournalPing.Payload, TimeSpan.FromSeconds(3));
        }
    }
}