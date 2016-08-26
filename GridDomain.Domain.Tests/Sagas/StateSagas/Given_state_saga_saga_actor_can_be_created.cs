using System;
using Akka.Actor;
using Akka.DI.Core;
using GridDomain.Node.Actors;
using GridDomain.Node.AkkaMessaging;
using GridDomain.Tests.Sagas.StateSagas.SampleSaga;
using NUnit.Framework;

namespace GridDomain.Tests.Sagas.StateSagas
{
    
    [TestFixture]
    public class Given_state_saga_saga_actor_can_be_created : SoftwareProgramming_StateSaga_Test
    {
        [Then]
        public void State_saga_actor_can_be_created()
        {
            var actorType = typeof(SagaActor<StateSagas.SampleSaga.SoftwareProgrammingSaga, SoftwareProgrammingSagaState>);

            var props = GridNode.System.DI().Props(actorType);
            var name = new AggregateActorName(typeof(SoftwareProgrammingSagaState), Guid.NewGuid()).ToString();
            var actor = GridNode.System.ActorOf(props, name);
            actor.Tell(new CheckHealth());
            ExpectMsg<HealthStatus>();
        }
    }
}