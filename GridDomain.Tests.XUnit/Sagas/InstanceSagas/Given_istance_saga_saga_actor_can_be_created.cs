using System;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.DI.Core;
using GridDomain.EventSourcing.Sagas;
using GridDomain.EventSourcing.Sagas.InstanceSagas;
using GridDomain.Node.Actors;
using GridDomain.Node.AkkaMessaging;
using GridDomain.Tests.Framework;
using GridDomain.Tests.XUnit.Sagas.SoftwareProgrammingDomain.Events;
using Xunit;

namespace GridDomain.Tests.XUnit.Sagas.InstanceSagas
{
   
    public class Given_istance_saga_saga_actor_can_be_created : SoftwareProgrammingInstanceSagaTest
    {
        [Fact]
        public void Instance_saga_actor_can_be_created()
        {
            var actorType = typeof(SagaActor<ISagaInstance<SoftwareProgrammingSaga, SoftwareProgrammingSagaData>,
                                                    SagaStateAggregate<SoftwareProgrammingSagaData>>);

            var props = GridNode.System.DI().Props(actorType);
            var name = new AggregateActorName(typeof(SagaStateAggregate<SoftwareProgrammingSagaData>), Guid.NewGuid()).ToString();
            var actor = GridNode.System.ActorOf(props, name);
            actor.Tell(new CheckHealth());
            ExpectMsg<HealthStatus>();
        }

        [Fact]
        public async Task Instance_saga_actor_has_correct_path_when_saga_is_raised_by_domain_message()
        {
            var msg = new GotTiredEvent(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());

            await GridNode.NewDebugWaiter()
                          .Expect<SagaMessageReceivedEvent<SoftwareProgrammingSagaData>>()
                          .Create()
                          .SendToSagas(msg);

            var sagaActor = LookupSagaActor<SoftwareProgrammingSaga, SoftwareProgrammingSagaData>(msg.SagaId);
            Assert.NotNull(sagaActor);
        }

    }
}