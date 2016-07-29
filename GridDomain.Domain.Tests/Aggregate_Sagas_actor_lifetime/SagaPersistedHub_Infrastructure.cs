using System;
using Akka.Actor;
using Akka.DI.Core;
using GridDomain.CQRS;
using GridDomain.Tests.Sagas.SoftwareProgrammingDomain.Events;
using GridDomain.Tests.SampleDomain.Commands;

namespace GridDomain.Tests.Aggregate_Sagas_actor_lifetime
{
    internal class SagaPersistedHub_Infrastructure : IPersistentActorTestsInfrastructure
    {
        public SagaPersistedHub_Infrastructure(ActorSystem system)
        {
            ChildId = Guid.NewGuid();
            var gotTired = new GotTiredEvent(Guid.NewGuid(),Guid.NewGuid(),Guid.NewGuid());
            ChildCreateMessage = gotTired;
            ChildActivateMessage = new CoffeMadeEvent(gotTired.FavoriteCoffeMachineId,gotTired.PersonId);
            var hubProps = system.DI().Props<TestSagaHubActor>();
            Hub = system.ActorOf(hubProps, $"TestIstanceSagaHub_{ChildId}");
        }
        public IActorRef Hub { get; }
        public object ChildCreateMessage { get; }
        public object ChildActivateMessage { get; }
        public Guid ChildId { get; }
    }
}