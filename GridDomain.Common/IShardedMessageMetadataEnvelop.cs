namespace GridDomain.Common {
    public interface IShardedMessageMetadataEnvelop : IMessageMetadataEnvelop
    {
        string EntityId { get; }
        string ShardId { get; }
    }
}