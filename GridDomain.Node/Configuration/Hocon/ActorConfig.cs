using System;
using Akka.Actor;
using GridDomain.Node.Configuration;


public abstract class ActorConfig: IAkkaConfig
{
    private readonly int _port;
    private readonly string _name;

    protected ActorConfig(int port, string name)
    {
        _name = name;
        _port = port;
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
             
             loggers = [""Akka.Logger.NLog.NLogLogger, Akka.Logger.NLog""]
             debug {
                   receive = on
                   autoreceive = on
                   lifecycle = on
                   event-stream = on
                   unhandled = on
             }

       }";

        var deploy = BuildActorProvider() + BuildTransport(_name, _port);

        return actorConfig + Environment.NewLine + deploy;
    }

    public abstract string BuildActorProvider();
    private string BuildTransport(string name, int port)
    {
        string transportString = 
           @"remote {
                    helios.tcp {
                               transport-class = ""Akka.Remote.Transport.Helios.HeliosTcpTransport, Akka.Remote""
                               transport-protocol = tcp
                               port = " + port + @"
                               hostname = localhost
                    }
            }";
        return transportString;
    }
}