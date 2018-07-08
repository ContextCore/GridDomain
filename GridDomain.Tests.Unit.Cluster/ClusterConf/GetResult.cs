namespace GridDomain.Tests.Unit.Cluster.ClusterConf {
    public class GetResult 
    {
        public int MaxMembers { get; }

        public GetResult(int maxMembers)
        {
            MaxMembers = maxMembers;
        }
    }
}