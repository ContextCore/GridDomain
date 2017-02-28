using System;
using CommonDomain.Core;

namespace GridDomain.Tests.XUnit.DependencyInjection.Infrastructure
{
    public class TestAggregate : AggregateBase
    {
        public string Value;

        private TestAggregate(Guid id)
        {
            Id = id;
        }

        public void Execute(int number, ITestDependency d)
        {
            var dependencyUseResult = d.Do(number);
            RaiseEvent(new TestDomainEvent(dependencyUseResult, Id));
        }

        private void Apply(TestDomainEvent e)
        {
            Value = e.Value;
        }
    }
}