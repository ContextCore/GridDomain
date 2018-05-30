using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Akka.Actor;
using GridDomain.Common;
using Serilog;

namespace GridDomain.Node.Cluster.Configuration
{
    public class ClusterInfo : IDisposable
    {
        private ILogger _logger;

        public ClusterInfo(Akka.Cluster.Cluster cluster, IReadOnlyCollection<Address> members, ILogger logger)
        {
            _logger = logger;
            Cluster = cluster;
            Members = members;
        }

        public Akka.Cluster.Cluster Cluster { get; }
        public IReadOnlyCollection<Address> Members { get; }

        public void Dispose()
        {
            try
            {
                CoordinatedShutdown.Get(Cluster.System)
                                   .Run()
                                   .GetAwaiter()
                                   .GetResult();
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Got error during cluster info {Cluster.SelfAddress.System} dispose ");
            }
        }
    }
}