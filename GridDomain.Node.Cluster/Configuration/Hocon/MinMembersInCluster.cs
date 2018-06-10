using Akka.Configuration;
using GridDomain.Node.Configuration.Hocon;

namespace GridDomain.Node.Cluster.Configuration.Hocon {
    public class MinMembersInCluster : IHoconConfig
    {
        private int _length;

        public MinMembersInCluster(int length)
        {
            _length = length;
        }

        public Config Build()
        {
            return $"akka.cluster.min-nr-of-members = {_length}";
        }
    }
}