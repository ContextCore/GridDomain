using System;
using System.Threading.Tasks;
using GridDomain.EventSourcing;

namespace GridDomain.Tests.XUnit.DependencyInjection.Infrastructure
{
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