using System;
using System.Threading.Tasks;
using GridDomain.CQRS;
using GridDomain.EventSourcing;
using GridDomain.Node.AkkaMessaging.Waiting;
using GridDomain.Tests.Common;
using GridDomain.Tests.Unit.BalloonDomain;
using GridDomain.Tests.Unit.BalloonDomain.Commands;
using GridDomain.Tests.Unit.BalloonDomain.Events;
using GridDomain.Tests.Unit.BalloonDomain.ProjectionBuilders;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.Unit.CommandsExecution
{
    //different fixtures from static method ? 
    public class When_executing_commands_and_aggregate_raises_an_exception : NodeTestKit
    {
        public When_executing_commands_and_aggregate_raises_an_exception(ITestOutputHelper helper)
            : base(helper, new BalloonFixture()) {}

        [Fact]
        public async Task When_aggregate_throws_fault_it_is_handled_without_implicit_registration()
        {
            //will throw exception in aggregate and in message handler
            var syncCommand = new PlanTitleWriteAndBlowCommand(50, Guid.NewGuid());
            var res = await Node.Prepare(syncCommand)
                                .Expect<BalloonTitleChangedNotification>()
                                .Or<Fault<BalloonTitleChanged>>()
                                .Execute(false);

            Assert.NotNull(res.Message<Fault<PlanTitleWriteAndBlowCommand>>());
        }

        [Fact]
        public async Task When_does_not_expect_fault_and_it_accures_wait_times_out()
        {
            var syncCommand = new PlanTitleWriteCommand(100, Guid.NewGuid());
            await Node.Prepare(syncCommand)
                      .Expect<BalloonTitleChangedNotification>(e => e.BallonId == syncCommand.AggregateId)
                      .Execute(TimeSpan.FromMilliseconds(50))
                      .ShouldThrow<TimeoutException>();
        }

        [Fact]
        public async Task When_expected_optional_fault_does_not_occur_wait_is_successfull()
        {
            var syncCommand = new PlanTitleWriteCommand(101, Guid.NewGuid());
            var res = await Node.Prepare(syncCommand)
                                .Expect<BalloonTitleChangedNotification>(e => e.BallonId == syncCommand.AggregateId)
                                .Or<Fault>(f => (f.Message as DomainEvent)?.SourceId == syncCommand.AggregateId)
                                .Execute();

            var evt = res.Message<BalloonTitleChangedNotification>();
            Assert.Equal(syncCommand.AggregateId, evt.BallonId);
        }

        [Fact]
        public async Task When_fault_is_produced_when_publish_command_with_base_type()
        {
            var syncCommand = new PlanTitleWriteAndBlowCommand(100, Guid.NewGuid());
            await Node.Prepare(syncCommand)
                      .Expect<BalloonTitleChangedNotification>(e => e.BallonId == syncCommand.AggregateId)
                      .Execute()
                      .ShouldThrowCommand<BalloonException>();
        }

        [Fact]
        public async Task When_fault_was_received_and_failOnFaults_is_set_results_raised_an_error()
        {
            var syncCommand = new PlanTitleWriteAndBlowCommand(100, Guid.NewGuid());
            await Node.Prepare(syncCommand)
                      .Expect<BalloonTitleChangedNotification>()
                      .Execute()
                      .ShouldThrowCommand<BalloonException>();
        }
    }
}