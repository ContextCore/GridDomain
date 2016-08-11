using System;

namespace GridDomain.Node.Configuration.Akka.Hocon
{
    public abstract class ActorConfig : IAkkaConfig
    {
        private readonly string _host;
        private readonly int _port;
        private readonly string _publicHost;

        private ActorConfig(int port, string host, string publicHost)
        {
            _publicHost = publicHost;
            _host = host;
            _port = port;
        }

        protected ActorConfig(IAkkaNetworkAddress config) : this(config.PortNumber, config.Host, config.PublicHost)
        {
        }


        public string Build()
        {
            string messageSerialization = "";
//#if DEBUG
//            messageSerialization = @"serialize-messages = on
//                                     serialize-creators = on";
//#endif
            var actorConfig = @"   
       actor {
             "+messageSerialization+ @"
             serializers {
                        wire = ""Akka.Serialization.WireSerializer, Akka.Serialization.Wire""
             }
             
             serialization-bindings {
                                   ""System.Object"" = wire
             }
       }";

            var deploy = BuildActorProvider() + BuildTransport(_host, _publicHost, _port);

            return actorConfig + Environment.NewLine + deploy;
        }

        public abstract string BuildActorProvider();

        private string BuildTransport(string hostName, string publicHostName, int port)
        {
            var transportString =
                @"remote {
                    log-remote-lifecycle-events = DEBUG
                    helios.tcp {
                               transport-class = ""Akka.Remote.Transport.Helios.HeliosTcpTransport, Akka.Remote""
                               transport-protocol = tcp
                               port = " + port + @"
                               hostname =  " + hostName + @"
                               public-hostname = " + publicHostName + @"
                    }
            }";
            return transportString;
        }
    }
}