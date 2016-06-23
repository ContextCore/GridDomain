using System;
using CommonDomain.Core;

namespace GridDomain.Tests.DependencyInjection
{
    public class TestAggregate : AggregateBase
    {
        private TestAggregate(Guid id)
        {
            Id = id;
        }

        public void Execute(int number, ITestDependency d)
        {
            var dependencyUseResult = d.Do(number);
            RaiseEvent(new DomainEvent(dependencyUseResult));
        }

        private void Apply(int number, ITestDependency d)
        {
            d.Do(number);
        }

        public class DomainEvent
        {
            public string Value;

            public DomainEvent(string value)
            {
                Value = value;
            }
        }
    }

    
}