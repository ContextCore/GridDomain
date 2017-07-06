using System;
using System.Configuration;
using System.Reflection;
using System.Threading.Tasks;
using Akka.Event;
using GridDomain.EventSourcing.Adapters;
using GridDomain.Node.Configuration.Akka;
using GridDomain.Tests.Common;
using GridDomain.Tests.Common.Configuration;
using GridDomain.Tests.Unit.BalloonDomain;
using GridDomain.Tools.Repositories.AggregateRepositories;
using GridDomain.Tools.Repositories.EventRepositories;
using Xunit;

namespace GridDomain.Tests.Acceptance.Tools
{
    public class AcceptanceAutoTestAkkaConfiguration : AkkaConfiguration
    {
        public AcceptanceAutoTestAkkaConfiguration(LogLevel verbosity = LogLevel.DebugLevel)
            : base(new AutoTestAkkaNetworkAddress(), GetConfig(), verbosity, typeof(LoggerActorDummy)) { }

        private static IAkkaDbConfiguration GetConfig()
        {
            var config = ConfigurationManager.OpenExeConfiguration(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath);
            var section = (WriteDbConfigSection)config.GetSection("WriteDb");
            return section?.ElementInformation.IsPresent == true ? (IAkkaDbConfiguration)section : new AutoTestAkkaDbConfiguration();
        }
    }

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