using System;
using Akka.Actor;
using Akka.DI.Core;
using GridDomain.CQRS;
using GridDomain.Tests.Sagas.SoftwareProgrammingDomain.Events;
using GridDomain.Tests.SampleDomain.Commands;

namespace GridDomain.Tests.Aggregate_Sagas_actor_lifetime
{
    internal class InstanceSagaPersistedHub_Infrastructure : IPersistentActorTestsInfrastructure
    {
        public InstanceSagaPersistedHub_Infrastructure(ActorSystem system)
        {
            var sagaId = Guid.NewGuid();
            ChildId = sagaId;
            var gotTired = new GotTiredEvent(Guid.NewGuid(),Guid.NewGuid(),Guid.NewGuid());
            var coffeMadeEvent = new CoffeMadeEvent(gotTired.FavoriteCoffeMachineId, gotTired.PersonId);

            ChildCreateMessage = gotTired.CloneWithSaga(sagaId);
            ChildActivateMessage = coffeMadeEvent.CloneWithSaga(sagaId);

            var hubProps = system.DI().Props<TestInstanceSagaHubActor>();
            Hub = system.ActorOf(hubProps, $"TestIstanceSagaHub_{Guid.NewGuid()}");
        }
        public IActorRef Hub { get; }
        public object ChildCreateMessage { get; }
        public object ChildActivateMessage { get; }
        public Guid ChildId { get; }
    }
}