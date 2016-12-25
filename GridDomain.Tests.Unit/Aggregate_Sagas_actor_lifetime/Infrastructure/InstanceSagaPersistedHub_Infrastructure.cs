using System;
using Akka.Actor;
using Akka.DI.Core;
using GridDomain.Common;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.Sagas;
using GridDomain.EventSourcing.Sagas.InstanceSagas;
using GridDomain.Node.Actors;
using GridDomain.Tests.Unit.Sagas.InstanceSagas;
using GridDomain.Tests.Unit.Sagas.SoftwareProgrammingDomain.Events;

namespace GridDomain.Tests.Unit.Aggregate_Sagas_actor_lifetime.Infrastructure
{
    internal class InstanceSagaPersistedHub_Infrastructure : IPersistentActorTestsInfrastructure
    {
        public InstanceSagaPersistedHub_Infrastructure(ActorSystem system)
        {
            var sagaId = Guid.NewGuid();
            ChildId = sagaId;
            var gotTired = new GotTiredEvent(Guid.NewGuid(),Guid.NewGuid(),Guid.NewGuid(), sagaId);
            var coffeMadeEvent = new CoffeMadeEvent(gotTired.FavoriteCoffeMachineId, gotTired.PersonId,null,sagaId);

            ChildCreateMessage = new MessageMetadataEnvelop<DomainEvent>(gotTired, 
                                                                         new MessageMetadata(gotTired.SourceId));

            ChildActivateMessage = new MessageMetadataEnvelop<DomainEvent>(coffeMadeEvent,
                                                                           new MessageMetadata(coffeMadeEvent.SourceId));

            HubProps = system.DI().Props<
                SagaHubActor<ISagaInstance<SoftwareProgrammingSaga, SoftwareProgrammingSagaData>,
                             SagaDataAggregate<SoftwareProgrammingSagaData>>>();
        }
        public Props HubProps { get; }
        public object ChildCreateMessage { get; }
        public object ChildActivateMessage { get; }
        public Guid ChildId { get; }
    }
}