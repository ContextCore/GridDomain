using GridDomain.Node.Configuration;

internal class ActorConfig: IAkkaConfig
{
    private string _providerType;

    private ActorConfig(string providerType)
    {
        _providerType = providerType;
    }

    public static ActorConfig SingleSystem()
    {
         return new ActorConfig("Akka.Remote.RemoteActorRefProvider, Akka.Remote"); 
    }

    public static ActorConfig Cluster()
    {
        return new ActorConfig("Akka.Cluster.ClusterActorRefProvider, Akka.Cluster");
    }

    public string Build()
    {
        string actorConfig = @"   
       actor {
             serializers {
                         wire = ""Akka.Serialization.WireSerializer, Akka.Serialization.Wire""
             }

             serialization-bindings {
                                    ""System.Object"" = wire
             }
             
             provider = """+_providerType+@"""
             loggers = [""Akka.Logger.NLog.NLogLogger, Akka.Logger.NLog""]
             debug {
                   receive = on
                   autoreceive = on
                   lifecycle = on
                   event-stream = on
                   unhandled = on
             }
       }";
        return actorConfig;
    }
}