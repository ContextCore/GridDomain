using System;
using System.Threading.Tasks;
using GridDomain.Tests.Unit.SampleDomain;
using GridDomain.Tests.Unit.SampleDomain.Commands;
using GridDomain.Tests.Unit.SampleDomain.Events;
using NUnit.Framework;

namespace GridDomain.Tests.Unit.CommandsExecution
{
    [TestFixture]
    public class When_execute_command_Then_aggregate_Should_persist_changed : SampleDomainCommandExecutionTests
    {
        protected override bool CreateNodeOnEachTest { get; } = true;

        public When_execute_command_Then_aggregate_Should_persist_changed(bool v):base(v)
        {
        }

        public When_execute_command_Then_aggregate_Should_persist_changed()
        {
            
        }

        [Then]
        public async Task Sync_method_should_change_aggregate()
        {
            var syncCommand = new LongOperationCommand(42, Guid.NewGuid());

            await GridNode.PrepareCommand(syncCommand)
                          .Expect<SampleAggregateChangedEvent>()
                          .Execute();

            //to finish persistence
            var aggregate = LoadAggregate<SampleAggregate>(syncCommand.AggregateId);
            Assert.AreEqual(syncCommand.Parameter.ToString(), aggregate.Value);
        }

        [Then]
        public async Task Async_method_should_change_aggregate()
        {
            var syncCommand = new AsyncMethodCommand(42, Guid.NewGuid());

            await GridNode.PrepareCommand(syncCommand)
                          .Expect<SampleAggregateChangedEvent>()
                          .Execute();

            //to finish persistence
            var aggregate = LoadAggregate<SampleAggregate>(syncCommand.AggregateId);
            Assert.AreEqual(syncCommand.Parameter.ToString(), aggregate.Value);
        }
    }
}