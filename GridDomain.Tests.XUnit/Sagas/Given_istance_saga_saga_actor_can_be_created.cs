using System;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.DI.Core;
using GridDomain.EventSourcing.Sagas;
using GridDomain.EventSourcing.Sagas.InstanceSagas;
using GridDomain.Node.Actors;
using GridDomain.Node.AkkaMessaging;
using GridDomain.Node.AkkaMessaging.Waiting;
using GridDomain.Tests.Common;
using GridDomain.Tests.XUnit.Sagas.SoftwareProgrammingDomain;
using GridDomain.Tests.XUnit.Sagas.SoftwareProgrammingDomain.Events;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.XUnit.Sagas
{
    public class Given_istance_saga_saga_actor_can_be_created : SoftwareProgrammingSagaTest
    {
        public Given_istance_saga_saga_actor_can_be_created(ITestOutputHelper helper) : base(helper) {}

        [Fact]
        public void Instance_saga_actor_can_be_created()
        {
            var actorType = typeof(SagaActor<SoftwareProgrammingState>);

            var props = Node.System.DI().Props(actorType);
            var name = new AggregateActorName(typeof(SagaStateAggregate<SoftwareProgrammingState>), Guid.NewGuid()).ToString();
            var actor = Node.System.ActorOf(props, name);
            actor.Tell(new CheckHealth());
            ExpectMsg<HealthStatus>();
        }

        [Fact]
        public async Task Instance_saga_actor_has_correct_path_when_saga_is_started_by_domain_message()
        {
            var resultMsg = await Node.NewDebugWaiter()
                                      .Expect<SagaCreated<SoftwareProgrammingState>>()
                                      .Create()
                                      .SendToSagas(new GotTiredEvent(Guid.NewGuid()));

            var sagaId = resultMsg.Message<SagaCreated<SoftwareProgrammingState>>().Id;

            var sagaActor = Node.LookupSagaActor<SoftwareProgrammingProcess, SoftwareProgrammingState>(sagaId);
            Assert.NotNull(sagaActor);
        }
    }
}