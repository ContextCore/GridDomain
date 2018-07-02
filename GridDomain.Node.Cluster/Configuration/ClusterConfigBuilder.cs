using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using Akka.Configuration;
using GridDomain.Node.Configuration;
using GridDomain.Node.Configuration.Hocon;

namespace GridDomain.Node.Cluster.Configuration
{
    public class ClusterConfigBuilder: IActorSystemConfigBuilder
    {
        private readonly IActorSystemConfigBuilder _seedActorSystemConfigBuilder;
        private readonly string _clusterName;
        private readonly List<INodeNetworkAddress> _seedNodeNetworkAddresses = new List<INodeNetworkAddress>();
        private readonly List<INodeNetworkAddress> _workerNodeNetworkAddresses = new List<INodeNetworkAddress>();

        public ClusterConfigBuilder(string clusterName, IActorSystemConfigBuilder systemConfigBuilder)
        {
            _clusterName = clusterName;
            _seedActorSystemConfigBuilder = systemConfigBuilder;
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
     //   private ILogger Logger { get; }

        public static int GetAvailablePort()
        {
            using (var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
            {
                socket.Bind(DefaultLoopbackEndpoint);
                return ((IPEndPoint)socket.LocalEndPoint).Port;
            }
        }

        public void Add(IHoconConfig cfg)
        {
            _seedActorSystemConfigBuilder.Add(cfg);
        }

        Config IActorSystemConfigBuilder.Build()
        {
            var clusterConfig = Build();
            return clusterConfig.AllNodes.First()
                                .Build();
        }

        public ActorSystemConfigBuilder Clone()
        {
            return _seedActorSystemConfigBuilder.Clone();
        }

        public IActorSystemFactory BuildActorSystemFactory(string systemName)
        {
            return _seedActorSystemConfigBuilder.BuildActorSystemFactory(systemName);
        }

        public ClusterConfig Build()
        {
            var clusterConfig = new ClusterConfig(_clusterName);
            if (_seedNodeNetworkAddresses.Any() && _seedNodeNetworkAddresses.All(a => a.PortNumber == 0))
            {
                _seedNodeNetworkAddresses.Add(((NodeNetworkAddress) _seedNodeNetworkAddresses.First()).Copy(GetAvailablePort()));
                _seedNodeNetworkAddresses.RemoveAt(0);
            }
            
            var preconfiguredSeeds = _seedNodeNetworkAddresses.Where(a => a.PortNumber != 0)
                                                              .ToArray();

            foreach (var address in _seedNodeNetworkAddresses)
            {
                var systemBuilder = _seedActorSystemConfigBuilder.Clone()
                                                      .Remote(address)
                                                      .ClusterSeed(_clusterName, preconfiguredSeeds);
                
                if (address.PortNumber == 0)
                    clusterConfig.AddAutoSeed(systemBuilder);
                else
                    clusterConfig.AddSeed(systemBuilder);
            }
            clusterConfig.AddWorker(_workerNodeNetworkAddresses.Select(i => _seedActorSystemConfigBuilder.Clone()
                                                                                              .Remote(i)
                                                                                              .ClusterSeed(_clusterName, preconfiguredSeeds))
                                                               .ToArray());
            return clusterConfig;
        }
    }
}