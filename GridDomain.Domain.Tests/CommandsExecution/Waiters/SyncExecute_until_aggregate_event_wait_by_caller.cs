using System;
using GridDomain.CQRS;
using GridDomain.Tests.SampleDomain;
using GridDomain.Tests.SampleDomain.Commands;
using GridDomain.Tests.SampleDomain.Events;
using NUnit.Framework;

namespace GridDomain.Tests.CommandsExecution.Waiters
{
    [TestFixture]
    public class SyncExecute_until_aggregate_event_wait_by_caller : SampleDomainCommandExecutionTests
    {
        public SyncExecute_until_aggregate_event_wait_by_caller():base(true)
        {
            
        }
        public SyncExecute_until_aggregate_event_wait_by_caller(bool inMemory=true):base(inMemory)
        {
            
        }

        [Then]
        public void Then_SyncExecute_until_aggregate_event_wait_by_caller()
        {
            var syncCommand = new LongOperationCommand(1000, Guid.NewGuid());
            var expectedMessage = Expect.Message<SampleAggregateChangedEvent>(nameof(SampleAggregateChangedEvent.SourceId),
                                                                              syncCommand.AggregateId);
            var task = GridNode.Execute(syncCommand,expectedMessage);
            if (!task.Wait(Timeout))
                throw new TimeoutException();

            //to finish persistence
            var aggregate = LoadAggregate<SampleAggregate>(syncCommand.AggregateId);
            Assert.AreEqual(syncCommand.Parameter.ToString(), aggregate.Value);
        }
    }
}