using System;
using Akka.Configuration;
using GridDomain.Aggregates;

namespace GridDomain.Node
{
    public class HoconAggregateSettings:AggregateSettings
    {
        public HoconAggregateSettings(Config cfg):base(
            TimeSpan.FromSeconds(cfg.GetInt("aggregate.maxinactivity")),
            cfg.GetInt("aggregate.snapshotstokeep"),
            cfg.GetString("aggregate.hostrole"))
        {
        }
    }
}