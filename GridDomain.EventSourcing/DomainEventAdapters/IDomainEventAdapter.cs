using System.Collections.Generic;

namespace GridDomain.EventSourcing.VersionedTypeSerialization
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
    public interface IEventAdapter
    {
        EventAdapterDescriptor Descriptor { get; }
        IEnumerable<object> Convert(object evt);
    }

    public interface IDomainEventAdapter<TFrom,TTo>: IEventAdapter 
                                                    where TFrom:DomainEvent 
                                                    where TTo: DomainEvent
    {
        IEnumerable<TTo> ConvertEvent(TFrom evt);
    }
}