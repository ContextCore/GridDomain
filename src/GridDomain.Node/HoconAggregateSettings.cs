using System;
using Akka.Configuration;
using GridDomain.Aggregates;

namespace GridDomain.Node
{
    public class HoconAggregateSettings:IAggregateSettings
    {
        public HoconAggregateSettings(Config cfg)
        {
            MaxInactivityPeriod = TimeSpan.FromSeconds(cfg.GetInt("aggregate.maxinactivity"));       
            SnapshotsKeepAmount = cfg.GetInt("aggregate.snapshotstokeep");       
        }

        public TimeSpan MaxInactivityPeriod { get; }
        public int SnapshotsKeepAmount { get; }
    }
}