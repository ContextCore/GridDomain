using System;
using System.Collections.Generic;
using System.Linq;
using Akka.Actor;
using Akka.Cluster.Tools.Singleton;
using Akka.Configuration;
using GridDomain.Node.Configuration;
using Serilog;

namespace GridDomain.Node.Cluster
{
    public class ClusterConfigBuilder
    {
        private readonly ActorSystemBuilder _actorSystemBuilder;
        private readonly string _clusterName;
        private List<INodeNetworkAddress> _seedNodeNetworkAddresses = new List<INodeNetworkAddress>();
        private List<INodeNetworkAddress> _workerNodeNetworkAddresses = new List<INodeNetworkAddress>();

        public ClusterConfigBuilder(string clusterName, ActorSystemBuilder systemBuilder)
        {
            _clusterName = clusterName;
            _actorSystemBuilder = systemBuilder;
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

        public ClusterConfig Build()
        {
            var clusterConfig = new ClusterConfig(_clusterName, _actorSystemBuilder.Logger);
            if (_seedNodeNetworkAddresses.All(a => a.PortNumber == 0))
            {
                _seedNodeNetworkAddresses.Add(((NodeNetworkAddress) _seedNodeNetworkAddresses.First()).Copy(10000));
                _seedNodeNetworkAddresses.RemoveAt(0);
            }
            
            var preconfiguredSeeds = _seedNodeNetworkAddresses.Where(a => a.PortNumber != 0)
                                                              .ToArray();

            foreach (var address in _seedNodeNetworkAddresses)
            {
                var systemBuilder = _actorSystemBuilder.Clone()
                                                       .Remote(address)
                                                       .ClusterSeed(_clusterName, preconfiguredSeeds);
                if (address.PortNumber == 0)
                    clusterConfig.AddAutoSeed(systemBuilder);
                else
                    clusterConfig.AddSeed(systemBuilder);
            }

            clusterConfig.AddWorker(_workerNodeNetworkAddresses.Select(i => _actorSystemBuilder.Clone()
                                                                                               .Remote(i)
                                                                                               .ClusterSeed(_clusterName, preconfiguredSeeds))
                                                               .ToArray());
            return clusterConfig;
        }
    }
}