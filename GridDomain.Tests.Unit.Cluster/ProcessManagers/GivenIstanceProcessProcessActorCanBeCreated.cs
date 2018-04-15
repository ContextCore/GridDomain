using System;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.DI.Core;
using GridDomain.Node.Actors.EventSourced.Messages;
using GridDomain.Node.Actors.ProcessManagers;
using GridDomain.Node.AkkaMessaging;
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
    public class Cluster_GivenIstanceProcessProcessActorCanBeCreated : GivenIstanceProcessProcessActorCanBeCreated
    {
        public Cluster_GivenIstanceProcessProcessActorCanBeCreated(ITestOutputHelper helper) : base(new SoftwareProgrammingProcessManagerFixture(helper).Clustered()) {}

    }
}