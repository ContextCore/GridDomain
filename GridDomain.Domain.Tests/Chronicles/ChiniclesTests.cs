using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GridDomain.Common;
using GridDomain.CQRS.Messaging;
using GridDomain.EventSourcing;
using GridDomain.Node.EventChronicles;
using GridDomain.Tests.Framework.Configuration;
using GridDomain.Tests.Framework.Persistence;
using GridDomain.Tests.SampleDomain;
using GridDomain.Tests.SampleDomain.Events;
using GridDomain.Tests.SampleDomain.ProjectionBuilders;
using GridDomain.Tests.SynchroniousCommandExecute;
using NUnit.Framework;
using GridDomain.CQRS.Messaging.MessageRouting;

namespace GridDomain.Tests.Chronicles
{
    [TestFixture]
    public class Given_persisted_events_replay : SampleDomainCommandExecutionTests
    {

        class AggregateCreatedProjectionBuilder_Test : AggregateCreatedProjectionBuilder
        {
            public override void Handle(SampleAggregateCreatedEvent msg)
            {
                base.Handle(msg);
                msg.History.HandlerName = this.GetType().Name;
            }
        }

        class AggregateChangedProjectionBuilder_Test : AggregateChangedProjectionBuilder
        {
            public override void Handle(SampleAggregateChangedEvent msg)
            {
                base.Handle(msg);
                msg.History.HandlerName = this.GetType().Name;
            }
        }

        private SampleAggregateCreatedEvent _sampleAggregateCreatedEvent;
        private SampleAggregateChangedEvent _sampleAggregateChangedEvent;

        [TestFixtureSetUp]
        public void Given_persisted_domain_events_when_replaying_it_for_existing_aggregate_id()
        {
            var aggregateId = Guid.NewGuid();
            _sampleAggregateCreatedEvent = new SampleAggregateCreatedEvent("123", aggregateId);
            _sampleAggregateChangedEvent = new SampleAggregateChangedEvent("234", aggregateId);

            SaveInJournal<SampleAggregate>(aggregateId, _sampleAggregateCreatedEvent, _sampleAggregateChangedEvent);


            var chronicle = new AkkaEventsChronicle(new AutoTestAkkaConfiguration());
            chronicle.Router.RegisterHandler<SampleAggregateCreatedEvent, AggregateCreatedProjectionBuilder_Test>(e => e.SourceId);
            chronicle.Router.RegisterHandler<SampleAggregateChangedEvent, AggregateChangedProjectionBuilder_Test>(e => e.SourceId);
            chronicle.Replay<SampleAggregate>(aggregateId);
        }

        [Then]
        public void Chronicle_aggregate_created_subscribers_received_it()
        {
            Assert.AreEqual(typeof(AggregateCreatedProjectionBuilder_Test).Name,
                _sampleAggregateCreatedEvent.History.HandlerName);
        }

        [Then]
        public void Chronicle_aggregate_changed_subscribers_received_it()
        {
            Assert.AreEqual(typeof(AggregateChangedProjectionBuilder_Test).Name,
                _sampleAggregateChangedEvent.History.HandlerName);
        }

        [Then]
        public void Created_event_was_processed_only_by_chronicle_subscribers()
        {
            Assert.AreEqual(1, _sampleAggregateChangedEvent.History.SequenceNumber);
        }

        [Then]
        public void Changed_event_was_processed_only_by_chronicle_subscribers()
        {
            Assert.AreEqual(1, _sampleAggregateCreatedEvent.History.SequenceNumber);
        }
    }
}
