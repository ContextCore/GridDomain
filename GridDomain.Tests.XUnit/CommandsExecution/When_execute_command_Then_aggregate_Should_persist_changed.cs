using System;
using System.Threading.Tasks;
using GridDomain.CQRS;
using GridDomain.Tests.XUnit.SampleDomain;
using GridDomain.Tests.XUnit.SampleDomain.Commands;
using GridDomain.Tests.XUnit.SampleDomain.Events;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.XUnit.CommandsExecution
{
    public class When_execute_command_Then_aggregate_Should_persist_changed : SampleDomainCommandExecutionTests
    {
        public When_execute_command_Then_aggregate_Should_persist_changed(ITestOutputHelper output) : base(output) {}

        [Fact]
        public async Task Async_method_should_change_aggregate()
        {
            var syncCommand = new AsyncMethodCommand(42, Guid.NewGuid());

            await Node.Prepare(syncCommand).Expect<SampleAggregateChangedEvent>().Execute();

            //to finish persistence
            var aggregate = await this.LoadAggregate<SampleAggregate>(syncCommand.AggregateId);
            Assert.Equal(syncCommand.Parameter.ToString(), aggregate.Value);
        }

        [Fact]
        public async Task Sync_method_should_change_aggregate()
        {
            var syncCommand = new LongOperationCommand(42, Guid.NewGuid());

            await Node.Prepare(syncCommand).Expect<SampleAggregateChangedEvent>().Execute();

            //to finish persistence
            var aggregate = await this.LoadAggregate<SampleAggregate>(syncCommand.AggregateId);
            Assert.Equal(syncCommand.Parameter.ToString(), aggregate.Value);
        }
    }
}