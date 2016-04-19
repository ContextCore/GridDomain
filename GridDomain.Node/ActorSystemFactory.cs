using Akka.Actor;
using Akka.Cluster;
using GridDomain.Node.Configuration;

namespace GridDomain.Node
{
    static public class ActorSystemFactory
    {
        public static ActorSystem CreateCluster(AkkaConfiguration akkaConf)
        {
            var port = akkaConf.Port;
            var seed  = CreateClusterActorSystem(akkaConf, port);
            var node1 = CreateClusterActorSystem(akkaConf.Copy(++port), port);
            var node2 = CreateClusterActorSystem(akkaConf.Copy(++port), port);
            var node3 = CreateClusterActorSystem(akkaConf.Copy(++port), port);
            
            return seed;
        }

        public static ActorSystem CreateActorSystem(AkkaConfiguration akkaConf)
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
                       ");
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
                                        port = " + akkaConf.Port + @"}
                                        hostname = " + akkaConf.Name + @"/
                                       
                                    }
                                }
                       ");

            return actorSystem;
        }
    }
}