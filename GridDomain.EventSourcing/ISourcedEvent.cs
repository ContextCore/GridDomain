using System;

namespace GridDomain.EventSourcing
{
    /// <summary>
    ///     Represents an event message.
    /// </summary>
    public interface ISourcedEvent
    {
        /// <summary>
        ///     Gets the identifier of the source originating the event.
        /// </summary>
        Guid SourceId { get; }
        /// <summary>
        ///     Сага, породившая данное сообщение.
        /// </summary>
        Guid SagaId { get; }
        DateTime CreatedTime { get; }
        //int Version { get; }
    }


}