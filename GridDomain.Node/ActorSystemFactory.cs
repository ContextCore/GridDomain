using Akka.Actor;
using Akka.Cluster;
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
                                        port = " + akkaConf.Network.PortNumber + @"}
                                        hostname = " + akkaConf.Network.Name + @"/
                                       
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
                                connection-string = """+ akkaConf.Persistence.JournalConnectionString + @"""

                                # default SQL commands timeout
                                connection-timeout = 30s

                                # SQL server schema name to table corresponding with persistent journal
                                schema-name = dbo

                                # SQL server table corresponding with persistent journal
                                table-name = """ + akkaConf.Persistence.JournalTableName + @"""

                                # should corresponding journal table be initialized automatically
                                auto-initialize = off

                                # timestamp provider used for generation of journal entries timestamps
                                timestamp-provider = ""Akka.Persistence.Sql.Common.Journal.DefaultTimestampProvider, Akka.Persistence.Sql.Common""

                                # metadata table
                                metadata-table-name = """ + akkaConf.Persistence.MetadataTableName + @"""
                            }
                        }

                        snapshot-store {
                            sql-server {

                                # qualified type name of the SQL Server persistence journal actor
                                class = ""Akka.Persistence.SqlServer.Snapshot.SqlServerSnapshotStore, Akka.Persistence.SqlServer""

                                # dispatcher used to drive journal actor
                                plugin-dispatcher = ""akka.actor.default-dispatcher""

                                # connection string used for database access
                                connection-string = """ + akkaConf.Persistence.SnapshotConnectionString + @"""

                                # default SQL commands timeout
                                connection-timeout = 30s

                                # SQL server schema name to table corresponding with persistent journal
                                schema-name = dbo

                                # SQL server table corresponding with persistent journal
                                table-name = """ + akkaConf.Persistence.SnapshotTableName + @"""

                                # should corresponding journal table be initialized automatically
                                auto-initialize = off
                    }
                        }
                    }

                       ";
            var actorSystem = ActorSystem.Create(akkaConf.Network.Name, hoconConfig);
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