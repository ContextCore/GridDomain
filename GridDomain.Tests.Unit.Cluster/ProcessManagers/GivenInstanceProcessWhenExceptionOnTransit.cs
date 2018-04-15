using System;
using System.Threading.Tasks;
using Automatonymous;
using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.Node.AkkaMessaging.Waiting;
using GridDomain.ProcessManagers;
using GridDomain.ProcessManagers.State;
using GridDomain.Tests.Common;
using GridDomain.Tests.Unit.Cluster;
using GridDomain.Tests.Unit.ProcessManagers.SoftwareProgrammingDomain;
using GridDomain.Tests.Unit.ProcessManagers.SoftwareProgrammingDomain.Events;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.Unit.ProcessManagers
{
    public class Cluster_GivenInstanceProcessWhenExceptionOnTransit : GivenInstanceProcessWhenExceptionOnTransit
    {
        public Cluster_GivenInstanceProcessWhenExceptionOnTransit(ITestOutputHelper helper) : base(new SoftwareProgrammingProcessManagerFixture(helper).Clustered()) {}
    }
}