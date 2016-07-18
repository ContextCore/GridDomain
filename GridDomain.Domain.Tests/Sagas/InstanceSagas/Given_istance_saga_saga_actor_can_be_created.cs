using System;
using Akka.Actor;
using Akka.DI.Core;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.Sagas;
using GridDomain.EventSourcing.Sagas.InstanceSagas;
using GridDomain.Node.Actors;
using GridDomain.Tests.Sagas.InstanceSagas;
using GridDomain.Tests.Sagas.InstanceSagas.Events;
using NUnit.Framework;

namespace GridDomain.Tests.Sagas.StateSagas
{
    [TestFixture]
    class Given_istance_saga_saga_actor_can_be_created : ProgrammingSoftwareSagaTest
    {
     

        [Then]
        public void Saga_actor_can_be_created()
        {
            var actorType  = typeof(SagaActor<ISagaInstance<SoftwareProgrammingSaga, SoftwareProgrammingSagaData>,
                SagaDataAggregate<SoftwareProgrammingSagaData>,
                GotTiredDomainEvent>);

            var props = GridNode.System.DI().Props(actorType);
            var actor = GridNode.System.ActorOf(props);
            actor.Ask(new DomainEvent(Guid.NewGuid()));
            ExpectNoMsg();
        }
    }
}