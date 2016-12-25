using System;
using GridDomain.Tests.Unit.SampleDomain;
using GridDomain.Tests.Unit.SampleDomain.Commands;
using NUnit.Framework;

namespace GridDomain.Tests.Unit.CommandsExecution.ExpectedMessages
{
    [TestFixture]
    public class When_dont_wait_execution : SampleDomainCommandExecutionTests
    {
        public When_dont_wait_execution():base(true)
        {
            
        }
        public When_dont_wait_execution(bool inMemory = true):base(inMemory)
        {
            
        }

        [Then]
        public void Aggregate_will_apply_events_later_than_command_execution_finish()
        {
            var syncCommand = new LongOperationCommand(42, Guid.NewGuid());
            GridNode.Execute(syncCommand);
            var aggregate = LoadAggregate<SampleAggregate>(syncCommand.AggregateId);
            Assert.AreNotEqual(syncCommand.Parameter, aggregate.Value);
        }
    }
}