using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using GridDomain.Node.Configuration;

namespace GridDomain.Node.Cluster.Configuration
{
    public class ClusterConfigBuilder
    {
        private readonly ActorSystemBuilder _seedActorSystemBuilder;
        private readonly string _clusterName;
        private readonly List<INodeNetworkAddress> _seedNodeNetworkAddresses = new List<INodeNetworkAddress>();
        private readonly List<INodeNetworkAddress> _workerNodeNetworkAddresses = new List<INodeNetworkAddress>();

        public ClusterConfigBuilder(string clusterName, ActorSystemBuilder systemBuilder)
        {
            _clusterName = clusterName;
            _seedActorSystemBuilder = systemBuilder;
        }


        public ClusterConfigBuilder Seeds(params INodeNetworkAddress[] addresses)
        {
            _seedNodeNetworkAddresses.AddRange(addresses);
            return this;
        }

        public ClusterConfigBuilder Workers(params INodeNetworkAddress[] addresses)
        {
            _workerNodeNetworkAddresses.AddRange(addresses);
            return this;
        }

        private static readonly IPEndPoint DefaultLoopbackEndpoint = new IPEndPoint(IPAddress.Loopback, port: 0);

        public static int GetAvailablePort()
        {
            using (var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
            {
                socket.Bind(DefaultLoopbackEndpoint);
                return ((IPEndPoint)socket.LocalEndPoint).Port;
            }
        }
        
        public ClusterConfig Build()
        {
            var clusterConfig = new ClusterConfig(_clusterName, _seedActorSystemBuilder.Logger);
            if (_seedNodeNetworkAddresses.Any() && _seedNodeNetworkAddresses.All(a => a.PortNumber == 0))
            {
                _seedNodeNetworkAddresses.Add(((NodeNetworkAddress) _seedNodeNetworkAddresses.First()).Copy(GetAvailablePort()));
                _seedNodeNetworkAddresses.RemoveAt(0);
            }
            
            var preconfiguredSeeds = _seedNodeNetworkAddresses.Where(a => a.PortNumber != 0)
                                                              .ToArray();

            foreach (var address in _seedNodeNetworkAddresses)
            {
                var systemBuilder = _seedActorSystemBuilder.Clone()
                                                      .Remote(address)
                                                      .ClusterSeed(_clusterName, preconfiguredSeeds);
                
                if (address.PortNumber == 0)
                    clusterConfig.AddAutoSeed(systemBuilder);
                else
                    clusterConfig.AddSeed(systemBuilder);
            }
            clusterConfig.AddWorker(_workerNodeNetworkAddresses.Select(i => _seedActorSystemBuilder.Clone()
                                                                                              .Remote(i)
                                                                                              .ClusterSeed(_clusterName, preconfiguredSeeds))
                                                               .ToArray());
            return clusterConfig;
        }
    }
}