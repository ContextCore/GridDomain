using System;
using System.Threading.Tasks;
using GridDomain.CQRS;
using GridDomain.Node.AkkaMessaging.Waiting;

namespace GridDomain.Tests.XUnit.CommandsExecution
{
  
    class Async_execution_dont_block_aggregate : SampleDomainCommandExecutionTests
    {
       
       [Fact]
        public async Task When_async_method_is_called_other_commands_can_be_executed_before_async_results()
        {
            var aggregateId = Guid.NewGuid();
            var asyncCommand = new AsyncMethodCommand(43, Guid.NewGuid(),Guid.NewGuid(),TimeSpan.FromSeconds(3));
            var syncCommand = new ChangeSampleAggregateCommand(42, aggregateId);

            var asyncCommandTask =Node.Prepare(asyncCommand)
                                           .Expect<SampleAggregateChangedEvent>()
                                           .Execute();

           await Node.Prepare(syncCommand)
                         .Expect<SampleAggregateChangedEvent>()
                         .Execute();

            var sampleAggregate = this.LoadAggregate<SampleAggregate>(syncCommand.AggregateId);

            Assert.Equal(syncCommand.Parameter.ToString(), sampleAggregate.Value);
            var waitResults = await asyncCommandTask;
            Assert.Equal(asyncCommand.Parameter.ToString(), waitResults.Message<SampleAggregateChangedEvent>().Value);
        }
    }
}