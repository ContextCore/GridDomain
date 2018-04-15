using System;
using System.Threading.Tasks;
using Akka.Actor;
using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.Node.Actors.CommandPipe.Messages;
using GridDomain.Node.AkkaMessaging.Waiting;
using GridDomain.ProcessManagers.State;
using GridDomain.Tests.Common;
using GridDomain.Tests.Unit.Cluster;
using GridDomain.Tests.Unit.ProcessManagers.SoftwareProgrammingDomain;
using GridDomain.Tests.Unit.ProcessManagers.SoftwareProgrammingDomain.Commands;
using GridDomain.Tests.Unit.ProcessManagers.SoftwareProgrammingDomain.Events;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.Unit.ProcessManagers
{
    public class Cluster_ProcessManagerProducedEventsAndCommandsHasProcessId : ProcessManagerProducedEventsAndCommandsHasProcessId
    {
        public Cluster_ProcessManagerProducedEventsAndCommandsHasProcessId(ITestOutputHelper helper)
            : base(new SoftwareProgrammingProcessManagerFixture(helper).Clustered()) { }
    }
}