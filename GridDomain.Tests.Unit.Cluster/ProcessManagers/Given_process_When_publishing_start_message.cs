using System;
using System.Threading.Tasks;
using GridDomain.Tests.Common;
using GridDomain.Tests.Unit.Cluster;
using GridDomain.Tests.Unit.ProcessManagers.SoftwareProgrammingDomain;
using GridDomain.Tests.Unit.ProcessManagers.SoftwareProgrammingDomain.Events;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.Unit.ProcessManagers
{
    public class Cluster_Given_process_When_publishing_start_message : Given_process_When_publishing_start_message
    {
        public Cluster_Given_process_When_publishing_start_message(ITestOutputHelper helper) :
            base(new SoftwareProgrammingProcessManagerFixture(helper).Clustered()) { }
    }
}