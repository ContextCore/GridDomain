using System;
using System.Threading.Tasks;
using CommonDomain.Core;
using GridDomain.EventSourcing;

namespace GridDomain.Tests.XUnit.DependencyInjection.Infrastructure
{
    public class TestAggregate : Aggregate
    {
        public string Value;

        private TestAggregate(Guid id):base(id)
        {
        }

        public async Task Execute(int number, ITestDependency d)
        {
            var dependencyUseResult = d.Do(number);
            await Emit(new TestDomainEvent(dependencyUseResult, Id));
        }

        private void Apply(TestDomainEvent e)
        {
            Value = e.Value;
        }
    }
}