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
           // var hashCode = this.GetHashCode();
           // var formattableString = $"Cluster info {hashCode} {Cluster.SelfAddress.System} dispose is started";
           //
           // _logger.Information(formattableString);
            try
            {
                //Task.Delay(TimeSpan.FromSeconds(20)).Wait();//.Result;
                var a = CoordinatedShutdown.Get(Cluster.System)
                                           .Run()
                                           .GetAwaiter()
                                           .GetResult();
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Got error during cluster info {Cluster.SelfAddress.System} dispose ");
            }

            //var formattable = $"Cluster info {hashCode} {Cluster.SelfAddress.System} dispose was finished";
            //
            //_logger.Information(formattable);
        }
    }
}