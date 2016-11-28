namespace GridDomain.EventSourcing
{
    public interface IMessageEnvelop
    {
        object Event { get; }
        /// <summary>
        /// Metadata can be mutable
        /// It is not supposed to be serialized!
        /// No business desicions should be made based on metadata
        /// </summary>
        IMetadata Metadata { get; }
    }

    public interface IMessageEnvelop<out T> : IMessageEnvelop
    {
        new T Event { get; }
    }
}