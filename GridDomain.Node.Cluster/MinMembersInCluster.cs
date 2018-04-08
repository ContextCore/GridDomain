using GridDomain.Node.Configuration.Hocon;

namespace GridDomain.Node.Cluster {
    public class MinMembersInCluster : IHoconConfig
    {
        private int _length;

        public MinMembersInCluster(int length)
        {
            _length = length;
        }

        public string Build()
        {
            return $"cluster.min-nr-of-members = {_length}";
        }
    }
}