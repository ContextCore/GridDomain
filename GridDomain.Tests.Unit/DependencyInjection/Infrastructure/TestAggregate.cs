using System;
using GridDomain.CQRS.Messaging;
using GridDomain.EventSourcing;
using GridDomain.Node.Configuration.Composition;
using Microsoft.Practices.Unity;

namespace GridDomain.Tests.Unit.DependencyInjection.Infrastructure
{

    public class TestAggregateDomainConfiguration : IDomainConfiguration
    {
        public void Register(IDomainBuilder builder)
        {
            builder.RegisterAggregate(DefaultAggregateDependencyFactory.New(new TestAggregatesCommandHandler(new TestDependencyImplementation())));
        }
    }

    public class TestAggregate : Aggregate
    {
        public string Value;

        private TestAggregate(Guid id) : base(id) {}

        public void Execute(int number, ITestDependency d)
        {
            var dependencyUseResult = d.Do(number);
            Emit(new TestDomainEvent(dependencyUseResult, Id));
        }

        private void Apply(TestDomainEvent e)
        {
            Value = e.Value;
        }
    }
}