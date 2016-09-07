using System;
using System.Collections;
using System.Threading;
using Akka.Actor;
using Akka.DI.Core;
using GridDomain.CQRS;
using GridDomain.CQRS.Messaging;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.Sagas;
using GridDomain.EventSourcing.Sagas.InstanceSagas;
using GridDomain.Node;
using GridDomain.Node.Actors;
using GridDomain.Node.AkkaMessaging;
using GridDomain.Tests.Sagas.SoftwareProgrammingDomain.Commands;
using GridDomain.Tests.Sagas.SoftwareProgrammingDomain.Events;
using GridDomain.Tests.Sagas.StateSagas.SampleSaga;
using Microsoft.Practices.Unity;
using NUnit.Framework;

namespace GridDomain.Tests.Sagas.InstanceSagas
{

    [TestFixture]
    class Given_instance_saga_actor_can_be_created_for_non_domain_event : ProgrammingSoftwareSagaTest_with_custom_routes
    {

        [Then]
        public void Instance_saga_actor_has_correct_path_when_saga_is_raised_by_command_fault()
        {
            var msg = new CustomRoutesSoftwareProgrammingSaga.CustomEvent { Payload = "1232", SagaId = Guid.NewGuid() };

            var publisher = GridNode.Container.Resolve<IPublisher>();
            publisher.Publish(msg);

            Thread.Sleep(500);

            var sagaActorName =
                AggregateActorName.New<SagaDataAggregate<SoftwareProgrammingSagaData>>(msg.SagaId).ToString();
            var sagaHubName = typeof(ISagaInstance<CustomRoutesSoftwareProgrammingSaga, SoftwareProgrammingSagaData>).BeautyName();

            string path = $"akka://LocalSystem/user/SagaHub_{sagaHubName}/*/{sagaActorName}";

            var sagaActor = GridNode.System.ActorSelection(path).ResolveOne(TimeSpan.FromSeconds(1)).Result;
            Assert.NotNull(sagaActor);
        }
    }

    [TestFixture]
    class Given_istance_saga_saga_actor_can_be_created : ProgrammingSoftwareSagaTest
    {
        [Then]
        public void Instance_saga_actor_can_be_created()
        {
            var actorType = typeof(SagaActor<ISagaInstance<SoftwareProgrammingSaga, SoftwareProgrammingSagaData>,
                SagaDataAggregate<SoftwareProgrammingSagaData>>);

            var props = GridNode.System.DI().Props(actorType);
            var name =
                new AggregateActorName(typeof(SagaDataAggregate<SoftwareProgrammingSagaData>), Guid.NewGuid()).ToString();
            var actor = GridNode.System.ActorOf(props, name);
            actor.Tell(new CheckHealth());
            ExpectMsg<HealthStatus>();
        }

        [Then]
        public void Instance_saga_actor_has_correct_path_when_saga_is_raised_by_domain_message()
        {

            var msg = new GotTiredEvent(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
            var publisher = GridNode.Container.Resolve<IPublisher>();
            publisher.Publish(msg);
            var sagaActorName =
                AggregateActorName.New<SagaDataAggregate<SoftwareProgrammingSagaData>>(msg.SagaId).ToString();
            var sagaHubName = typeof(ISagaInstance<SoftwareProgrammingSaga, SoftwareProgrammingSagaData>).BeautyName();
            WaitFor<MakeCoffeCommand>();
            string path = $"akka://LocalSystem/user/SagaHub_{sagaHubName}/*/{sagaActorName}";

            var sagaActor = GridNode.System.ActorSelection(path).ResolveOne(TimeSpan.FromSeconds(1)).Result;
            Assert.NotNull(sagaActor);
        }

    }
}