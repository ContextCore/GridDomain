using System;
using GridDomain.Configuration;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.CommonDomain;
using GridDomain.Node.Configuration.Composition;

namespace GridDomain.Tests.Unit.DependencyInjection.Infrastructure {
    public class TestAggregateDomainConfiguration : IDomainConfiguration
    {
        public void Register(IDomainBuilder builder)
        {
            var commandHandler = new TestAggregatesCommandHandler(new TestDependencyImplementation());
            var aggregateDependencyFactory = DefaultAggregateDependencyFactory.New(commandHandler);
            aggregateDependencyFactory.AggregateFactoryCreator = () => new TestAggregateFactory(new TestDependencyImplementation());
            aggregateDependencyFactory.SnapshotsFactoryCreator = () => new TestAggregateFactory(new TestDependencyImplementation());
            builder.RegisterAggregate(aggregateDependencyFactory);
        }
    }

    public class TestAggregateSnapshot : IMemento
    {
        public string Id { get; set; }
        public int Version { get; set; }
        public string Value { get; set; }
    }
    
    public class TestAggregateFactory : AggregateFactory
    {
        private readonly TestDependencyImplementation _testDependencyImplementation;

        public TestAggregateFactory(TestDependencyImplementation testDependencyImplementation)
        {
            _testDependencyImplementation = testDependencyImplementation;
        }

        public override IAggregate Build(Type type, string id, IMemento snapshot = null)
        {
            if (type == typeof(TestAggregate) && snapshot == null)
            {
              return new TestAggregate(id, _testDependencyImplementation);
            }
            return base.Build(type, id, snapshot);
        }

        public override IMemento GetSnapshot(IAggregate aggregate)
        {
            if (aggregate is TestAggregate agr)
            {
                return new TestAggregateSnapshot(){Id = agr.Id, Version = agr.Version, Value = agr.Value};
            }
            return base.GetSnapshot(aggregate);
        }

        protected override IAggregate BuildFromSnapshot(Type type, string id, IMemento snapshot)
        {
            if (type == typeof(TestAggregate))
            {
                var snap = snapshot as TestAggregateSnapshot;
                if(snap == null) throw new ArgumentException(nameof(snapshot));
                var agr = TestAggregate.FromSnapshot(snap, _testDependencyImplementation);
                ((IMemento) agr).Version = snap.Version;
                agr.CommitAll();
                return agr;
            }
            else return base.Build(type, id, snapshot);
        }

     
    }
}