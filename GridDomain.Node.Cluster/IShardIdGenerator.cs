namespace GridDomain.Node.Cluster {
    /// <summary>
    /// Generates shardId as string from given seed and max shards number
    /// ca be used to change names generation algorithm for shards
    /// </summary>
    public interface IShardIdGenerator
    {
        string Resolve(string seed, int? maxShards=null);
    }
}