using System;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Configuration;
using GridDomain.Aggregates;

namespace GridDomain.Node
{

    public class HoconAggregateConfiguration:IAggregateConfiguration
    {
        public HoconAggregateConfiguration(Config cfg)
        {
            MaxInactivityPeriod = TimeSpan.FromSeconds(cfg.GetInt("aggregate.maxinactivity"));       
            SnapshotsKeepAmount = cfg.GetInt("aggregate.snapshotstokeep");       
        }

        public TimeSpan MaxInactivityPeriod { get; }
        public int SnapshotsKeepAmount { get; }
    }
    public interface IActorSystemFactory
    {
        ActorSystem CreateSystem();
    }
}