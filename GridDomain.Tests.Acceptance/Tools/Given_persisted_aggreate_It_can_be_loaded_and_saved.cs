using System;
using System.Threading.Tasks;
using GridDomain.EventSourcing.Adapters;
using GridDomain.Tests.Unit.BalloonDomain;
using GridDomain.Tools.Repositories.AggregateRepositories;
using GridDomain.Tools.Repositories.EventRepositories;
using Xunit;

namespace GridDomain.Tests.Acceptance.Tools
{
    public class Given_persisted_aggreate_It_can_be_loaded_and_saved
    {
        [Fact]
        public async Task Given_persisted_aggreate()
        {
            var aggregateId = Guid.NewGuid();
            var agregateValue = "initial";
            var aggregate = new Balloon(aggregateId, agregateValue);

            using (
                var repo =
                    new AggregateRepository(ActorSystemJournalRepository.New(new AcceptanceAutoTestAkkaConfiguration(),
                                                                             new EventsAdaptersCatalog())))
            {
                await repo.Save(aggregate);
            }

            using (
                var repo =
                    new AggregateRepository(ActorSystemJournalRepository.New(new AcceptanceAutoTestAkkaConfiguration(),
                                                                             new EventsAdaptersCatalog())))
            {
                aggregate = await repo.LoadAggregate<Balloon>(aggregate.Id);
            }

            //Aggregate_has_correct_id()
            Assert.Equal(aggregateId, aggregate.Id);
            //Aggregate_has_state_from_changed_event()
            Assert.Equal(agregateValue, aggregate.Title);
        }
    }
}