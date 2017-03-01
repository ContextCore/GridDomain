using System;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.DI.Core;
using GridDomain.EventSourcing.Sagas;
using GridDomain.EventSourcing.Sagas.InstanceSagas;
using GridDomain.Node.Actors;
using GridDomain.Node.AkkaMessaging;
using GridDomain.Tests.Framework;
using GridDomain.Tests.XUnit.Sagas.SoftwareProgrammingDomain;
using GridDomain.Tests.XUnit.Sagas.SoftwareProgrammingDomain.Events;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.XUnit.Sagas
{
    public class Given_istance_saga_saga_actor_can_be_created : SoftwareProgrammingInstanceSagaTest
    {
        public Given_istance_saga_saga_actor_can_be_created(ITestOutputHelper helper) : base(helper) {}

        [Fact]
        public void Instance_saga_actor_can_be_created()
        {
            var actorType =
                typeof(
                    SagaActor
                    <ISagaInstance<SoftwareProgrammingSaga, SoftwareProgrammingSagaData>,
                        SagaStateAggregate<SoftwareProgrammingSagaData>>);

            var props = Node.System.DI().Props(actorType);
            var name =
                new AggregateActorName(typeof(SagaStateAggregate<SoftwareProgrammingSagaData>), Guid.NewGuid()).ToString();
            var actor = Node.System.ActorOf(props, name);
            actor.Tell(new CheckHealth());
            ExpectMsg<HealthStatus>();
        }

        [Fact]
        public async Task Instance_saga_actor_has_correct_path_when_saga_is_raised_by_domain_message()
        {
            var msg = new GotTiredEvent(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());

            await
                Node.NewDebugWaiter()
                    .Expect<SagaMessageReceivedEvent<SoftwareProgrammingSagaData>>()
                    .Create()
                    .SendToSagas(msg);

            var sagaActor = Node.LookupSagaActor<SoftwareProgrammingSaga, SoftwareProgrammingSagaData>(msg.SagaId);
            Assert.NotNull(sagaActor);
        }
    }
}