using System;
using System.Linq;
using GridDomain.EventSourcing;
using GridDomain.Node;
using GridDomain.Tests.Acceptance.EventsUpgrade.SampleDomain;
using GridDomain.Tests.CommandsExecution;
using NUnit.Framework;

namespace GridDomain.Tests.Acceptance.EventsUpgrade
{
    [TestFixture]
    class GridNode_should_upgrade_objects_in_domain_events_after_save_load_by_journal : SampleDomainCommandExecutionTests
    {
        protected override bool ClearDataOnStart { get; } = true;

        public GridNode_should_upgrade_objects_in_domain_events_after_save_load_by_journal():base(false)
        {
            
        }

        [Test]
        public void GridNode_updates_objects_in_events_by_adapter()
        {
            DomainEventsJsonSerializer.Register(new BookOrderAdapter());
            
            var orderA = new BookOrder_V1("A");
            var orderB = new BookOrder_V1("B");
            var id = Guid.NewGuid();

            var events = new DomainEvent[]
            {
                new EventA(id, orderA),
                new EventB(id, orderB)
            };

            SaveToJournal(events);

            var loadedEvents = LoadFromJournal("testId", 2).ToArray();
            var expectA = loadedEvents.OfType<EventA>().FirstOrDefault();
            var expectB = loadedEvents.OfType<EventB>().FirstOrDefault();

            Assert.IsInstanceOf<BookOrder_V2>(expectA?.Order);
            Assert.IsInstanceOf<BookOrder_V2>(expectB?.Order);
         }

    }

   
}