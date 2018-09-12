using System;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.CommonDomain;

namespace GridDomain.Tests.Unit.DependencyInjection.Infrastructure
{
   
    
    public class TestAggregate : ConventionAggregate
    {
        private ITestDependency _testDependency;

        public TestAggregate(string Id, ITestDependency testDependency):base(Id)
        {
            _testDependency = testDependency;
            Execute<TestCommand>(c => Execute(c.Parameter, testDependency));
            Execute<TestCommandB>(c => ExecuteWithOwnedDependency(c.Parameter));
        }

        //private setter prevents factory from setting this value, so aggregate have to know its snapshot type 
        //and provide a method to build instance from it
        public string Value { get; private set; }

        public static TestAggregate FromSnapshot(TestAggregateSnapshot snapshot,ITestDependency dep)
        {
            return new TestAggregate(snapshot.Id, dep) {Value = snapshot.Value};
        }
        
        public void Execute(int number, ITestDependency d)
        {
            var dependencyUseResult = d.Do(number);
            Emit(new[] {new TestDomainEvent(dependencyUseResult, Id)});
        }

        public void ExecuteWithOwnedDependency(int number)
        {
            var dependencyUseResult = _testDependency.Do(number);
            Emit(new[] {new TestDomainEvent(dependencyUseResult, Id)});
        }

        
        private void Apply(TestDomainEvent e)
        {
            Value = e.Value;
        }
    }
}