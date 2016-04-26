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
}