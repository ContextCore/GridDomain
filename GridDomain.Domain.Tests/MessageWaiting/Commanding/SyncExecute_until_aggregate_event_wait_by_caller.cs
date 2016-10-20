using System;
using GridDomain.CQRS;
using GridDomain.Tests.CommandsExecution;
using GridDomain.Tests.SampleDomain;
using GridDomain.Tests.SampleDomain.Commands;
using GridDomain.Tests.SampleDomain.Events;
using NUnit.Framework;

namespace GridDomain.Tests.MessageWaiting.Commanding
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

        [Test]
        public void Then_SyncExecute_until_aggregate_event_wait_by_caller()
        {
            var cmd = new LongOperationCommand(1000, Guid.NewGuid());

            var waiter = GridNode.NewCommandWaiter()
                                   .Expect<SampleAggregateChangedEvent>(e => e.SourceId == cmd.AggregateId)
                                   .Create(Timeout)
                                 .Execute(cmd);

            if (!waiter.Wait(Timeout))
                throw new TimeoutException();

            //to finish persistence
            var aggregate = LoadAggregate<SampleAggregate>(cmd.AggregateId);
            Assert.AreEqual(cmd.Parameter.ToString(), aggregate.Value);
        }
    }
}