namespace GridDomain.Node.Cluster.CommandPipe {
    /// <summary>
    /// Generates shardId as string from given seed and max shards number
    /// ca be used to change names generation algorithm for shards
    /// </summary>
    public interface IShardIdGenerator
    {
        string GetShardId(string seed, int? maxShards=null);
    }
}