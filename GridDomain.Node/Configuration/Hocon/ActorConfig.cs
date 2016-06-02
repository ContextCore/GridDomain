using System;

namespace GridDomain.Node.Configuration.Hocon
{
    public abstract class ActorConfig : IAkkaConfig
    {
        private readonly string _host;
        private readonly int _port;

        private ActorConfig(int port, string host)
        {
            _host = host;
            _port = port;
        }

        protected ActorConfig(IAkkaNetworkAddress config) : this(config.PortNumber, config.Host)
        {
        }


        public string Build()
        {
            var actorConfig = @"   
       actor {
             #serializers {
             #            wire = ""Akka.Serialization.WireSerializer, Akka.Serialization.Wire""
             #}
             #
             #serialization-bindings {
             #                       ""System.Object"" = wire
             #}
             
             loggers = [""Akka.Logger.NLog.NLogLogger, Akka.Logger.NLog""]
             debug {
                   receive = on
                   autoreceive = on
                   lifecycle = on
                   event-stream = on
                   unhandled = on
             }

       }";

            var deploy = BuildActorProvider() + BuildTransport(_host, _port);

            return actorConfig + Environment.NewLine + deploy;
        }

        public abstract string BuildActorProvider();

        private string BuildTransport(string name, int port)
        {
            var transportString =
                @"remote {
                    helios.tcp {
                               transport-class = ""Akka.Remote.Transport.Helios.HeliosTcpTransport, Akka.Remote""
                               transport-protocol = tcp
                               port = " + port + @"
                               hostname = " + name + @"
                    }
            }";
            return transportString;
        }
    }
}