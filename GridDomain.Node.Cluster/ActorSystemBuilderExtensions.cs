using System;
using System.Collections.Generic;
using System.Linq;
using Akka.Cluster.Tools.Singleton;
using Akka.Configuration;
using GridDomain.Node.Actors.Logging;
using GridDomain.Node.Cluster.Configuration;
using GridDomain.Node.Cluster.Configuration.Hocon;
using GridDomain.Node.Configuration;
using GridDomain.Node.Configuration.Hocon;
using Serilog.Events;

namespace GridDomain.Node.Cluster {
    public static class ActorSystemBuilderExtensions
    {
        public static IActorSystemConfigBuilder ClusterSeed(this IActorSystemConfigBuilder configBuilder, string name, params INodeNetworkAddress[] otherSeeds)
        {
            configBuilder.Add(new ClusterSeedAwareTransportConfig(otherSeeds.Select(s => s.ToFullTcpAddress(name)).ToArray()));
            return configBuilder;
        }
        
        public static ClusterConfigBuilder Cluster(this IActorSystemConfigBuilder configBuilder, string name=null)
        {
            name = name ?? "TestCluster" + configBuilder.GetHashCode();
            configBuilder.Add(new PubSubConfig());
           // builder.Add(new ClusterSingletonInternalMessagesSerializerConfig());
            configBuilder.Add(new ClusterShardingMessagesSerializerConfig());
            configBuilder.Add(new HyperionForAll());
           // builder.Add(new AutoTerminateProcessOnClusterShutdown());

            return new ClusterConfigBuilder(name, configBuilder, configBuilder.Logger);
        }  
        
        public static IActorSystemFactory BuildClusterSystemFactory(this IActorSystemConfigBuilder configBuilder, string name)
        {
            Config hocon = configBuilder.Build();
            var factory = new HoconActorSystemFactory(name, hocon.WithFallback(ClusterSingletonManager.DefaultConfig()));
            return factory;
        }
    }
}