using System.Threading;
using System.Threading.Tasks;
using Akka.Persistence.Journal;
using GridDomain.EventSourcing;

namespace GridDomain.Tests.Acceptance.EventsUpgrade.SampleDomain
{

    /// <summary>
    /// How to updata and event
    /// 1) Create a copy of event and add existing number in type by convention _V(N) where N is version
    /// for example BalanceAggregateCreatedEvent should be copied as BalanceAggregateCreatedEvent_V1.
    /// All existing persisted events must be convertible to versioned one by duck typing. 
    /// 2) Update existing event. 
    ///    Scenarios: 
    ///    a) Add field
    ///    b) Remove field
    ///    c) Change field name
    ///    d) Change field type
    ///    e) Rename event
    ///    f) Event splitting
    ///    
    /// 3) Create an event adapter from versioned type to new one 
    /// 4) Register event adapter 
    /// </summary>


    public class BalanceChangedEventAdapter : IEventAdapter
    {
        public string Manifest(object evt)
        {
            return evt.GetType().ToString() + "_V" + ((DomainEvent)evt).Version;
        }

        public object ToJournal(object evt)
        {
            return evt;
        }

        public IEventSequence FromJournal(object evt, string manifest)
        {
            return EventSequence.Single(evt);
        }

        //   <journal_identifier> {
        // event-adapters
        // {
        //     domainEventsUpgrade = " GridDomain.Tests.Acceptance.EventsUpgrade.SampleDomain.BalanceChangedEventAdapter, GridDmoin.Tests.Acceptance"
        // }
        //
        // event-adapter-bindings
        // {
        //     "GridDomain.EventSourcing.DomainEvent, GridDomain.EventSourcing" = domainEventsUpgrade
        // }
        // }
    }
}