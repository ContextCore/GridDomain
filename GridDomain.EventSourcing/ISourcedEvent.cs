using System;

namespace GridDomain.EventSourcing
{

    public interface IHaveId
    {
        Guid Id { get; }
    }

    /// <summary>
    ///     Represents an event message.
    /// </summary>
    public interface ISourcedEvent
    {
        /// <summary>
        ///     Gets the identifier of the source originating the event.
        /// </summary>
        Guid SourceId { get; }
        DateTime CreatedTime { get; }
    }
}