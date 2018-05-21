using System;
using System.Threading.Tasks;
using GridDomain.CQRS;
using GridDomain.EventSourcing;
using GridDomain.Node.AkkaMessaging.Waiting;
using GridDomain.Scheduling.Quartz;
using GridDomain.Tests.Common;
using GridDomain.Tests.Unit.BalloonDomain;
using GridDomain.Tests.Unit.BalloonDomain.Commands;
using GridDomain.Tests.Unit.BalloonDomain.Events;
using GridDomain.Tests.Unit.BalloonDomain.ProjectionBuilders;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.Unit.CommandsExecution.ExecutionWithErrors
{
    //different fixtures from static method ? 
    public class When_executing_commands_and_aggregate_raises_an_exception : NodeTestKit
    {
        protected When_executing_commands_and_aggregate_raises_an_exception(NodeTestFixture fixture) : base(fixture) { }

        public When_executing_commands_and_aggregate_raises_an_exception(ITestOutputHelper helper)
            : base(new BalloonFixture(helper)) { }

        [Fact]
        public async Task Given_aggregate_method_When_execute_prepared_Then_exception_is_caught()
        {
            await Node.Prepare(new PlanBallonBlowCommand("asd", TimeSpan.FromMilliseconds(500)))
                      .Expect<JobFailed>()
                      .Execute(TimeSpan.FromSeconds(5))
                      .ShouldThrow<BalloonException>();
        }

        [Fact]
        public async Task Given_aggregate_method_When_execute_prepared_Then_exception_is_caught_2()
        {
            await Node.Prepare(new PlanBallonBlowCommand("asd", TimeSpan.FromMilliseconds(500)))
                      .Expect(typeof(JobFailed))
                      .Execute(TimeSpan.FromSeconds(5))
                      .ShouldThrow<BalloonException>();
        }

        [Fact]
        public async Task Given_async_aggregate_method_Then_execute_throws_exception_from_aggregate()
        {
            await Node.Execute(new PlanBallonBlowCommand(Guid.NewGuid()
                                                             .ToString()))
                      .ShouldThrow<BalloonException>();
        }

        [Fact]
        public async Task Given_async_aggregate_method_Then_execute_throws_exception_from_aggregate_with_stack_trace()
        {
            var syncCommand = new PlanTitleWriteAndBlowCommand(42,
                                                               Guid.NewGuid()
                                                                   .ToString(),
                                                               TimeSpan.FromMilliseconds(500));

            await Node.Execute(syncCommand)
                      .ShouldThrow<BalloonException>((ex => ex.StackTrace.Contains(typeof(Balloon).Name) || ex.Message.Contains(typeof(Balloon).Name)));
        }

        [Fact]
        public async Task Given_sync_aggregate_method_Then_execute_throws_exception_from_aggregate_with_stack_trace()
        {
            await Node.Execute(new BlowBalloonCommand(Guid.NewGuid()
                                                          .ToString()))
                      .ShouldThrow<BalloonException>((ex => ex.StackTrace.Contains(typeof(Balloon).Name) || ex.Message.Contains(typeof(Balloon).Name)));
        }

        [Fact]
        public async Task Given_aggregate_When_executing_several_commands_in_same_inbox_and_part_fails_Then_other_should_be_executed()
        {
            var waiter = Node.NewWaiter()
                             .Expect<BalloonTitleChanged>()
                             .Create();

            var aggregateId = "test_balloon";
            //aggregate should fail after blow command
            //and restores after write title, no loosing write title command
            // intentionally not waiting for command result, allowing aggregate actor take care of command managment and 
            // storage

#pragma warning disable 4014
            Node.Execute(new InflateNewBallonCommand(1, aggregateId), CommandConfirmationMode.None);
            Node.Execute(new BlowBalloonCommand(aggregateId), CommandConfirmationMode.None);
            Node.Execute(new WriteTitleCommand(2, aggregateId), CommandConfirmationMode.None);
#pragma warning restore 4014

            var evt = await waiter;
            Assert.Equal("2",
                         evt.Message<BalloonTitleChanged>()
                            .Value);
        }

        [Fact]
        public async Task When_aggregate_throws_fault_it_is_handled_without_implicit_registration()
        {
            //will throw exception in aggregate and in message handler
            await Node.Prepare(new PlanTitleWriteAndBlowCommand(50, Guid.NewGuid()))
                      .Expect<BalloonTitleChangedNotification>()
                      .Execute()
                      .ShouldThrow<BalloonException>();
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
                                .Or<Fault<DomainEvent>>(f => f.Message.SourceId == syncCommand.AggregateId)
                                .Execute();

            var evt = res.Message<BalloonTitleChangedNotification>();
            Assert.Equal(syncCommand.AggregateId, evt.BallonId);
        }

        [Fact]
        public async Task When_fault_is_produced_when_publish_command_with_base_type()
        {
            var syncCommand = new PlanTitleWriteAndBlowCommand(100, Guid.NewGuid());
            await (Task<BalloonException>) Node.Prepare(syncCommand)
                                               .Expect<BalloonTitleChangedNotification>(e => e.BallonId == syncCommand.AggregateId)
                                               .Execute()
                                               .ShouldThrow((Predicate<BalloonException>) null);
        }

        [Fact]
        public async Task When_fault_was_received_and_failOnFaults_is_set_results_raised_an_error()
        {
            var syncCommand = new PlanTitleWriteAndBlowCommand(100, Guid.NewGuid());
            await (Task<BalloonException>) Node.Prepare(syncCommand)
                                               .Expect<BalloonTitleChangedNotification>()
                                               .Execute()
                                               .ShouldThrow((Predicate<BalloonException>) null);
        }
    }
}