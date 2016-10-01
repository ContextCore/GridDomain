using System.Collections.Generic;

namespace GridDomain.EventSourcing.Adapters
{
    /// <summary>
    /// Adapter used to change domain events in terms of new version, e.g : 
    /// field add, rename, delete or type change.  
    /// Works on events after their deserialization. 
    /// </summary>
    /// <typeparam name="TFrom"></typeparam>
    /// <typeparam name="TTo"></typeparam>
    public interface IDomainEventAdapter<TFrom,TTo>: IEventAdapter 
                                                    where TFrom:DomainEvent 
                                                    where TTo: DomainEvent
    {
        IEnumerable<TTo> ConvertEvent(TFrom evt);
    }

    /// <summary>
    /// Object adapter used for value object upgrade in domain events
    /// If value object is changed, you should create one object adaptor for this
    /// and none of domain event adapter.
    /// Works as part of deserialiation logic.
    /// Can be used on interface implementation or child classes change  
    /// </summary>
    /// <typeparam name="TFrom"></typeparam>
    /// <typeparam name="TTo"></typeparam>
    public interface IObjectAdapter<TFrom, TTo> : IObjectAdapter
    {
        TTo Convert(TFrom evt);
    }

    public interface IObjectAdapter
    {
        object Convert(object evt);
    }

}