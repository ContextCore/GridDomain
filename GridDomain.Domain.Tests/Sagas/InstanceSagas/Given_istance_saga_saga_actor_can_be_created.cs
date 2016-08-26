using System;
using Akka.Actor;
using Akka.DI.Core;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.Sagas;
using GridDomain.EventSourcing.Sagas.InstanceSagas;
using GridDomain.Node;
using GridDomain.Node.Actors;
using GridDomain.Node.AkkaMessaging;
using GridDomain.Tests.Sagas.SoftwareProgrammingDomain.Events;
using GridDomain.Tests.Sagas.StateSagas.SampleSaga;
using NUnit.Framework;

namespace GridDomain.Tests.Sagas.InstanceSagas
{
    [TestFixture]
    class Given_istance_saga_saga_actor_can_be_created : ProgrammingSoftwareSagaTest
    {
        [Then]
        public void Instance_saga_actor_can_be_created()
        {
            var actorType  = typeof(SagaActor<ISagaInstance<SoftwareProgrammingSaga, SoftwareProgrammingSagaData>,
                                    SagaDataAggregate<SoftwareProgrammingSagaData>>);

            var props = GridNode.System.DI().Props(actorType);
            var name = new AggregateActorName(typeof(SagaDataAggregate<SoftwareProgrammingSagaData>),Guid.NewGuid()).ToString();
            var actor = GridNode.System.ActorOf(props, name);
            actor.Tell(new CheckHealth());
            ExpectMsg<HealthStatus>();
        }
    }
}