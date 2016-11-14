using System;
using System.Threading;
using System.Threading.Tasks;
using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.Node.AkkaMessaging.Waiting;
using GridDomain.Tests.CommandsExecution;
using GridDomain.Tests.SampleDomain;
using GridDomain.Tests.SampleDomain.Commands;
using GridDomain.Tests.SampleDomain.Events;
using GridDomain.Tests.SampleDomain.ProjectionBuilders;
using NUnit.Framework;

namespace GridDomain.Tests.MessageWaiting.Commanding
{
    [TestFixture]
    public class Execute_command_waiting_aggregate_event : SampleDomainCommandExecutionTests
    {

        [Test]
        public void CommandWaiter_Should_wait_until_aggregate_event()
        {
            var cmd = new LongOperationCommand(100, Guid.NewGuid());

            var msg = GridNode.NewCommandWaiter(Timeout)
                              .Expect<SampleAggregateChangedEvent>(e => e.SourceId == cmd.AggregateId)
                              .Create()
                              .Execute(cmd)
                              .Result
                              .Message<SampleAggregateChangedEvent>();

            Assert.AreEqual(cmd.Parameter.ToString(), msg.Value);
        }
        
        [Test]
        public async Task MessageWaiter_after_cmd_execute_should_waits_until_aggregate_event()
        {
            var cmd = new LongOperationCommand(100, Guid.NewGuid());

            var waiter = GridNode.NewWaiter(Timeout)
                                 .Expect<SampleAggregateChangedEvent>(e => e.SourceId == cmd.AggregateId)
                                 .Create();

            GridNode.Execute(cmd);
            var res = await waiter;
            
            Assert.AreEqual(cmd.Parameter.ToString(), res.Message<SampleAggregateChangedEvent>());
        }

        [Then]
        public void After_wait_ends_aggregate_should_be_changed()
        {
            var cmd = new LongOperationCommand(100, Guid.NewGuid());

            var msg = GridNode.NewCommandWaiter(Timeout)
                            .Expect<SampleAggregateChangedEvent>(e => e.SourceId == cmd.AggregateId)
                            .Create()
                            .Execute(cmd)
                            .Result
                            .Message<SampleAggregateChangedEvent>();

            Assert.AreEqual(cmd.Parameter.ToString(), msg.Value);
        }

        [Then]
        public async Task Wait_for_timeout_command_throws_excpetion()
        {
            var cmd = new LongOperationCommand(1000, Guid.NewGuid());

            var res = GridNode.NewCommandWaiter(TimeSpan.FromMilliseconds(100))
                              .Expect<SampleAggregateChangedEvent>(e => e.SourceId == cmd.AggregateId)
                              .Create()
                              .Execute(cmd);

            await res.ShouldThrow<TimeoutException>();
        }


        [Then]
        public async Task After_wait_of_async_command_aggregate_should_be_changed()
        {
            var cmd = new AsyncMethodCommand(42, Guid.NewGuid(),Guid.NewGuid(),TimeSpan.FromMilliseconds(50));

            var res = await GridNode.NewCommandWaiter(Timeout)
                          .Expect<SampleAggregateChangedEvent>(e => e.SourceId == cmd.AggregateId)
                          .Create()
                          .Execute(cmd);

            Assert.AreEqual(cmd.Parameter.ToString(), res.Message<SampleAggregateChangedEvent>().Value);
        }


        [Then]
        public async Task CommandWaiter_will_wait_for_all_of_expected_message()
        {
            var cmd = new LongOperationCommand(1000, Guid.NewGuid());

            await GridNode.NewCommandWaiter(Timeout)
                          .Expect<SampleAggregateChangedEvent>(e => e.SourceId == cmd.AggregateId)
                          .And<SampleAggregateCreatedEvent>(e => e.SourceId == cmd.AggregateId)
                          .Create()
                          .Execute(cmd)
                          .ShouldThrow<TimeoutException>();
        }
    }
}