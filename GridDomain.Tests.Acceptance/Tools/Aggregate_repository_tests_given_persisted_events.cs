using System;
using System.Collections;
using GridDomain.EventSourcing;
using GridDomain.Node.AkkaMessaging;
using GridDomain.Tests.Framework.Configuration;
using GridDomain.Tests.SampleDomain;
using GridDomain.Tests.SampleDomain.Events;
using GridDomain.Tests.Tools;
using GridDomain.Tools.Persistence;
using GridDomain.Tools.Repositories;
using NUnit.Framework;

namespace GridDomain.Tests.Acceptance.Tools
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
            _sourceId = Guid.NewGuid();
            _createdEvent = new SampleAggregateCreatedEvent("initial value", _sourceId);
            _changedEvent = new SampleAggregateChangedEvent("changed value", _sourceId);
           
            string persistenceId = AggregateActorName.New<SampleAggregate>(_sourceId).ToString();
            eventRepo.Save(persistenceId, _createdEvent, _changedEvent);
            _aggregate = aggrRepo.LoadAggregate<SampleAggregate>(_sourceId);

            Assert.AreEqual(_sourceId, _aggregate.Id);
            Assert.AreEqual(_changedEvent.Value, _aggregate.Value);
        }
        private static readonly AutoTestAkkaConfiguration AutoTestAkkaConfiguration = new AutoTestAkkaConfiguration();
        private static readonly string AkkaWriteDbConnectionString = AutoTestAkkaConfiguration.Persistence.JournalConnectionString;

        static readonly object[] EventRepositories =
        {
            new object[] { ActorSystemEventRepository.New(AutoTestAkkaConfiguration), TestRepository.NewPersistent()},
            new object[] { ActorSystemEventRepository.New(AutoTestAkkaConfiguration), AggregateRepository.New(AkkaWriteDbConnectionString)},
            new object[] { DomainEventsRepository.New(AkkaWriteDbConnectionString),   AggregateRepository.New(AkkaWriteDbConnectionString)},
            new object[] { DomainEventsRepository.New(AkkaWriteDbConnectionString),   TestRepository.NewPersistent()},
        };
    }
}