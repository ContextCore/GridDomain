using Akka.Actor;
using Akka.Cluster;
using Akka.Persistence.SqlServer;
using GridDomain.Node.Configuration;

namespace GridDomain.Node
{
    static public class ActorSystemFactory
    {
        public static ActorSystem[] CreateCluster(AkkaConfiguration akkaConf)
        {
            var port = akkaConf.Network.PortNumber;
            var seed  = CreateClusterActorSystem(akkaConf, akkaConf.Network.PortNumber);
            var node1 = CreateClusterActorSystem(akkaConf.Copy(++port), akkaConf.Network.PortNumber);
            var node2 = CreateClusterActorSystem(akkaConf.Copy(++port), akkaConf.Network.PortNumber);
            var node3 = CreateClusterActorSystem(akkaConf.Copy(++port), akkaConf.Network.PortNumber);
            
            return new [] {seed,node1,node2,node3};
        }

        public static ActorSystem CreateActorSystem(AkkaConfiguration akkaConf)
        {

            string logConfig =
@"
      stdout-loglevel = " + akkaConf.LogLevel + @"
      loglevel = " + akkaConf.LogLevel + @"
      log-config-on-start = on";

            string actorConfig = @"   
       actor {
             serializers {
                         wire = ""Akka.Serialization.WireSerializer, Akka.Serialization.Wire""
             }

             serialization-bindings {
                                    ""System.Object"" = wire
             }
             
             provider = ""Akka.Cluster.ClusterActorRefProvider, Akka.Cluster""
             loggers = [""Akka.Logger.NLog.NLogLogger, Akka.Logger.NLog""]
             debug {
                   receive = on
                   autoreceive = on
                   lifecycle = on
                   event-stream = on
                   unhandled = on
             }
       }";

            string remoteConfig = @"
        remote {
               helios.tcp {
                          transport -class = ""Akka.Remote.Transport.Helios.HeliosTcpTransport, Akka.Remote""
                          transport-protocol = tcp
                          port = " + akkaConf.Network.PortNumber + @"
               }
               hostname = " + akkaConf.Network.Name + @"
        }";

            string persistenceJournalConfig = @"
            journal {
                    plugin = ""akka.persistence.journal.sql-server""
                    sql-server {
                               class = ""Akka.Persistence.SqlServer.Journal.SqlServerJournal, Akka.Persistence.SqlServer""
                              # plugin-dispatcher = ""akka.actor.default-dispatcher""
                               connection-string =  """ + akkaConf.Persistence.JournalConnectionString + @"""
                             #  connection-timeout = 30s
                              # schema-name = dbo
                               table-name = """ + akkaConf.Persistence.JournalTableName + @"""
                               auto-initialize = on
                              # timestamp-provider = ""Akka.Persistence.Sql.Common.Journal.DefaultTimestampProvider, Akka.Persistence.Sql.Common""
                               metadata-table-name = """ + akkaConf.Persistence.MetadataTableName + @"""
                    }
            }";

            string persistenceSnapshotStorageConfig = @" 
            snapshot-store {
                           plugin =  ""akka.persistence.snapshot-store.sql-server""
                           sql-server {
                                      class = ""Akka.Persistence.SqlServer.Snapshot.SqlServerSnapshotStore, Akka.Persistence.SqlServer""
                                      plugin-dispatcher = ""akka.actor.default-dispatcher""
                                      connection-string = """+ akkaConf.Persistence.SnapshotConnectionString + @"""
                                      #connection-timeout = 30s
                                      schema-name = dbo
                                      table-name = """ + akkaConf.Persistence.SnapshotTableName + @"""
                                      auto-initialize = on
                           }
            }";

            string akkaPersistenceConfig =
@"      persistence {
                    publish-plugin-commands = on
" + persistenceJournalConfig + @"
" + persistenceSnapshotStorageConfig + @"
        }";

            var hoconConfig = @"akka {  
" + logConfig + @"
" + actorConfig + @"
" + akkaPersistenceConfig + @"
}";

            var actorSystem = ActorSystem.Create(akkaConf.Network.Name, hoconConfig);
            SqlServerPersistence persistence = SqlServerPersistence.Get(actorSystem);
            return actorSystem;
        }


        public static ActorSystem CreateClusterActorSystem(AkkaConfiguration akkaConf, int clusterPort)
        {
            var actorSystem = ActorSystem.Create(akkaConf.Network.Name,
                @"akka {  
                        actor {
                                 provider = ""Akka.Cluster.ClusterActorRefProvider, Akka.Cluster""
                                 loggers = [""Akka.Logger.NLog.NLogLogger, Akka.Logger.NLog""]
                                 debug {
                                          receive = on
                                          autoreceive = on
                                          lifecycle = on
                                          event-stream = on
                                          unhandled = on
                                       }
                        }

                        cluster {
                            seed - nodes = [""akka.tcp://" + akkaConf.Network.Name + "@" + akkaConf.Network.Host + ":" + clusterPort + @"""]
                         
                        stdout-loglevel = " + akkaConf.LogLevel + @"
                        loglevel = " + akkaConf.LogLevel + @"
                        log-config-on-start = on

                        remote {
                                    helios.tcp {
                                        transport-class = ""Akka.Remote.Transport.Helios.HeliosTcpTransport, Akka.Remote""
                                        transport-protocol = tcp
                                        port = 0
                                        hostname = " + akkaConf.Network.Name + @"/
                                    }
                                }
                       ");

            return actorSystem;
        }
    }
}