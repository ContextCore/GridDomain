using System;
using GridDomain.Node.Akka.Configuration.Hocon;

namespace GridDomain.Node.Akka.Cluster.Hocon
{
    public class ClusterBehaviorConfig : IHoconConfig
    {
        private readonly string _autoDownUnreachablePeriod;
        private readonly string _moveShardedEntities;

        public ClusterBehaviorConfig(TimeSpan? autoDownUnreachablePeriod = null, TimeSpan? moveShardedEntities = null)
        {
            _moveShardedEntities = moveShardedEntities == null ? "off": moveShardedEntities.Value.TotalSeconds + "s";
            _autoDownUnreachablePeriod = autoDownUnreachablePeriod == null ? "off": autoDownUnreachablePeriod.Value.TotalSeconds + "s";
        }
        public string Build()
        {
            return @"akka.cluster.auto-down-unreachable-after = "+_autoDownUnreachablePeriod+@"
                        akka.cluster.down-removal-margin = "+_moveShardedEntities;
        }
    }
}