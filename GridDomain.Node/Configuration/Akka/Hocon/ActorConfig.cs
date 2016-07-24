using System;

namespace GridDomain.Node.Configuration.Akka.Hocon
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
            string messageSerialization = "";
#if DEBUG
            messageSerialization = @"serialize-messages = on
                                     serialize-creators = on";
#endif
            var actorConfig = @"   
       actor {
             "+messageSerialization+@"
             serializers {
                         wire = ""Akka.Serialization.WireSerializer, Akka.Serialization.Wire""
                         #json = ""Akka.Serialization.NewtonSoftJsonSerializer, Akka.Serialization.Json""
                }
             
             serialization-bindings {
                                    ""System.Object"" = wire
                                    #""Automatonymous.State"" = json
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