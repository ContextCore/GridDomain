using System;
using System.Linq;
using System.Threading.Tasks;
using GridDomain.CQRS;
using GridDomain.Node.AkkaMessaging.Waiting;
using GridDomain.Tests.Unit.BalloonDomain;
using GridDomain.Tests.Unit.BalloonDomain.Commands;
using GridDomain.Tests.Unit.BalloonDomain.Configuration;
using GridDomain.Tests.Unit.BalloonDomain.Events;
using GridDomain.Tests.Unit.CommandsExecution.ExecutionWithErrors;
using GridDomain.Tests.Unit.FutureEvents.Infrastructure;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.Unit.CommandsExecution
{
    public class Async_execution_dont_block_aggregate : NodeTestKit
    {
        public Async_execution_dont_block_aggregate(ITestOutputHelper output) : this(new NodeTestFixture(output)) { }
        protected Async_execution_dont_block_aggregate(NodeTestFixture fixture) : base(fixture.Add(new BalloonDomainConfiguration())) { }

        [Fact]
        public async Task When_executing_async_command_Then_sync_commands_are_waiting_for_the_result()
        {
            var aggregateId = Guid.NewGuid()
                                  .ToString();
            var asyncCommand = new PlanTitleChangeCommand(Guid.NewGuid()
                                                              .ToString(),
                                                          43,
                                                          TimeSpan.FromSeconds(2));
            var syncCommand = new WriteTitleCommand(42, aggregateId);

            //dont wait for async comand intentionally
#pragma warning disable 4014
            Node.Prepare(asyncCommand)
                .Expect<BalloonTitleChanged>()
                .Execute();
#pragma warning restore 4014

            //aggregate should wait for the async command finish before executing sync command
            await Node.Prepare(syncCommand)
                      .Expect<BalloonTitleChanged>()
                      .Execute();

            var sampleAggregate = await Node.LoadAggregateByActor<Balloon>(syncCommand.AggregateId);

            Assert.Equal(syncCommand.Parameter.ToString(), sampleAggregate.Title);
        }
    }
}