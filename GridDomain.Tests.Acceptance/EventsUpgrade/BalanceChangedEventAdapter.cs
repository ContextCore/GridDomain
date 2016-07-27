using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Akka.Actor;
using Akka.Persistence.Journal;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.VersionedTypeSerialization;

namespace GridDomain.Tests.Acceptance.EventsUpgrade
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

        public BalanceChangedEventAdapter(ExtendedActorSystem system)
        {
        }

        public BalanceChangedEventAdapter()
        {
            
        }
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
            var type = Type.GetType(manifest);
            var factType = evt.GetType();
            return EventSequence.Single(evt);
        }
    }
}