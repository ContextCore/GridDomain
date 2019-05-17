using System;

namespace GridDomain.Aggregates.Abstractions
{
    /// <summary>
    ///     Represents an event message.
    /// </summary>
    public interface ISourcedEvent
    {
        /// <summary>
        ///     Gets the identifier of the source originating the event.
        /// </summary>
        IAggregateAddress Source { get; }
        DateTimeOffset Occured { get; }
    }
}