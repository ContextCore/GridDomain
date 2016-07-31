using System;
using System.Diagnostics;
using System.Threading;
using GridDomain.Common;
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
    public class Given_State_Saga : InMemorySampleDomainTests
    {
        private Guid _sagaId;
        private State _sagaState;

        [TestFixtureSetUp]
        public void When_saga_starts_itself_again()
        {
            var publisher = GridNode.Container.Resolve<IPublisher>();
            _sagaId = Guid.NewGuid();

            publisher.Publish(new StartEvent(Guid.NewGuid()).CloneWithSaga(_sagaId));

            Thread.Sleep(TimeSpan.FromMilliseconds(500));

            _sagaState = LoadSagaState<SagaForRecycling,
                                       State,
                                       StartEvent>(_sagaId);
        }

        [Then]
        public void Saga_state_has_id_from_message()
        {
            Assert.AreEqual(_sagaId, _sagaState.Id);
        }

        [Then]
        public void Saga_state_changed_to_start()
        {
            Assert.AreEqual(States.Created, _sagaState.MachineState);
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
