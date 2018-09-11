using System;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.CommonDomain;
using GridDomain.Tests.Unit.BalloonDomain;
using GridDomain.Tests.Unit.BalloonDomain.ProjectionBuilders;

namespace GridDomain.Tests.Unit.ProcessManagers {
    internal class BalloonAggregateFactory : AggregateFactory
    {
        
        public new static BalloonAggregateFactory Default { get; } = new BalloonAggregateFactory();
        
        protected override IAggregate BuildFromSnapshot(Type type, string id, ISnapshot snapshot)
        {
                var snap = snapshot as BalloonSnapshot;
                if (snap == null)
                    throw new InvalidOperationException("Sample aggregate can be restored only from memento with type "
                                                        + typeof(BalloonSnapshot).Name);

                var aggregate = new Balloon(snap.Id, snap.Value);
            aggregate.ClearUncommitedEvents();
            ((ISnapshot)aggregate).Version = snap.Version;
                return aggregate;
        }

        public override ISnapshot GetSnapshot(IAggregate aggregate)
        {
            if (aggregate is Balloon balloon)
            {
                return new BalloonSnapshot(balloon.Id, balloon.Version, balloon.Title);
            }
            return base.GetSnapshot(aggregate);
        }
        
        internal class BalloonSnapshot : ISnapshot
        {
            public BalloonSnapshot(string id, int version, string value)
            {
                Id = id;
                Version = version;
                Value = value;
            }

            public string Value { get; }

            public string Id { get; set; }
            public int Version { get; set; }
        }
    }
}