using System.Collections.Generic;

namespace GridDomain.EventSourcing.Adapters
{
    /// <summary>
    ///     Adapter used to change domain events in terms of new version, e.g :
    ///     field add, rename, delete or type change.
    ///     Works on events after their deserialization.
    /// </summary>
    /// <typeparam name="TFrom"></typeparam>
    /// <typeparam name="TTo"></typeparam>
    public interface IDomainEventAdapter<TFrom, TTo> : IEventAdapter where TFrom : DomainEvent where TTo : DomainEvent
    {
        IEnumerable<TTo> ConvertEvent(TFrom evt);
    }

    public interface IObjectAdapter
    {
        object ConvertAny(object evt);
    }
}