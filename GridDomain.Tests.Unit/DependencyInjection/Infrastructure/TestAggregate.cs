using System;
using GridDomain.EventSourcing;

namespace GridDomain.Tests.Unit.DependencyInjection.Infrastructure
{
    public class TestAggregate : ConventionAggregate
    {
        public string Value;

        private TestAggregate(Guid id) : base(id) {}

        public void Execute(int number, ITestDependency d)
        {
            var dependencyUseResult = d.Do(number);
            Produce(new TestDomainEvent(dependencyUseResult, Id));
        }

        private void Apply(TestDomainEvent e)
        {
            Value = e.Value;
        }
    }
}