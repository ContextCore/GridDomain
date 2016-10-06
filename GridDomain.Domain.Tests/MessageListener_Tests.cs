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
        public void Should_wait_for_event_from_single_command_execution()
        {
            var cmd = new CreateSampleAggregateCommand(1,Guid.NewGuid(),Guid.NewGuid());

            var waitTask = GridNode.Listener.WaitAny(Expect.Message<SampleAggregateCreatedEvent>(e => e.SourceId, cmd.AggregateId));
            GridNode.Execute(cmd);

            Assert.True(waitTask.Wait(Timeout));
        }

        [Test]
        public void Should_wait_for_any_of_events_from_different_command_execution()
        {
            var cmdCreate = new CreateSampleAggregateCommand(1, Guid.NewGuid(), Guid.NewGuid());
            var cmdChange = new ChangeSampleAggregateCommand(1, cmdCreate.AggregateId);

            var createExpect = Expect.Message<SampleAggregateCreatedEvent>(e => e.SourceId, cmdCreate.AggregateId);
            var changeExpect = Expect.Message<SampleAggregateChangedEvent>(e => e.SourceId, cmdChange.AggregateId);

            var waitTask = GridNode.Listener.WaitAny(createExpect, changeExpect);

            GridNode.Execute(cmdCreate, cmdChange);

            Assert.True(waitTask.Wait(Timeout));
        }

        [Test]
        public void Should_wait_for_only_expected_events()
        {
            var cmdChange = new ChangeSampleAggregateCommand(1, Guid.NewGuid());

            var createExpect = Expect.Message<SampleAggregateCreatedEvent>(e => e.SourceId, cmdChange.AggregateId);

            GridNode.Execute(cmdChange);

            var waitTask = GridNode.Listener.WaitAny(createExpect);
            Assert.False(waitTask.Wait(TimeSpan.FromMilliseconds(200)));
        }

        [Test]
        public void Should_wait_for_all_of_events_from_different_command_execution()
        {
            var cmdCreate = new CreateSampleAggregateCommand(1, Guid.NewGuid(), Guid.NewGuid());
            var cmdChange = new ChangeSampleAggregateCommand(1, cmdCreate.AggregateId);

            var createExpect = Expect.Message<SampleAggregateCreatedEvent>(e => e.SourceId, cmdCreate.AggregateId);
            var changeExpect = Expect.Message<SampleAggregateChangedEvent>(e => e.SourceId, cmdChange.AggregateId);

            GridNode.Execute(cmdCreate, cmdChange);

            var waitTask = GridNode.Listener.WaitAll(createExpect, changeExpect);
            Assert.True(waitTask.Wait(Timeout));
        }

        [Test]
        public void Should_not_wait_for_fault_by_default_by_default()
        {
            var cmdChange = new AlwaysFaultCommand(Guid.NewGuid());

            var changeExpect = Expect.Message<SampleAggregateChangedEvent>(e => e.SourceId, cmdChange.AggregateId);
            
            GridNode.Execute(cmdChange, cmdChange);

            var waitTask = GridNode.Listener.WaitAny(changeExpect);
            Assert.False(waitTask.Wait(TimeSpan.FromMilliseconds(300)));
        }


    }
}
