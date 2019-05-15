namespace GridDomain.Node.Akka.Cluster
{
    public interface IShardEnvelop
    {
        string EntityId { get; }
        string ShardId { get; }
        string Region { get; }
        object Message { get; }
    }
}