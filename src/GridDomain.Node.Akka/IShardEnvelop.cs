namespace GridDomain.Node.Akka
{
    public interface IShardEnvelop
    {
        string EntityId { get; }
        string ShardId { get; }
        string Region { get; }
        object Message { get; }
    }
}