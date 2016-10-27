using System;
using Akka.Actor;
using Akka.DI.Core;
using GridDomain.Node.Actors;
using GridDomain.Tests.Sagas.SoftwareProgrammingDomain.Events;
using GridDomain.Tests.Sagas.StateSagas.SampleSaga;

namespace GridDomain.Tests.Aggregate_Sagas_actor_lifetime.Infrastructure
{
    internal class StateSagaPersistedHub_Infrastructure : IPersistentActorTestsInfrastructure
    {
        public StateSagaPersistedHub_Infrastructure(ActorSystem system)
        {
            var sagaId = Guid.NewGuid();
            ChildId = sagaId;
            var gotTired = new GotTiredEvent(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
            var coffeMadeEvent = new CoffeMadeEvent(gotTired.FavoriteCoffeMachineId, gotTired.PersonId);

            ChildCreateMessage = gotTired.CloneWithSaga(sagaId);
            ChildActivateMessage = coffeMadeEvent.CloneWithSaga(sagaId);

            HubProps = system.DI().Props<SagaHubActor<SoftwareProgrammingSaga,SoftwareProgrammingSagaState>>();
        }
        public Props HubProps { get; }
        public object ChildCreateMessage { get; }
        public object ChildActivateMessage { get; }
        public Guid ChildId { get; }
    }
}