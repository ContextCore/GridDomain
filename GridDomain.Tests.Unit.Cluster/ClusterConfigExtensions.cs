using System;
using System.Threading.Tasks;
using GridDomain.Common;
using GridDomain.Node.Cluster;

namespace GridDomain.Tests.Unit.Cluster {
    public static class ClusterConfigExtensions
    {
        public static Task<ClusterInfo> CreateInTime(this ClusterConfig cfg, TimeSpan? timeout = null)
        {
            timeout = timeout ?? TimeSpan.FromSeconds(15);

            cfg.AdditionalInit(s =>
                               {
                                   //var a = ActorSystemExtensions
                                   if (cfg.Logger != null)
                                       s.AttachSerilogLogging(cfg.Logger);
                                   return Task.CompletedTask;
                               });

            return cfg.Create()
                      .TimeoutAfter(timeout.Value, "Cluster was not formed in time");
        }
    }
}