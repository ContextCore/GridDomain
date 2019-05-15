using System;

namespace GridDomain.Node.Akka.Configuration.Hocon
{
    public class RemoteConfig : IHoconConfig
    {
        private readonly string _host;
        private readonly int _port;
        private readonly string _publicHost;

        private RemoteConfig(int port, string host, string publicHost)
        {
            _publicHost = publicHost;
            _host = host;
            _port = port;
        }

        public RemoteConfig(NodeNetworkAddress config)
            : this(config.PortNumber, config.Host, config.PublicHost)
        {
        }

        public string Build()
        {
            var transportString = @"akka.remote {
                    log-remote-lifecycle-events = DEBUG
                    dot-netty.tcp {
                               port = " + _port + @"
                               hostname =  " + _host + @"
                               public-hostname = " + _publicHost + @"
                    }
            }";
            return transportString;
        }
    }
}