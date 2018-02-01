using System;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.CommonDomain;
using GridDomain.Tests.Unit.BalloonDomain;
using GridDomain.Tests.Unit.BalloonDomain.ProjectionBuilders;

namespace GridDomain.Tests.Unit.ProcessManagers {
    internal class BalloonAggregateFactory : AggregateFactory
    {
        
        public new static BalloonAggregateFactory Default { get; } = new BalloonAggregateFactory();
        
        protected override IAggregate BuildFromSnapshot(Type type, Guid id, IMemento memento)
        {
                var snapshot = memento as BalloonSnapshot;
                if (snapshot == null)
                    throw new InvalidOperationException("Sample aggregate can be restored only from memento with type "
                                                        + typeof(BalloonSnapshot).Name);

                var aggregate = new Balloon(snapshot.Id, snapshot.Value);
                aggregate.CommitAll();
                ((IMemento)aggregate).Version = snapshot.Version;
                return aggregate;
        }

        public override IMemento GetSnapshot(IAggregate aggregate)
        {
            if (aggregate is Balloon balloon)
            {
                return new BalloonSnapshot(balloon.Id, balloon.Version, balloon.Title);
            }
            return base.GetSnapshot(aggregate);
        }
        
        internal class BalloonSnapshot : IMemento
        {
            public BalloonSnapshot(Guid id, int version, string value)
            {
                Id = id;
                Version = version;
                Value = value;
            }

            public string Value { get; }

            public Guid Id { get; set; }
            public int Version { get; set; }
        }
    }
}