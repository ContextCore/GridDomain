using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GridDomain.Common;
using GridDomain.CQRS.Messaging;
using GridDomain.EventSourcing;
using GridDomain.Tests.Framework.Persistence;
using GridDomain.Tests.SampleDomain;
using GridDomain.Tests.SampleDomain.Events;
using GridDomain.Tests.SynchroniousCommandExecute;
using NUnit.Framework;

namespace GridDomain.Tests.Chronicles
{
    [TestFixture]
    public class Given_persisted_events : SampleDomainCommandExecutionTests
    {


        [TestFixtureSetUp]
        public void Given_persisted_domain_events_when_replaying_it_for_existing_aggregate_id()
        {
            var aggregateId = Guid.NewGuid();
            var events = new DomainEvent[]
            {
                new SampleAggregateCreatedEvent("123", aggregateId),
                new SampleAggregateChangedEvent("234", aggregateId)
            };

            SaveInJournal<SampleAggregate>(aggregateId, events);
        }

        public Given_persisted_events(bool inMemory) : base(inMemory)
        {
        }




        protected override TimeSpan Timeout { get; }
        protected override IContainerConfiguration CreateConfiguration()
        {
            throw new NotImplementedException();
        }

        protected override IMessageRouteMap CreateMap()
        {
            throw new NotImplementedException();
        }
    }
}
