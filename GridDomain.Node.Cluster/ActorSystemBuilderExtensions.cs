using System;
using System.Collections.Generic;
using System.Linq;
using GridDomain.Node.Actors.Logging;
using GridDomain.Node.Configuration;
using GridDomain.Node.Configuration.Hocon;
using Serilog.Events;

namespace GridDomain.Node.Cluster {


    public static class ClusterConfigBuilderExtensions
    {
        public static ClusterConfigBuilder Seeds(this ClusterConfigBuilder builder, params int[] ports)
        {
            builder.Seeds(ports.Select(p => new NodeNetworkAddress(null, p))
                               .ToArray());
            return builder;
        }
        
        public static ClusterConfigBuilder Workers(this ClusterConfigBuilder builder, int number)
        {
            builder.Workers(Enumerable.Range(0,number)
                                      .Select(p => new NodeNetworkAddress())
                                      .ToArray());
            return builder;
        }
    }
    public class ClusterConfigBuilder
    {
        private readonly ActorSystemBuilder _actorSystemBuilder;
        private readonly string _clusterName;
        private INodeNetworkAddress[] _seedNodeNetworkAddresses;
        private INodeNetworkAddress[] _workerNodeNetworkAddresses;

        public ClusterConfigBuilder(string clusterName, ActorSystemBuilder systemBuilder)
        {
            _clusterName = clusterName;
            _actorSystemBuilder = systemBuilder;
        }

        public ClusterConfigBuilder Seeds(params INodeNetworkAddress[] addresses)
        {
            _seedNodeNetworkAddresses = addresses;
            return this;
        }
        
        public ClusterConfigBuilder Workers(params INodeNetworkAddress[] addresses)
        {
            _workerNodeNetworkAddresses = addresses;
            return this;
        }

        public ClusterConfig Build()
        {
            return new ClusterConfig(_clusterName, 
                                     _seedNodeNetworkAddresses.Select(n => _actorSystemBuilder.Clone()
                                                                                              .Remote(n)
                                                                                              .ClusterSeed(_clusterName,_seedNodeNetworkAddresses)).ToArray(),
                                     _workerNodeNetworkAddresses.Select(i => _actorSystemBuilder.Clone()
                                                                                                .Remote(i)
                                                                                                .ClusterSeed(_clusterName,_seedNodeNetworkAddresses)).ToArray()); 
        }
    }
    
    public static class ActorSystemBuilderExtensions
    {
        public static ActorSystemBuilder ClusterSeed(this ActorSystemBuilder builder, string name, params INodeNetworkAddress[] otherSeeds)
        {
            builder.Add(new ClusterSeedTransportConfig(otherSeeds.Select(s => s.ToFullTcpAddress(name)).ToArray()));
            builder.Add(new AutoTerminateProcessOnClusterShutdown());
            return builder;
        }

        public static ClusterConfigBuilder Cluster(this ActorSystemBuilder builder, string name)
        {
            return new ClusterConfigBuilder(name, builder);
        }
    }
}