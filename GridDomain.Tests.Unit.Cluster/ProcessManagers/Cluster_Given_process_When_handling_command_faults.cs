using System;
using System.Threading.Tasks;
using GridDomain.Common;
using GridDomain.ProcessManagers.State;
using GridDomain.Tests.Common;
using GridDomain.Tests.Unit.ProcessManagers;
using GridDomain.Tests.Unit.ProcessManagers.SoftwareProgrammingDomain;
using GridDomain.Tests.Unit.ProcessManagers.SoftwareProgrammingDomain.Events;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.Unit.Cluster.ProcessManagers
{

    public class Cluster_Given_process_When_handling_command_faults : Given_process_When_handling_command_faults
    {
        public Cluster_Given_process_When_handling_command_faults(ITestOutputHelper output) :
            base(new SoftwareProgrammingProcessManagerFixture(output).Clustered()) {}
    }
}