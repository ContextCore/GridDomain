namespace GridDomain.Common
{
    public interface IMessageWithMetadata
    {
        object Message { get; }
        /// <summary>
        /// Metadata can be mutable
        /// It is not supposed to be serialized!
        /// No business desicions should be made based on metadata
        /// </summary>
        IMessageMetadata Metadata { get; }
    }


    public interface IMessageMetadataEnvelop<out T> : IMessageWithMetadata
    {
        new T Message { get; }
    }
}