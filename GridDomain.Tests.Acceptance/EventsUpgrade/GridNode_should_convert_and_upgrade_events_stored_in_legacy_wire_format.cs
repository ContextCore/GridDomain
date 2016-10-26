using System;
using System.Linq;
using GridDomain.Common;
using GridDomain.EventSourcing;
using GridDomain.Node.Configuration.Akka.Hocon;
using GridDomain.Tests.Acceptance.EventsUpgrade.SampleDomain;
using GridDomain.Tests.CommandsExecution;
using GridDomain.Tools.Persistence.SqlPersistence;
using GridDomain.Tools.Repositories;
using NUnit.Framework;

namespace GridDomain.Tests.Acceptance.EventsUpgrade
{
    [TestFixture]
    class GridNode_should_convert_and_upgrade_events_stored_in_legacy_wire_format : SampleDomainCommandExecutionTests
    {
        private object[] _loadedEvents;
        private static readonly RawSqlAkkaPersistenceRepository RawDataRepository = new RawSqlAkkaPersistenceRepository(AkkaCfg.Persistence.JournalConnectionString);

        [OneTimeSetUp]
        public void When_wire_stored_events_loaded_and_saved_back()
        {
            GridNode.DomainEventsSerializer.Register(new BookOrderAdapter());

            var orderA = new BookOrder_V1("A");
            var orderB = new BookOrder_V1("B");
            var id = Guid.NewGuid();

            var events = new DomainEvent[]
            {
                new EventA(id, orderA),
                new EventB(id, orderB)
            };

            SaveWithLegacyWire(events);

            _loadedEvents = LoadFromJournal("testId", 2).ToArray();

            SaveToJournal(_loadedEvents);

        }

        [Test]
        public void Then_it_should_be_serialized_to_json()
        {
            var convertedItems = RawDataRepository.Load("testId");
            var restoredFromJson = convertedItems.Select(i => GridNode.DomainEventsSerializer.FromBinary(i.Payload,Type.GetType(i.Manifest)));

            CollectionAssert.AllItemsAreNotNull(restoredFromJson);
        }

        [Test]
        public void Then_it_should_be_upgraded_by_json_custom_adapter()
        {
            var expectA = _loadedEvents.OfType<EventA>().FirstOrDefault();
            var expectB = _loadedEvents.OfType<EventB>().FirstOrDefault();
            
            Assert.IsInstanceOf<BookOrder_V2>(expectA?.Order);
            Assert.IsInstanceOf<BookOrder_V2>(expectB?.Order);
        }

        private static void SaveWithLegacyWire(DomainEvent[] events)
        {
            var legacySerializer = new LegacyWireSerializer();
            long seqNum = 0;
            var journalEntries = events.Select(e => new JournalItem("testId",
                seqNum++,
                false,
                e.GetType().AssemblyQualifiedShortName(),
                DateTime.Now,
                "",
                legacySerializer.Serialize(e)))
                .ToArray();

            RawDataRepository.Save("testId", journalEntries);
        }
    }
}