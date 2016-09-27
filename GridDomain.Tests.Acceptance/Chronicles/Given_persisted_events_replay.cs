using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using GridDomain.CQRS.Messaging.MessageRouting;
using GridDomain.Node.EventChronicles;
using GridDomain.Tests.CommandsExecution;
using GridDomain.Tests.Framework.Configuration;
using GridDomain.Tests.SampleDomain;
using GridDomain.Tests.SampleDomain.Events;
using GridDomain.Tests.SampleDomain.ProjectionBuilders;
using NUnit.Framework;

namespace GridDomain.Tests.Acceptance.Chronicles
{
    //TODO: replace with messaging

    [TestFixture]
    public class Given_persisted_events_replay : SampleDomainCommandExecutionTests
    {
        public Given_persisted_events_replay():base(false)
        {
            
        }

        class AggregateCreatedProjectionBuilder_Test : AggregateCreatedProjectionBuilder
        {
            public override void Handle(SampleAggregateCreatedEvent msg)
            {
                base.Handle(msg);
               EventsReplayInfoHolder.ProcessedMessages[msg.SourceId].Add(new ProcessedHistory
               {
                   SequenceNumber = msg.History.SequenceNumber,
                   HandlerName = this.GetType().Name
               });
            }
        }

        class AggregateChangedProjectionBuilder_Test : AggregateChangedProjectionBuilder
        {
            public override void Handle(SampleAggregateChangedEvent msg)
            {
                base.Handle(msg);
                EventsReplayInfoHolder.ProcessedMessages[msg.SourceId].Add(new ProcessedHistory
                {
                    SequenceNumber = msg.History.SequenceNumber,
                    HandlerName = this.GetType().Name
                });
            }
        }

        private SampleAggregateCreatedEvent _sampleAggregateCreatedEvent;
        private SampleAggregateChangedEvent _sampleAggregateChangedEvent;

        [OneTimeSetUp]
        public void Given_persisted_domain_events_when_replaying_it_for_existing_aggregate_id()
        {
            var aggregateId = Guid.NewGuid();
            _sampleAggregateCreatedEvent = new SampleAggregateCreatedEvent("123", aggregateId);
            _sampleAggregateChangedEvent = new SampleAggregateChangedEvent("234", aggregateId);

            SaveInJournal<SampleAggregate>(aggregateId, _sampleAggregateCreatedEvent, _sampleAggregateChangedEvent);

            EventsReplayInfoHolder.ProcessedMessages[aggregateId] = new List<ProcessedHistory>();

            var chronicle = new AkkaEventsChronicle(new AutoTestAkkaConfiguration());
            chronicle.Router.RegisterHandler<SampleAggregateCreatedEvent, AggregateCreatedProjectionBuilder_Test>(e => e.SourceId);
            chronicle.Router.RegisterHandler<SampleAggregateChangedEvent, AggregateChangedProjectionBuilder_Test>(e => e.SourceId);
            chronicle.Replay<SampleAggregate>(aggregateId);
            Thread.Sleep(2000);
        }

        [Test]
        public void When_replaying_not_existing_id_no_exception_is_thrown()
        {
            var aggregateId = Guid.NewGuid();
            var chronicle = new AkkaEventsChronicle(new AutoTestAkkaConfiguration());
            chronicle.Replay<SampleAggregate>(aggregateId);
        }

        [Then]
        public void Chronicle_aggregate_created_subscribers_received_it()
        {
            Assert.AreEqual(typeof(AggregateCreatedProjectionBuilder_Test).Name,
                  EventsReplayInfoHolder.ProcessedMessages[_sampleAggregateCreatedEvent.SourceId].First().HandlerName);
        }

        [Then]
        public void Chronicle_aggregate_changed_subscribers_received_it()
        {
            Assert.AreEqual(typeof(AggregateChangedProjectionBuilder_Test).Name,
                EventsReplayInfoHolder.ProcessedMessages[_sampleAggregateChangedEvent.SourceId].Skip(1).First().HandlerName);
        }

        [Then]
        public void Created_event_was_processed_only_by_chronicle_subscribers()
        {
            Assert.AreEqual(1, EventsReplayInfoHolder.ProcessedMessages[_sampleAggregateCreatedEvent.SourceId].First().SequenceNumber);
        }

        [Then]
        public void Changed_event_was_processed_only_by_chronicle_subscribers()
        {
            Assert.AreEqual(1, EventsReplayInfoHolder.ProcessedMessages[_sampleAggregateChangedEvent.SourceId].Skip(1).First().SequenceNumber);
        }
    }
}
