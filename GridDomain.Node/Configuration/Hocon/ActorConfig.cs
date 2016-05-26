using GridDomain.Node.Configuration;

internal class ActorConfig: IAkkaConfig
{
    public static string BuildActorConfig() 
    {
        string actorConfig = @"   
       actor {
             serializers {
                         wire = ""Akka.Serialization.WireSerializer, Akka.Serialization.Wire""
             }

             serialization-bindings {
                                    ""System.Object"" = wire
             }
             
             provider = ""Akka.Remote.RemoteActorRefProvider, Akka.Remote""
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

    public string Build()
    {
        return BuildActorConfig();
    }
}