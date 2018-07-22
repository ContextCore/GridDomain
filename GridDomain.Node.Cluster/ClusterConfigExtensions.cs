using System;
using System.Threading.Tasks;
using Akka.Actor;
using GridDomain.Common;
using GridDomain.Node.Cluster;
using GridDomain.Node.Cluster.Configuration;

namespace GridDomain.Node.Cluster
{
    public static class ClusterConfigExtensions
    {
        public static Task<ClusterInfo> CreateInTime(this ClusterConfig cfg, TimeSpan? timeout = null)
        {
            timeout = timeout ?? TimeSpan.FromSeconds(15);

            return cfg
                   .Create()
                   .TimeoutAfter(timeout.Value, "Cluster was not formed in time");
        }

        public static ClusterConfig AdditionalInit(this ClusterConfig cfg, Action<ActorSystem> init)
        {
            return cfg.AdditionalInit(s =>
                                      {
                                          init(s);
                                          return Task.CompletedTask;
                                      });
        }
    }
}