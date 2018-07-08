using GridDomain.Tests.Unit.Metadata;
using GridDomain.Tests.Unit.ProcessManagers;
using Xunit.Abstractions;

namespace GridDomain.Tests.Unit.Cluster.Metadata
{
    public class Cluster_MetadataFromProcessReceivedEventPassedToProducedCommands : MetadataFromProcessReceivedEventPassedToProducedCommands
    {
        public Cluster_MetadataFromProcessReceivedEventPassedToProducedCommands(ITestOutputHelper helper) 
            : base(new SoftwareProgrammingProcessManagerFixture(helper).Clustered()) {}
    }
}