using System;

namespace GridDomain.Tests.XUnit.CommandsExecution
{
  
    public class When_execute_command_without_expectations : SampleDomainCommandExecutionTests
    {

        public When_execute_command_without_expectations() : base(true)
        {

        }

        public When_execute_command_without_expectations(bool inMemory = true):base(inMemory)
        {
            
        }

       [Fact]
        public void Aggregate_will_apply_events_later_than_command_execution_finish()
        {
            var syncCommand = new LongOperationCommand(42, Guid.NewGuid());
            GridNode.Execute(syncCommand);
            var aggregate = this.LoadAggregate<SampleAggregate>(syncCommand.AggregateId);
            Assert.NotEqual(syncCommand.Parameter, aggregate.Value);
        }
    }
}