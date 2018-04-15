using System;
using System.Threading.Tasks;
using GridDomain.ProcessManagers.State;
using GridDomain.Tests.Unit.Cluster;
using GridDomain.Tests.Unit.ProcessManagers.SoftwareProgrammingDomain;
using GridDomain.Tests.Unit.ProcessManagers.SoftwareProgrammingDomain.Events;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.Unit.ProcessManagers
{
    public class Cluster_Given_uninitialized_process_When_processing_messages : Given_uninitialized_process_When_processing_messages
    {
        public Cluster_Given_uninitialized_process_When_processing_messages(ITestOutputHelper helper)
            : base(new SoftwareProgrammingProcessManagerFixture(helper).Clustered()) { }
    }
}