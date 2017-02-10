using System;
using System.Linq;
using System.Threading.Tasks;
using Akka.Actor;
using GridDomain.EventSourcing;
using GridDomain.Node;
using GridDomain.Tests.Acceptance.XUnit.EventsUpgrade.SampleDomain;
using GridDomain.Tests.Framework;
using GridDomain.Tests.XUnit;
using GridDomain.Tests.XUnit.CommandsExecution;
using GridDomain.Tests.XUnit.SampleDomain;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.Acceptance.XUnit.EventsUpgrade
{
    public class GridNode_should_upgrade_objects_in_domain_events_after_save_load_by_journal : NodeTestKit
    {
        [Fact]
        public async Task GridNode_updates_objects_in_events_by_adapter()
        {
            var id = Guid.NewGuid();
            var persistId = id.ToString();

            await Node.SaveToJournal(persistId, 
                                    new EventA(id, new BookOrder_V1("A")), 
                                    new EventB(id, new BookOrder_V1("B")));

            var loadedEvents = await Node.LoadFromJournal(persistId);
            var expectA = loadedEvents.OfType<EventA>()
                                      .FirstOrDefault();

            var expectB = loadedEvents.OfType<EventB>()
                                      .FirstOrDefault();

            Assert.IsAssignableFrom<BookOrder_V2>(expectA?.Order);
            Assert.IsAssignableFrom<BookOrder_V2>(expectB?.Order);
        }

        public GridNode_should_upgrade_objects_in_domain_events_after_save_load_by_journal(ITestOutputHelper output)
            : base(output, new ObjectsUpgradeFixture() {InMemory = false}) {}

        class ObjectsUpgradeFixture : NodeTestFixture
        {
            protected override void OnNodeCreated()
            {
                Node.EventsAdaptersCatalog.Register(new BookOrderAdapter());
            }
        }
    }
}