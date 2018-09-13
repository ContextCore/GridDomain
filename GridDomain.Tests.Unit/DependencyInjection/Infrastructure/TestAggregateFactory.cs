using System;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.CommonDomain;

namespace GridDomain.Tests.Unit.DependencyInjection.Infrastructure {
    public class TestAggregateFactory : AggregateFactory
    {
        private readonly TestDependencyImplementation _testDependencyImplementation;

        public TestAggregateFactory(TestDependencyImplementation testDependencyImplementation)
        {
            _testDependencyImplementation = testDependencyImplementation;
        }

        public override IAggregate Build(Type type, string id, ISnapshot snapshot = null)
        {
            if (type == typeof(TestAggregate) && snapshot == null)
            {
                return new TestAggregate(id, _testDependencyImplementation);
            }
            return base.Build(type, id, snapshot);
        }

        public override ISnapshot GetSnapshot(IAggregate aggregate)
        {
            if (aggregate is TestAggregate agr)
            {
                return new TestAggregateSnapshot(){Id = agr.Id, Version = agr.Version, Value = agr.Value};
            }
            return base.GetSnapshot(aggregate);
        }

        protected override IAggregate BuildFromSnapshot(Type type, string id, ISnapshot snapshot)
        {
            if (type == typeof(TestAggregate))
            {
                var snap = snapshot as TestAggregateSnapshot;
                if(snap == null) throw new ArgumentException(nameof(snapshot));
                var agr = TestAggregate.FromSnapshot(snap, _testDependencyImplementation);
                ((ISnapshot) agr).Version = snap.Version;
                agr.Clear();
                return agr;
            }
            else return base.Build(type, id, snapshot);
        }

     
    }
}