using System;
using System.Threading.Tasks;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.Adapters;
using GridDomain.Node.AkkaMessaging;
using GridDomain.Tests.Framework.Configuration;
using GridDomain.Tests.XUnit.SampleDomain;
using GridDomain.Tests.XUnit.SampleDomain.Events;
using GridDomain.Tools.Repositories;
using GridDomain.Tools.Repositories.AggregateRepositories;
using GridDomain.Tools.Repositories.EventRepositories;
using Xunit;

namespace GridDomain.Tests.Acceptance.XUnit.Tools
{
    public class Aggregate_repository_tests_given_persisted_events
    {
        private SampleAggregate _aggregate;
        private Guid _sourceId;
        private SampleAggregateCreatedEvent _createdEvent;
        private SampleAggregateChangedEvent _changedEvent;

        [Theory]
        [MemberData(nameof(EventRepositories))]
        public async Task Given_only_aggregate_events_persisted_it_can_be_loaded(IRepository<DomainEvent> eventRepo, AggregateRepository aggrRepo)
        {
            try
            {
                _sourceId = Guid.NewGuid();
                _createdEvent = new SampleAggregateCreatedEvent("initial value", _sourceId);
                _changedEvent = new SampleAggregateChangedEvent("changed value", _sourceId);

                string persistenceId = AggregateActorName.New<SampleAggregate>(_sourceId).ToString();
                await eventRepo.Save(persistenceId, _createdEvent, _changedEvent);
                _aggregate = await aggrRepo.LoadAggregate<SampleAggregate>(_sourceId);

                Assert.Equal(_sourceId, _aggregate.Id);
                Assert.Equal(_changedEvent.Value, _aggregate.Value);
            }
            finally
            {
                eventRepo.Dispose();
                aggrRepo.Dispose();
            }
        }
        private static readonly AutoTestAkkaConfiguration AutoTestAkkaConfiguration = new AutoTestAkkaConfiguration();
        private static readonly string AkkaWriteDbConnectionString = AutoTestAkkaConfiguration.Persistence.JournalConnectionString;

        public static readonly object[] EventRepositories =
        {
            new object[] { ActorSystemEventRepository.New(AutoTestAkkaConfiguration, new EventsAdaptersCatalog()), new AggregateRepository(ActorSystemEventRepository.New(new AutoTestAkkaConfiguration(), new EventsAdaptersCatalog()))},
            new object[] { ActorSystemEventRepository.New(AutoTestAkkaConfiguration, new EventsAdaptersCatalog()), AggregateRepository.New(AkkaWriteDbConnectionString)},
            new object[] { DomainEventsRepository.New(AkkaWriteDbConnectionString),   AggregateRepository.New(AkkaWriteDbConnectionString)},
            new object[] { DomainEventsRepository.New(AkkaWriteDbConnectionString),   new AggregateRepository(ActorSystemEventRepository.New(new AutoTestAkkaConfiguration(), new EventsAdaptersCatalog()))},
        };
    }
}