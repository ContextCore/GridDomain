using System;
using System.Linq;
using Akka.Actor;
using Akka.Persistence;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.Adapters;
using GridDomain.Node.Configuration.Akka.Hocon;
using GridDomain.Tests.CommandsExecution;
using GridDomain.Tests.SampleDomain;
using NUnit.Framework;

namespace GridDomain.Tests.EventsUpgrade
{
    [TestFixture]
    class GridNode_should_upgrade_objects_in_domain_events_after_save_load_by_journal : SampleDomainCommandExecutionTests
    {

        class EventA : DomainEvent
        {
            public IOrder Order { get; }

            public EventA(Guid sourceId, IOrder order) : base(sourceId)
            {
                Order = order;
            }
        }

        class EventB : DomainEvent
        {
            public IOrder Order { get; }

            public EventB(Guid sourceId, IOrder order) : base(sourceId)
            {
                Order = order;
            }
        }

        internal interface IOrder
        {
            string Number { get; }
        }

        class BookOrder_V1 : IOrder
        {
            public BookOrder_V1(string number)
            {
                Number = number;
            }
            public string Number { get; }
        }

        class BookOrder_V2 : IOrder
        {
            public BookOrder_V2(string number, int quantity)
            {
                Number = number;
                Quantity = quantity;
            }

            public string Number { get; }
            public int Quantity { get; }
        }

        class BookOrderAdapter : ObjectAdapter<BookOrder_V1, BookOrder_V2>
        {
            public override BookOrder_V2 Convert(BookOrder_V1 value)
            {
                return new BookOrder_V2(value.Number,0);
            }
        }

        protected override bool InMemory { get; } = false;

        [Test]
        public void GridNode_updates_objects_in_events_by_adapter()
        {
            GridNode.DomainEventsSerializer.Register(new BookOrderAdapter());
            var persistenceExtension = Persistence.Instance.Apply(GridNode.System);

            var settings = persistenceExtension.Settings;
            var journal = persistenceExtension.JournalFor(null);
            
            var orderA = new BookOrder_V1("A");
            var orderB = new BookOrder_V1("B");
            var id = Guid.NewGuid();
            var events = new DomainEvent[]
            {
                new EventA(id, orderA),
                new EventB(id, orderB)
            };

            int seqNumber=0;
            var envelop =
                events.Select(e => new Akka.Persistence.AtomicWrite(
                             new Persistent(e, seqNumber++, "testId", e.GetType()
                                                                      .AssemblyQualifiedShortName())))
                      .Cast<IPersistentEnvelope>()
                      .ToArray();

            var writeMsg = new WriteMessages(envelop, TestActor,1);

            journal.Ask<object>(writeMsg);//.Wait();

            var msg = ExpectMsg<object>();

            Assert.IsInstanceOf<WriteMessagesSuccessful>(msg);
            var loadMsg = new ReplayMessages(0,5,5,"testId",TestActor);

            journal.Tell(loadMsg);

            var confirmWriteA = ExpectMsg<WriteMessageSuccess>();
            var confirmWriteB = ExpectMsg<WriteMessageSuccess>();

            var expectA = ExpectMsg<ReplayedMessage>();
            var expectB = ExpectMsg<ReplayedMessage>();

            Assert.IsInstanceOf<BookOrder_V2>((expectA.Persistent.Payload as EventA)?.Order);
            Assert.IsInstanceOf<BookOrder_V2>((expectB.Persistent.Payload as EventB)?.Order);
         }
    }

   
}