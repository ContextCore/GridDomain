using System;
using GridDomain.CQRS;
using GridDomain.Node.AkkaMessaging.Waiting;
using GridDomain.Tests.Common;

namespace GridDomain.Node.Cluster {
    public static class GridNodeBuilderExtensions
    {
        public static IGridDomainNode BuildCluster(this GridNodeBuilder builder)
        {
            return new GridClusterNode(builder.Configurations,builder.ActorSystemFactory,builder.Logger,builder.DefaultTimeout);
        }



      
    }
}