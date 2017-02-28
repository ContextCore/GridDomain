using System;
using System.Linq;
using System.Threading.Tasks;
using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.Tests.Framework;
using GridDomain.Tests.XUnit.CommandsExecution;
using GridDomain.Tests.XUnit.SampleDomain.Commands;
using GridDomain.Tests.XUnit.SampleDomain.Events;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.XUnit.SyncProjection
{
    public class SynchronizedProjectionBuildersTests : SampleDomainCommandExecutionTests
    {
        public SynchronizedProjectionBuildersTests(ITestOutputHelper output) : base(output) {}

        [Fact]
        public async Task When_execute_many_commands_for_create_and_update()
        {
            var totalAggregates = 5;
            var eachAggregateChanges = 5;

            var createCommands =
                Enumerable.Range(0, totalAggregates)
                          .Select(r => new CreateSampleAggregateCommand(0, Guid.NewGuid()))
                          .ToArray();

            var aggregateIds = createCommands.Select(c => c.AggregateId).ToArray();

            var updateCommands =
                createCommands.SelectMany(
                    c =>
                        Enumerable.Range(0, eachAggregateChanges)
                                  .Select(r => new ChangeSampleAggregateCommand(r, aggregateIds.RandomElement())));

            createCommands.Shuffle();

            var createWaiters = createCommands.Select(c => Node.Prepare(c).Expect<SampleAggregateCreatedEvent>().Execute());

            var updateWaiters = updateCommands.Select(c => Node.Prepare(c).Expect<SampleAggregateChangedEvent>().Execute());

            var allResults = await Task.WhenAll(createWaiters.Union(updateWaiters));

            var eventsPerAggregate =
                allResults.SelectMany(r => r.All)
                          .Cast<IMessageMetadataEnvelop>()
                          .Select(m => (IHaveProcessingHistory) m.Message)
                          .GroupBy(e => e.SourceId)
                          .ToDictionary(g => g.Key, g => g.OrderBy(i => i.History.ElapsedTicksFromAppStart).ToArray());

            //all change events for one aggregate should be processed synchroniously, one-by-one, according to their 
            //sequence numbers

            foreach (var oneAggregateEvents in eventsPerAggregate.Values)
            {
                oneAggregateEvents.IsIncreasing(h => h.History.SequenceNumber);
            }
        }
    }
}