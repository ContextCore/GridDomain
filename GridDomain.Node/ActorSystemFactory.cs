using Akka.Actor;
using Akka.Cluster;
using GridDomain.Node.Configuration;

namespace GridDomain.Node
{
    static public class ActorSystemFactory
    {
        public static Cluster CreateCluster(AkkaConfiguration akkaConf)
        {

            var seed = CreateClusterSeedActorSystem(akkaConf);
            var node1 = CreateActorSystem(akkaConf, akkaConf.Port + 1);
            var node2 = CreateActorSystem(akkaConf, akkaConf.Port + 2);
            var node3 = CreateActorSystem(akkaConf, akkaConf.Port + 3);

            var cluster = Cluster.Get(seed);

          //  cluster.Join(node1.);
            return null; //retur  return Cluster.;
        }

        public static ActorSystem CreateActorSystem(AkkaConfiguration akkaConf, int? port = null)
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
                                        port = " + (port??akkaConf.Port) + @"}
                                        hostname = " + akkaConf.Name + @"/
                                       
                                    }
                                }
                       ");
            return actorSystem;
        }


        public static ActorSystem CreateClusterSeedActorSystem(AkkaConfiguration akkaConf, int? port = null)
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
                            seed - nodes = [""akka.tcp://" + akkaConf.Name + "@" + akkaConf.Host + ":" + (port?? akkaConf.Port) + @"""]
                         
                        stdout-loglevel = " + akkaConf.LogLevel + @"
                        loglevel = " + akkaConf.LogLevel + @"
                        log-config-on-start = on

                        remote {
                                    helios.tcp {
                                        transport-class = ""Akka.Remote.Transport.Helios.HeliosTcpTransport, Akka.Remote""
                                        transport-protocol = tcp
                                        port = " + (port ?? akkaConf.Port) + @"}
                                        hostname = " + akkaConf.Name + @"/
                                       
                                    }
                                }
                       ");
            return actorSystem;
        }
    }
}