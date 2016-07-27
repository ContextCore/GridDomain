using System;
using Akka.Actor;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.Sagas;
using GridDomain.EventSourcing.Sagas.InstanceSagas;
using GridDomain.Node;
using GridDomain.Node.Actors;
using GridDomain.Tests.Sagas.SoftwareProgrammingDomain.Events;
using NUnit.Framework;

namespace GridDomain.Tests.Sagas.InstanceSagas
{
    [TestFixture]
    class Given_istance_saga_saga_actor_can_be_created : ProgrammingSoftwareSagaTest
    {
        [Then]
        public void Saga_actor_can_be_created()
        {
            var actorType  = typeof(SagaActor<ISagaInstance<SoftwareProgrammingSaga, SoftwareProgrammingSagaData>,
                                    SagaDataAggregate<SoftwareProgrammingSagaData>,
                                    GotTiredEvent>);

            var actor = GridNode.System.CreatePersistedIdentityActor(actorType, Guid.Empty);
            actor.Ask(new DomainEvent(Guid.NewGuid()));
            ExpectNoMsg();
        }
    }
}