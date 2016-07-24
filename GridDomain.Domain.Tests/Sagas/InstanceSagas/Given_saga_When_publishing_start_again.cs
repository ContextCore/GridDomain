using System;
using System.Threading;
using GridDomain.EventSourcing.Sagas.InstanceSagas;
using GridDomain.Tests.Sagas.InstanceSagas.Events;
using NUnit.Framework;

namespace GridDomain.Tests.Sagas.InstanceSagas
{
    [TestFixture]
    class Given_saga_When_publishing_start_again : ProgrammingSoftwareSagaTest
    {
        private GotTiredDomainEvent _startMessage;
        private CoffeMadeDomainEvent _coffeMadeDomainEvent;
        private GotTiredDomainEvent _reStartEvent;
        private SagaDataAggregate<SoftwareProgrammingSagaData> _sagaDataAggregate;

        [TestFixtureSetUp]
        public void When_publishing_start_message()
        {
            _startMessage = (GotTiredDomainEvent)
               new GotTiredDomainEvent(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid())
                   .CloneWithSaga(Guid.NewGuid());

            _coffeMadeDomainEvent = (CoffeMadeDomainEvent)
                new CoffeMadeDomainEvent(_startMessage.FavoriteCoffeMachineId, _startMessage.PersonId)
                    .CloneWithSaga(_startMessage.SagaId);

            _reStartEvent = (GotTiredDomainEvent)
                new GotTiredDomainEvent(Guid.NewGuid(),_startMessage.LovelySofaId, Guid.NewGuid())
                    .CloneWithSaga(_startMessage.SagaId);



            GridNode.Transport.Publish(_startMessage);
            GridNode.Transport.Publish(_coffeMadeDomainEvent);
            GridNode.Transport.Publish(_reStartEvent);

            Thread.Sleep(1000);
         
            _sagaDataAggregate = LoadAggregate<SagaDataAggregate<SoftwareProgrammingSagaData>>(_startMessage.SagaId);
        }


        [Then]
        public void Saga_state_should_contain_all_messages()
        {
            var messagesSent = new object[] {_startMessage, _coffeMadeDomainEvent, _reStartEvent};
            CollectionAssert.AreEquivalent(messagesSent, _sagaDataAggregate.ReceivedMessages);
        }

        [Then]
        public void Saga_state_should_be_correct()
        {
            var saga = new SoftwareProgrammingSaga();
            Assert.AreEqual(saga.MakingCoffee.Name, _sagaDataAggregate.Data.CurrentStateName);
        }

        [Then]
        public void Saga_data_contains_information_from_restart_message()
        {
            var saga = new SoftwareProgrammingSaga();
            Assert.AreEqual(_reStartEvent.PersonId, _sagaDataAggregate.Data.PersonId);
        }




        public Given_saga_When_publishing_start_again() : base(true)
        {
        }
    }
}