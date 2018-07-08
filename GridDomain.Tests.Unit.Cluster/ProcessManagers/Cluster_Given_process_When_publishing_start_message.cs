using System.Threading.Tasks;
using GridDomain.EventSourcing;
using GridDomain.ProcessManagers;
using GridDomain.Tests.Common;
using GridDomain.Tests.Unit.ProcessManagers;
using GridDomain.Tests.Unit.ProcessManagers.SoftwareProgrammingDomain;
using GridDomain.Tests.Unit.ProcessManagers.SoftwareProgrammingDomain.Events;
using Xunit.Abstractions;

namespace GridDomain.Tests.Unit.Cluster.ProcessManagers
{
    public class Cluster_Given_process_When_publishing_start_message : Given_process_When_publishing_start_message
    {
        public Cluster_Given_process_When_publishing_start_message(ITestOutputHelper helper) :
            base(new SoftwareProgrammingProcessManagerFixture(helper).Clustered()) { }
    }
}