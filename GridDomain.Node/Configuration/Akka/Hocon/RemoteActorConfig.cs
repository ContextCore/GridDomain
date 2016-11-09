using System;
using Akka.Serialization;
using CommonDomain;
using CommonDomain.Core;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.Sagas.FutureEvents;

namespace GridDomain.Node.Configuration.Akka.Hocon
{
    public abstract class RemoteActorConfig : IAkkaConfig
    {
        private readonly string _host;
        private readonly int _port;
        private readonly string _publicHost;
        private readonly bool _enforceIpVersion;

        private RemoteActorConfig(int port, string host, string publicHost, bool enforceIpVersion)
        {
            _enforceIpVersion = enforceIpVersion;
            _publicHost = publicHost;
            _host = host;
            _port = port;
        }

        protected RemoteActorConfig(IAkkaNetworkAddress config) : this(config.PortNumber, config.Host, config.PublicHost, config.EnforceIpVersion)
        {
        }


        public string Build()
        {
            string messageSerialization = "";

#if DEBUG
            messageSerialization = @"# serialize-messages = on
                                       serialize-creators = on";
#endif
            var actorConfig = @"   
       actor {
             "+messageSerialization+ @"
             serializers {
                        wire = """+typeof(WireSerializer).AssemblyQualifiedShortName()+ @"""
                        json = """+typeof(DomainEventsJsonAkkaSerializer).AssemblyQualifiedShortName() + @"""
             }
             
             serialization-bindings {
                                   """ + typeof(DomainEvent).AssemblyQualifiedShortName() + @"""   = json
                                   """ + typeof(IAggregate).AssemblyQualifiedShortName() + @"""    = json
                                  # for local snapshots storage
                                   ""Akka.Persistence.Serialization.Snapshot, Akka.Persistence"" = json
                                   ""System.Object"" = wire

             }
       }";

            var deploy = BuildActorProvider() + BuildTransport(_host, _publicHost, _port, _enforceIpVersion);

            return actorConfig + Environment.NewLine + deploy;
        }

        public abstract string BuildActorProvider();

        private string BuildTransport(string hostName, string publicHostName, int port, bool enforceIpEnv)
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
                               enforce-ip-family = " + (enforceIpEnv ? "true":"false") + @"
                    }
            }";
            return transportString;
        }
    }
}