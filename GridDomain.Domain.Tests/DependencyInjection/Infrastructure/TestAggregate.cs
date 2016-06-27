using System;
using CommonDomain.Core;
using GridDomain.EventSourcing;

namespace GridDomain.Tests.DependencyInjection.Infrastructure
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
            RaiseEvent(new TestDomainEvent(dependencyUseResult, Id));
        }

        private void Apply(TestDomainEvent e)
        {
            Value = e.Value;
        }

        public string Value;
    }
    public class TestDomainEvent : DomainEvent
    {
        public string Value;
        public TestDomainEvent(string value, Guid sourceId, DateTime? createdTime = default(DateTime?), Guid sagaId = default(Guid)) : base(sourceId, createdTime, sagaId)
        {
            Value = value;

        }
    }


}