namespace GridDomain.Node
{
    public interface IShardEnvelop
    {
        string EntityId { get; }
        string ShardId { get; }
        string Region { get; }
        object Message { get; }
    }
}