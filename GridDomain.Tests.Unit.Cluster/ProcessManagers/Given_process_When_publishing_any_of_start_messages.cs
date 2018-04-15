using System;
using System.Threading.Tasks;
using GridDomain.Common;
using GridDomain.Node.AkkaMessaging.Waiting;
using GridDomain.ProcessManagers.State;
using GridDomain.Tests.Common;
using GridDomain.Tests.Unit.Cluster;
using GridDomain.Tests.Unit.ProcessManagers.SoftwareProgrammingDomain;
using GridDomain.Tests.Unit.ProcessManagers.SoftwareProgrammingDomain.Events;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.Unit.ProcessManagers
{
    public class Cluster_Given_process_When_publishing_any_of_start_messages : Given_process_When_publishing_any_of_start_messages
    {
        public Cluster_Given_process_When_publishing_any_of_start_messages(ITestOutputHelper helper)
            : base(new SoftwareProgrammingProcessManagerFixture(helper).Clustered()){}
    }
}