using System;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.DI.Core;
using GridDomain.EventSourcing.Sagas;
using GridDomain.EventSourcing.Sagas.InstanceSagas;
using GridDomain.Node.Actors;
using GridDomain.Node.AkkaMessaging;
using GridDomain.Node.AkkaMessaging.Waiting;
using GridDomain.Tests.Framework;
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
            var actorType = typeof(SagaActor<SoftwareProgrammingSagaState>);

            var props = Node.System.DI().Props(actorType);
            var name = new AggregateActorName(typeof(SagaStateAggregate<SoftwareProgrammingSagaState>), Guid.NewGuid()).ToString();
            var actor = Node.System.ActorOf(props, name);
            actor.Tell(new CheckHealth());
            ExpectMsg<HealthStatus>();
        }

        [Fact]
        public async Task Instance_saga_actor_has_correct_path_when_saga_is_started_by_domain_message()
        {
            var msg = new GotTiredEvent(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());

            var resultMsg = await Node.NewDebugWaiter()
                                      .Expect<SagaCreatedEvent<SoftwareProgrammingSagaState>>()
                                      .Create()
                                      .SendToSagas(msg);

            var sagaId = resultMsg.Message<SagaCreatedEvent<SoftwareProgrammingSagaState>>().Id;

            var sagaActor = Node.LookupSagaActor<SoftwareProgrammingSaga, SoftwareProgrammingSagaState>(sagaId);
            Assert.NotNull(sagaActor);
        }
    }
}