using System;
using System.Collections.Generic;
using Akka.Actor;

namespace GridDomain.Node.Cluster {
    public class ClusterInfo : IDisposable
    {
        public ClusterInfo(Akka.Cluster.Cluster cluster, IReadOnlyCollection<Address> members)
        {
            Cluster = cluster;
            Members = members;
        }

        public Akka.Cluster.Cluster Cluster { get; }
        public IReadOnlyCollection<Address> Members { get; }

        public void Dispose()
        {
            CoordinatedShutdown.Get(Cluster.System)
                               .Run()
                               .Wait(TimeSpan.FromSeconds(10));
        }
    }
}