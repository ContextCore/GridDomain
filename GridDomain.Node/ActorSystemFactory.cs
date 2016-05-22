using Akka.Actor;
using Akka.Cluster;
using GridDomain.Node.Configuration;

namespace GridDomain.Node
{
    static public class ActorSystemFactory
    {
        public static ActorSystem[] CreateCluster(AkkaConfiguration akkaConf)
        {
            var port = akkaConf.Port;
            var seed  = CreateClusterActorSystem(akkaConf, akkaConf.Port);
            var node1 = CreateClusterActorSystem(akkaConf.Copy(++port), akkaConf.Port);
            var node2 = CreateClusterActorSystem(akkaConf.Copy(++port), akkaConf.Port);
            var node3 = CreateClusterActorSystem(akkaConf.Copy(++port), akkaConf.Port);
            
            return new [] {seed,node1,node2,node3};
        }

        public static ActorSystem CreateActorSystem(AkkaConfiguration akkaConf)
        {
            var hoconConfig = @"akka {  
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
                        stdout-loglevel = " + akkaConf.LogLevel + @"
                        loglevel = " + akkaConf.LogLevel + @"
                        log-config-on-start = on

                        remote {
                                    helios.tcp {
                                        transport-class = ""Akka.Remote.Transport.Helios.HeliosTcpTransport, Akka.Remote""
                                        transport-protocol = tcp
                                        port = " + akkaConf.Port + @"}
                                        hostname = " + akkaConf.Name + @"/
                                       
                                    }
                                }

                        akka.persistence{

                        journal {
                            sql-server {

                                # qualified type name of the SQL Server persistence journal actor
                                class = ""Akka.Persistence.SqlServer.Journal.SqlServerJournal, Akka.Persistence.SqlServer""

                                # dispatcher used to drive journal actor
                                plugin-dispatcher = ""akka.actor.default-dispatcher""

                                # connection string used for database access
                                connection-string = """+ akkaConf.JournalConnectionString + @"""

                                # default SQL commands timeout
                                connection-timeout = 30s

                                # SQL server schema name to table corresponding with persistent journal
                                schema-name = dbo

                                # SQL server table corresponding with persistent journal
                                table-name = EventJournal

                                # should corresponding journal table be initialized automatically
                                auto-initialize = on

                                # timestamp provider used for generation of journal entries timestamps
                                timestamp-provider = ""Akka.Persistence.Sql.Common.Journal.DefaultTimestampProvider, Akka.Persistence.Sql.Common""

                                # metadata table
                                metadata-table-name = Metadata
                            }
                        }

                        snapshot-store {
                            sql-server {

                                # qualified type name of the SQL Server persistence journal actor
                                class = ""Akka.Persistence.SqlServer.Snapshot.SqlServerSnapshotStore, Akka.Persistence.SqlServer""

                                # dispatcher used to drive journal actor
                                plugin-dispatcher = ""akka.actor.default-dispatcher""

                                # connection string used for database access
                                connection-string = """+ akkaConf.SnapshotConnectionString + @"""

                                # default SQL commands timeout
                                connection-timeout = 30s

                                # SQL server schema name to table corresponding with persistent journal
                                schema-name = dbo

                                # SQL server table corresponding with persistent journal
                                table-name = SnapshotStore

                                # should corresponding journal table be initialized automatically
                                auto-initialize = on
                    }
                        }
                    }

                       ";
            var actorSystem = ActorSystem.Create(akkaConf.Name, hoconConfig);
            return actorSystem;
        }


        public static ActorSystem CreateClusterActorSystem(AkkaConfiguration akkaConf, int clusterPort)
        {
            var actorSystem = ActorSystem.Create(akkaConf.Name,
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
                            seed - nodes = [""akka.tcp://" + akkaConf.Name + "@" + akkaConf.Host + ":" + clusterPort + @"""]
                         
                        stdout-loglevel = " + akkaConf.LogLevel + @"
                        loglevel = " + akkaConf.LogLevel + @"
                        log-config-on-start = on

                        remote {
                                    helios.tcp {
                                        transport-class = ""Akka.Remote.Transport.Helios.HeliosTcpTransport, Akka.Remote""
                                        transport-protocol = tcp
                                        port = 0
                                        hostname = " + akkaConf.Name + @"/
                                    }
                                }
                       ");

            return actorSystem;
        }
    }
}