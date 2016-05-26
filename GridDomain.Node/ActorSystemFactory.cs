using Akka.Actor;
using Akka.Cluster;
using Akka.Persistence;
using Akka.Persistence.SqlServer;
using GridDomain.Node.Configuration;

namespace GridDomain.Node
{
    public class ActorSystemFactory
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
            var actorSystem = ActorSystem.Create(akkaConf.Network.Name, akkaConf.ToSingleSystemConfig());
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