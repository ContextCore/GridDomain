using System;
using System.Linq;
using System.Threading.Tasks;
using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.Node.Configuration;
using GridDomain.Tests.Common;
using GridDomain.Tests.Unit.BalloonDomain.Commands;
using GridDomain.Tests.Unit.BalloonDomain.Configuration;
using GridDomain.Tests.Unit.BalloonDomain.Events;
using GridDomain.Tests.Unit.CommandsExecution;
using GridDomain.Tests.Unit.CommandsExecution.ExecutionWithErrors;
using Serilog.Events;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.Unit.SyncProjection
{
    public class SynchronizedProjectionBuildersTests : NodeTestKit
    {
        public SynchronizedProjectionBuildersTests(ITestOutputHelper output) :
            this(new NodeTestFixture(output).Add(new BalloonDomainConfiguration())
                                            .PrintSystemConfig()
                                            .LogLevel(LogEventLevel.Verbose)){ }

        protected SynchronizedProjectionBuildersTests(NodeTestFixture fixture) : base(fixture) { }

        [Fact]
        public async Task When_execute_many_commands_for_create_and_update()
        {
            var totalAggregates = 5;
            var eachAggregateChanges = 5;

            var createCommands =
                Enumerable.Range(0, totalAggregates)
                          .Select(r => new InflateNewBallonCommand(0,
                                                                   Guid.NewGuid()
                                                                       .ToString()))
                          .ToArray();

            var aggregateIds = createCommands.Select(c => c.AggregateId)
                                             .ToArray();

            var updateCommands =
                createCommands.SelectMany(
                                          c =>
                                              Enumerable.Range(0, eachAggregateChanges)
                                                        .Select(r => new WriteTitleCommand(r, aggregateIds.RandomElement())));

            createCommands.Shuffle();

            var createWaiters = createCommands.Select(async c => (IWaitResult) await Node.Prepare(c)
                                                                                         .Expect<BalloonCreated>()
                                                                                         .Execute());

            var updateWaiters = updateCommands.Select(async c => (IWaitResult) await Node.Prepare(c)
                                                                                         .Expect<BalloonTitleChanged>()
                                                                                         .Execute());

            var allResults = await Task.WhenAll(createWaiters.Concat(updateWaiters));

            var eventsPerAggregate =
                allResults.SelectMany(r => r.All)
                          .Cast<IMessageMetadataEnvelop>()
                          .Select(m => (IHaveProcessingHistory) m.Message)
                          .GroupBy(e => e.SourceId)
                          .ToDictionary(g => g.Key,
                                        g => g.OrderBy(i => i.History.ElapsedTicksFromAppStart)
                                              .ToArray());

            //all change events for one aggregate should be processed synchroniously, one-by-one, according to their 
            //sequence numbers

            foreach (var oneAggregateEvents in eventsPerAggregate.Values)
                oneAggregateEvents.IsIncreasing(h => h.History.SequenceNumber);
        }
    }
}