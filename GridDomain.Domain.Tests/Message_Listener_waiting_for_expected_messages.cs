using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GridDomain.CQRS;
using GridDomain.Tests.CommandsExecution;
using GridDomain.Tests.SampleDomain.Commands;
using GridDomain.Tests.SampleDomain.Events;
using NUnit.Framework;

namespace GridDomain.Tests
{
    [TestFixture]
    class Message_Listener_waiting_for_expected_messages : InMemorySampleDomainTests
    {
        [Test]
        public void Should_wait_for_all_of_events_from_different_command_execution()
        {
            var cmdCreate = new CreateSampleAggregateCommand(1, Guid.NewGuid(), Guid.NewGuid());
            var cmdChange = new ChangeSampleAggregateCommand(1, cmdCreate.AggregateId);

            var createExpect = Expect.Message<SampleAggregateCreatedEvent>(e => e.SourceId, cmdCreate.AggregateId);
            var changeExpect = Expect.Message<SampleAggregateChangedEvent>(e => e.SourceId, cmdChange.AggregateId);

            GridNode.Execute(cmdCreate, cmdChange);

            var waitTask = GridNode.Listener.WaitAll(Timeout,createExpect, changeExpect);
            Assert.True(waitTask.Wait(Timeout));
        }
    }
}
