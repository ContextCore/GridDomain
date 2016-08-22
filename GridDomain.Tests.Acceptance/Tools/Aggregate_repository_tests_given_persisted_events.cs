using System;
using GridDomain.EventSourcing;
using GridDomain.Node.AkkaMessaging;
using GridDomain.Tests.Framework.Configuration;
using GridDomain.Tests.SampleDomain;
using GridDomain.Tests.SampleDomain.Events;
using GridDomain.Tests.Tools;
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

        [TestFixtureSetUp]
        public void Given_only_aggregate_events_persisted_it_can_be_loaded()
        {
            _sourceId = Guid.NewGuid();
            _createdEvent = new SampleAggregateCreatedEvent("initial value", _sourceId);
            _changedEvent = new SampleAggregateChangedEvent("changed value", _sourceId);

            using (var eventsRepo = (IRepository<DomainEvent>) ActorSystemEventRepository.New(new AutoTestAkkaConfiguration()))
            {
                string persistenceId = AggregateActorName.New<SampleAggregate>(_sourceId).ToString();
                eventsRepo.Save(persistenceId, _createdEvent, _changedEvent);
            }

            using (var repo = TestRepository.NewPersistent())
            {
                _aggregate = repo.LoadAggregate<SampleAggregate>(_sourceId);
            }
        }

        [Then]
        public void Aggregate_has_correct_id()
        {
            Assert.AreEqual(_sourceId, _aggregate.Id);
        }


        [Then]
        public void Aggregate_has_state_from_changed_event()
        {
            Assert.AreEqual(_changedEvent.Value, _aggregate.Value);
        }

    }
}