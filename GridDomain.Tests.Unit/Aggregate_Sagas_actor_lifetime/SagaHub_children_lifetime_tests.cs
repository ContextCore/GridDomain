using System;
using Akka.Actor;
using Akka.DI.Core;
using GridDomain.Common;
using GridDomain.EventSourcing;
using GridDomain.Node.Actors;
using GridDomain.Node.Actors.Saga;
using GridDomain.Tests.Unit.Sagas.SoftwareProgrammingDomain;
using GridDomain.Tests.Unit.Sagas.SoftwareProgrammingDomain.Events;
using Xunit.Abstractions;

namespace GridDomain.Tests.Unit.Aggregate_Sagas_actor_lifetime
{
    public class SagaHub_children_lifetime_tests : PersistentHubChildrenLifetimeTest
    {
        public SagaHub_children_lifetime_tests(ITestOutputHelper output)
            : base(output, new PersistentHubFixture(new InstanceSagaPersistedHubInfrastructure())) {}

        internal class InstanceSagaPersistedHubInfrastructure : IPersistentActorTestsInfrastructure
        {
            public InstanceSagaPersistedHubInfrastructure()
            {
                var sagaId = Guid.NewGuid();
                ChildId = sagaId;
                var gotTired = new GotTiredEvent(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), sagaId);
                var coffeMadeEvent = new CoffeMadeEvent(gotTired.FavoriteCoffeMachineId, gotTired.PersonId, null, sagaId);

                ChildCreateMessage = new MessageMetadataEnvelop<DomainEvent>(gotTired, new MessageMetadata(gotTired.SourceId));

                ChildActivateMessage = new MessageMetadataEnvelop<DomainEvent>(coffeMadeEvent,
                                                                               new MessageMetadata(coffeMadeEvent.SourceId));
            }

            Props IPersistentActorTestsInfrastructure.CreateHubProps(ActorSystem system)
            {
                return
                    system.DI().Props<SagaHubActor<SoftwareProgrammingState>>();
            }

            public object ChildCreateMessage { get; }
            public object ChildActivateMessage { get; }
            public Guid ChildId { get; }
        }
    }
}