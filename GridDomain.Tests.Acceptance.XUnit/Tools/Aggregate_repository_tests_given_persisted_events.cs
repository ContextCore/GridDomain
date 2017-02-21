using System;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.Adapters;
using GridDomain.Node.AkkaMessaging;
using GridDomain.Tests.Framework.Configuration;
using GridDomain.Tools.Repositories;
using GridDomain.Tools.Repositories.AggregateRepositories;
using GridDomain.Tools.Repositories.EventRepositories;

namespace GridDomain.Tests.Acceptance.XUnit.Tools
{
    [TestFixture]
    class Aggregate_repository_tests_given_persisted_events
    {
        private SampleAggregate _aggregate;
        private Guid _sourceId;
        private SampleAggregateCreatedEvent _createdEvent;
        private SampleAggregateChangedEvent _changedEvent;

        [Then,TestCaseSource(nameof(EventRepositories))]
        public void Given_only_aggregate_events_persisted_it_can_be_loaded(IRepository<DomainEvent> eventRepo, AggregateRepository aggrRepo)
        {
            try
            {
                _sourceId = Guid.NewGuid();
                _createdEvent = new SampleAggregateCreatedEvent("initial value", _sourceId);
                _changedEvent = new SampleAggregateChangedEvent("changed value", _sourceId);

                string persistenceId = AggregateActorName.New<SampleAggregate>(_sourceId).ToString();
                eventRepo.Save(persistenceId, _createdEvent, _changedEvent);
                _aggregate = aggrRepo.LoadAggregate<SampleAggregate>(_sourceId);

                Assert.AreEqual(_sourceId, _aggregate.Id);
                Assert.AreEqual(_changedEvent.Value, _aggregate.Value);
            }
            finally
            {
                eventRepo.Dispose();
                aggrRepo.Dispose();
            }
        }
        private static readonly AutoTestAkkaConfiguration AutoTestAkkaConfiguration = new AutoTestAkkaConfiguration();
        private static readonly string AkkaWriteDbConnectionString = AutoTestAkkaConfiguration.Persistence.JournalConnectionString;

        static readonly object[] EventRepositories =
        {
            new object[] { ActorSystemEventRepository.New(AutoTestAkkaConfiguration, new EventsAdaptersCatalog()), new AggregateRepository(ActorSystemEventRepository.New(new AutoTestAkkaConfiguration(), new EventsAdaptersCatalog()))},
            new object[] { ActorSystemEventRepository.New(AutoTestAkkaConfiguration, new EventsAdaptersCatalog()), AggregateRepository.New(AkkaWriteDbConnectionString)},
            new object[] { DomainEventsRepository.New(AkkaWriteDbConnectionString),   AggregateRepository.New(AkkaWriteDbConnectionString)},
            new object[] { DomainEventsRepository.New(AkkaWriteDbConnectionString),   new AggregateRepository(ActorSystemEventRepository.New(new AutoTestAkkaConfiguration(), new EventsAdaptersCatalog()))},
        };
    }
}