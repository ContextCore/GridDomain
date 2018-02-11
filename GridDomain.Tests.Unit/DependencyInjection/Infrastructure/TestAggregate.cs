using System;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.CommonDomain;

namespace GridDomain.Tests.Unit.DependencyInjection.Infrastructure
{
   
    
    public class TestAggregate : ConventionAggregate
    {
        private ITestDependency _testDependency;

        //private setter prevents factory from setting this value, so aggregate have to know its snapshot type 
        //and provide a method to build instance from it
        public string Value { get; private set; }

        public static TestAggregate FromSnapshot(TestAggregateSnapshot snapshot,ITestDependency dep)
        {
            return new TestAggregate(snapshot.Id, dep) {Value = snapshot.Value};
        }
        
        internal TestAggregate(string id, ITestDependency d) : base(id)
        {
            _testDependency = d;
        }

        public void Execute(int number, ITestDependency d)
        {
            var dependencyUseResult = d.Do(number);
            Produce(new TestDomainEvent(dependencyUseResult, Id));
        }

        public void ExecuteWithOwnedDependency(int number)
        {
            var dependencyUseResult = _testDependency.Do(number);
            Produce(new TestDomainEvent(dependencyUseResult, Id));
        }

        
        private void Apply(TestDomainEvent e)
        {
            Value = e.Value;
        }
    }
}