using System;
using System.Linq;
using System.Threading.Tasks;
using Akka.Actor;
using GridDomain.EventSourcing;
using GridDomain.Tests.Acceptance.XUnit.EventsUpgrade.SampleDomain;
using GridDomain.Tests.Framework;
using GridDomain.Tests.XUnit;
using GridDomain.Tests.XUnit.CommandsExecution;
using GridDomain.Tests.XUnit.SampleDomain;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.Acceptance.XUnit.EventsUpgrade
{
   
    public class GridNode_should_upgrade_objects_in_domain_events_after_save_load_by_journal: NodeTestKit
    {

        class ObjectsUpgradeFixture : NodeTestFixture
        {
            protected override void OnNodeCreated()
            {
                Node.EventsAdaptersCatalog.Register(new BookOrderAdapter());
                InMemory = false;
            }
        }

        [Fact]
        public async Task GridNode_updates_objects_in_events_by_adapter()
        {
            var orderA = new BookOrder_V1("A");
            var orderB = new BookOrder_V1("B");
            var id = Guid.NewGuid();

            var events = new object[]
            {
                new EventA(id, orderA),
                new EventB(id, orderB)
            };

            await Sys.SaveToJournal(id.ToString(), events);

            var loadedEvents = await Sys.LoadFromJournal(id.ToString());
            var expectA = loadedEvents.OfType<EventA>().FirstOrDefault();
            var expectB = loadedEvents.OfType<EventB>().FirstOrDefault();

            Assert.IsAssignableFrom<BookOrder_V2>(expectA?.Order);
            Assert.IsAssignableFrom<BookOrder_V2>(expectB?.Order);
         }

        public GridNode_should_upgrade_objects_in_domain_events_after_save_load_by_journal
            (ITestOutputHelper output) : base(output, new ObjectsUpgradeFixture()) {}
    }

   
}