using System;
using System.Threading.Tasks;
using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.Node.AkkaMessaging.Waiting;
using GridDomain.Scheduling;
using GridDomain.Scheduling.Quartz;
using GridDomain.Tests.Common;
using GridDomain.Tests.Unit.BalloonDomain;
using GridDomain.Tests.Unit.BalloonDomain.Commands;
using GridDomain.Tests.Unit.BalloonDomain.Configuration;
using GridDomain.Tests.Unit.BalloonDomain.Events;
using GridDomain.Tests.Unit.FutureEvents.Infrastructure;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.Unit.CommandsExecution.ExecutionWithErrors
{
    public class When_execute_command_causing_an_aggregate_error : NodeTestKit
    {
        public When_execute_command_causing_an_aggregate_error(ITestOutputHelper output) :
            this(new NodeTestFixture(output)) { }

        protected When_execute_command_causing_an_aggregate_error(NodeTestFixture fixture) :
            base(fixture.Add(new BalloonDomainConfiguration())) { }

        [Fact]
        public async Task Given_async_aggregate_method_Then_execute_throws_exception_from_aggregate()
        {
            await Node.Execute(new PlanBallonBlowCommand(Guid.NewGuid()
                                                             .ToString()))
                      .ShouldThrow<BalloonException>();
        }

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
    }
}