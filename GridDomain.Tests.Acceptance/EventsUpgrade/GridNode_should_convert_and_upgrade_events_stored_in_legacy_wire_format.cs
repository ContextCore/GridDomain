using System;
using System.Linq;
using Akka.Actor;
using GridDomain.Common;
using GridDomain.EventSourcing;
using GridDomain.Node;
using GridDomain.Node.Configuration.Akka.Hocon;
using GridDomain.Tests.Acceptance.EventsUpgrade.SampleDomain;
using GridDomain.Tests.CommandsExecution;
using GridDomain.Tools.Persistence.SqlPersistence;
using GridDomain.Tools.Repositories;
using GridDomain.Tools.Repositories.RawDataRepositories;
using NUnit.Framework;

namespace GridDomain.Tests.Acceptance.EventsUpgrade
{
    [TestFixture]
    class GridNode_should_convert_and_upgrade_events_stored_in_legacy_wire_format : SampleDomainCommandExecutionTests
    {
        private object[] _loadedEvents;
        private static readonly RawJournalRepository RawDataRepository = new RawJournalRepository(AkkaCfg.Persistence.JournalConnectionString);
        private string _persistenceId;
        protected override bool ClearDataOnStart { get; } = true;

        public GridNode_should_convert_and_upgrade_events_stored_in_legacy_wire_format():base(false)
        {
            
        }

        [OneTimeSetUp]
        public void When_wire_stored_events_loaded_and_saved_back()
        {
            GridNode.EventsAdaptersCatalog.Register(new BookOrderAdapter());
            
            var orderA = new BookOrder_V1("A");
            var orderB = new BookOrder_V1("B");
            var id = Guid.NewGuid();

            var events = new DomainEvent[]
            {
                new EventA(id, orderA),
                new EventB(id, orderB)
            };

            _persistenceId = "testId";
            SaveWithLegacyWire(_persistenceId, events);

            _loadedEvents = LoadFromJournal(_persistenceId, 2).ToArray();

            SaveToJournal(_loadedEvents);

        }

        [Test]
        public void Then_it_should_be_serialized_to_json()
        {
            var convertedItems = RawDataRepository.Load(_persistenceId);
            var serializer = new WireJsonSerializer();
            var restoredFromJson = convertedItems.Select(i => serializer.FromBinary(i.Payload,Type.GetType(i.Manifest)));

            CollectionAssert.AllItemsAreNotNull(restoredFromJson);
        }

        public static void SaveWithLegacyWire(string testid, DomainEvent[] events)
        {
            var legacySerializer = new LegacyWireSerializer();
            long seqNum = 0;
            var journalEntries = events.Select(e => new JournalItem(testid,
                seqNum++,
                false,
                e.GetType().AssemblyQualifiedShortName(),
                DateTime.Now,
                "",
                legacySerializer.Serialize(e)))
                .ToArray();

            RawDataRepository.Save(testid, journalEntries);
        }
    }
}