using System;
using System.Collections.Generic;
using GridDomain.Node.Actors.Logging;
using GridDomain.Node.Configuration;
using GridDomain.Node.Configuration.Hocon;
using Serilog.Events;

namespace GridDomain.Node.Cluster {
    public static class ActorSystemBuilderExtensions
    {
        public static ActorSystemBuilder ClusterSeed(this ActorSystemBuilder builder, NodeConfiguration thisSeed, params INodeNetworkAddress[] otherSeeds)
        {
            builder.Add(ClusterConfig.SeedNode(thisSeed, otherSeeds));
            return builder;
        }

        public static ActorSystemBuilder ClusterNonSeed(this ActorSystemBuilder builder, NodeConfiguration thisSeed, params INodeNetworkAddress[] otherSeeds)
        {
            builder.Add(ClusterConfig.NonSeedNode(thisSeed, otherSeeds));
            return builder;
        }


    }
}