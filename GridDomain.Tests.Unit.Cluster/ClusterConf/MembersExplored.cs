using System.Collections.Generic;
using Akka.Cluster;

namespace GridDomain.Tests.Unit.Cluster.ClusterConf {
    public class MembersExplored
    {
        public IReadOnlyCollection<Member> Members { get; }

        public MembersExplored(IReadOnlyCollection<Member> members)
        {
            Members = members;
        }
    }
}