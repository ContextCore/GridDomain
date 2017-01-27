using System;
using System.Threading.Tasks;
using GridDomain.CQRS;

namespace GridDomain.Tests.XUnit.CommandsExecution
{
  
    public class When_execute_command_Then_aggregate_Should_persist_changed : SampleDomainCommandExecutionTests
    {
        protected override bool CreateNodeOnEachTest { get; } = true;

        public When_execute_command_Then_aggregate_Should_persist_changed(bool v):base(v)
        {
        }

        public When_execute_command_Then_aggregate_Should_persist_changed()
        {
            
        }

       [Fact]
        public async Task Sync_method_should_change_aggregate()
        {
            var syncCommand = new LongOperationCommand(42, Guid.NewGuid());

            await Node.Prepare(syncCommand)
                          .Expect<SampleAggregateChangedEvent>()
                          .Execute();

            //to finish persistence
            var aggregate = this.LoadAggregate<SampleAggregate>(syncCommand.AggregateId);
            Assert.Equal(syncCommand.Parameter.ToString(), aggregate.Value);
        }

       [Fact]
        public async Task Async_method_should_change_aggregate()
        {
            var syncCommand = new AsyncMethodCommand(42, Guid.NewGuid());

            await Node.Prepare(syncCommand)
                          .Expect<SampleAggregateChangedEvent>()
                          .Execute();

            //to finish persistence
            var aggregate = this.LoadAggregate<SampleAggregate>(syncCommand.AggregateId);
            Assert.Equal(syncCommand.Parameter.ToString(), aggregate.Value);
        }
    }
}