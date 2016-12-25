using System;
using System.Threading;
using GridDomain.Common;
using GridDomain.CQRS.Messaging;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.Sagas;
using GridDomain.EventSourcing.Sagas.InstanceSagas;
using GridDomain.Node.AkkaMessaging;
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
            var msg = new CustomEvent(Guid.NewGuid()) { Payload = "1232"};

            var publisher = GridNode.Container.Resolve<IPublisher>();
            publisher.Publish(msg,new MessageMetadata(msg.SagaId));

            Thread.Sleep(100);

            var sagaActor =
                LookupInstanceSagaActor<CustomRoutesSoftwareProgrammingSaga, SoftwareProgrammingSagaData>(msg.SagaId);
            Assert.NotNull(sagaActor);
        }
    }
}