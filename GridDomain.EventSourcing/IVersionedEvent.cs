namespace GridDomain.EventSourcing
{
    /// <summary>
    /// Represents an event message that belongs to an ordered event stream.
    /// </summary>
    public interface IVersionedEvent : ISourcedEvent
    {
        /// <summary>
        /// Gets the version or order of the event in the stream.
        /// </summary>
        int Version { get; }
    }
}