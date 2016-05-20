using System;
using CommonDomain.Core;

namespace GridDomain.EventSourcing
{
    /// <summary>
    ///     Represents an event message that has a version
    /// </summary>
    public interface IVersionedEvent : ISourcedEvent
    {
        /// <summary>
        ///     Gets the version of event. Should be increased after each event class change
        /// </summary>
        int Version { get; }
    }

    public interface IAggregateRepository
    {
        T Load<T>(Guid id) where T : AggregateBase;
        void Save(AggregateBase aggregate, Guid commitId);
    }
}