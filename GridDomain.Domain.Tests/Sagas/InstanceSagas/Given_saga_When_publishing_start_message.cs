using System;
using System.Linq;
using System.Threading;
using Akka.Actor;
using Akka.DI.Core;
using CommonDomain;
using GridDomain.CQRS.Messaging;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.Sagas;
using GridDomain.EventSourcing.Sagas.InstanceSagas;
using GridDomain.Node.Actors;
using GridDomain.Node.Configuration.Composition;
using GridDomain.Tests.FutureEvents;
using GridDomain.Tests.Sagas.InstanceSagas;
using GridDomain.Tests.Sagas.InstanceSagas.Events;
using GridDomain.Tests.SynchroniousCommandExecute;
using NUnit.Framework;
using Ploeh.AutoFixture;

namespace GridDomain.Tests.Sagas.StateSagas
{
    [TestFixture]
    class Given_istance_saga_saga_actor_can_be_created : InMemorySampleDomainTests
    {
        protected override IContainerConfiguration CreateConfiguration()
        {
            var baseConf = base.CreateConfiguration();
            return new CustomContainerConfiguration(
                c => c.RegisterSaga<SoftwareProgrammingSaga,
                                    SoftwareProgrammingSagaData,
                                    GotTiredDomainEvent,
                                    SoftwareProgrammingSagaFactory
                                    >(),
                c => c.Register(baseConf)
                );
        }

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

    [TestFixture]
    class Given_saga_When_publishing_start_message : InMemorySampleDomainTests   
{
        private Guid _sagaId;
        private GotTiredDomainEvent _sagaStartMessage;
        private SagaDataAggregate<SoftwareProgrammingSagaData> _sagaData;

        protected override IMessageRouteMap CreateMap()
        {
            return new SoftwareProgrammingSagaRoutes();
        }

        protected override IContainerConfiguration CreateConfiguration()
        {
            var baseConf = base.CreateConfiguration();
            return new CustomContainerConfiguration(
                c => c.RegisterSaga<SoftwareProgrammingSaga,
                                    SoftwareProgrammingSagaData,
                                    GotTiredDomainEvent,
                                    SoftwareProgrammingSagaFactory
                                    >(),
                c => c.RegisterAggregate<SagaDataAggregate<SoftwareProgrammingSagaData>,
                                        SagaDataAggregateCommandsHandlerDummy<SoftwareProgrammingSagaData>>(),
                c => c.Register(baseConf)
                );
        }

        [TestFixtureSetUp]
        public void When_publishing_start_message()
        {
            var generator = new Fixture();
            _sagaStartMessage = (GotTiredDomainEvent)
                new GotTiredDomainEvent(Guid.NewGuid(),Guid.NewGuid(),Guid.NewGuid())
                                         .CloneWithSaga(Guid.NewGuid());

            _sagaId = _sagaStartMessage.SagaId;
            GridNode.Transport.Publish(_sagaStartMessage);

            Thread.Sleep(1000);

            _sagaData = LoadAggregate<SagaDataAggregate<SoftwareProgrammingSagaData>>(_sagaId);
        }

        [Then]
        public void Saga_has_correct_state()
        {
            var saga = new SoftwareProgrammingSaga();
            Assert.AreEqual(_sagaData.Data.CurrentState,saga.MakingCoffee);
        }

        [Then]
        public void Saga_has_correct_id()
        {
            Assert.AreEqual(_sagaStartMessage.SagaId,_sagaData.Id);
        }

        [Then]
        public void Saga_has_correct_data()
        {
            Assert.AreEqual(_sagaStartMessage.PersonId, _sagaData.Data.PersonId);
        }
    }
}
