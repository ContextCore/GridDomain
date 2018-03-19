namespace GridDomain.Common
{
    public interface IMessageMetadataEnvelop
    {
        object Message { get; }

        /// <summary>
        ///     Metadata can be mutable
        ///     It is not supposed to be serialized!
        ///     No business desicions should be made based on metadata
        /// </summary>
        IMessageMetadata Metadata { get; }
    }

    public interface IMessageMetadataEnvelop<out T> : IMessageMetadataEnvelop
    {
        new T Message { get; }
    }

    public interface IShardedMessageMetadataEnvelop : IMessageMetadataEnvelop<IHaveId> //where T : IHaveId
    {
        string ShardId { get; }
    }
    
    public interface IShardedMessageMetadataEnvelop<T> : IMessageMetadataEnvelop<T> where T : IHaveId
    {
        string ShardId { get; }
    }
}