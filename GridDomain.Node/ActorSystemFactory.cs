using Akka.Actor;
using GridDomain.Node.Configuration;

static public class ActorSystemFactory
{
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


                        cluster {
                                seed-nodes = ""akka.tcp://" + akkaConf.Name + "@" + akkaConf.Host + ":" + akkaConf.Port +
            @"""
                            }
                        remote {
                                    helios.tcp {
                                        transport-class = ""Akka.Remote.Transport.Helios.HeliosTcpTransport, Akka.Remote""
                                        transport-protocol = tcp
                                        port = " + akkaConf.Port + @"}
                                        hostname = " + akkaConf.Host + @"/
                                       
                                    }
                                }
                       ");
        return actorSystem;
    }
}