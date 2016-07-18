using System;
using System.Diagnostics;
using System.Threading;
using GridDomain.CQRS.Messaging;
using GridDomain.EventSourcing.Sagas;
using GridDomain.Node;
using GridDomain.Node.Configuration.Akka;
using GridDomain.Node.Configuration.Composition;
using GridDomain.Node.Configuration.Persistence;
using GridDomain.Scheduling.Quartz;
using GridDomain.Tests.Framework;
using GridDomain.Tests.Framework.Configuration;
using GridDomain.Tests.FutureEvents;
using GridDomain.Tests.Sagas.SagaRecycling.Saga;
using GridDomain.Tests.SynchroniousCommandExecute;
using Microsoft.Practices.Unity;
using NUnit.Framework;

namespace GridDomain.Tests.Sagas.SagaRecycling
{
    [TestFixture]
    public class SagaRecyclingTest : InMemorySampleDomainTests
    {

        [Test]
        public void SagaRecycling()
        {
            var publisher = GridNode.Container.Resolve<IPublisher>();
            var sagaId = Guid.NewGuid();

            publisher.Publish(new StartEvent(Guid.NewGuid()).CloneWithSaga(sagaId));

            Thread.Sleep(TimeSpan.FromMilliseconds(1000));

            var sagaState = LoadSagaState<SagaForRecycling,
                                          State,
                                          StartEvent>(sagaId);

            Assert.AreEqual(sagaId, sagaState.Id);
            Assert.AreEqual(States.Created, sagaState.MachineState);
        }

        protected override IContainerConfiguration CreateConfiguration()
        {
            return new CustomContainerConfiguration(container =>
            {
                container.RegisterType<ISagaFactory<SagaForRecycling, StartEvent>, SagaForRecyclingFactory>();
                container.RegisterType<ISagaFactory<SagaForRecycling, State>, SagaForRecyclingFactory>();
                container.RegisterType<ISagaFactory<SagaForRecycling, Guid>, SagaForRecyclingFactory>();
                container.Register(base.CreateConfiguration());
            });
        }

        protected override IMessageRouteMap CreateMap()
        {
            return new SagaForRecyclingRouteMap();
        }
    }
}
