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
using GridDomain.Tests.Unit.ProcessManagers.SoftwareProgrammingDomain;
using GridDomain.Tests.Unit.ProcessManagers.SoftwareProgrammingDomain.Events;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.Unit.ProcessManagers
{
    public class GivenIstanceProcessProcessActorCanBeCreated : NodeTestKit
    {
        public GivenIstanceProcessProcessActorCanBeCreated(ITestOutputHelper helper) : this(new SoftwareProgrammingProcessManagerFixture(helper)) {}
        protected GivenIstanceProcessProcessActorCanBeCreated(NodeTestFixture fixture) : base(fixture) {}

        [Fact]
        public void Instance_process_actor_can_be_created()
        {
            var actorType = typeof(ProcessActor<SoftwareProgrammingState>);
            var props = Node.System.DI().Props(actorType);
            var name = EntityActorName.New<ProcessStateAggregate<SoftwareProgrammingState>>(Guid.NewGuid().ToString()).ToString();
            var actor = Node.System.ActorOf(props, name);
            actor.Tell(new CheckHealth());
            ExpectMsg<HealthStatus>();
        }

        [Fact]
        public async Task Instance_process_actor_has_correct_path_when_process_is_started_by_domain_message()
        {
            var resultMsg = await Node.NewLocalDebugWaiter()
                                      .Expect<ProcessManagerCreated<SoftwareProgrammingState>>()
                                      .Create()
                                      .SendToProcessManagers(new GotTiredEvent(Guid.NewGuid().ToString()));

            var processId = resultMsg.Message<ProcessManagerCreated<SoftwareProgrammingState>>().Id;

            var processActor = Node.LookupProcessActor<SoftwareProgrammingProcess, SoftwareProgrammingState>(processId);
            Assert.NotNull(processActor);
        }
    }
}