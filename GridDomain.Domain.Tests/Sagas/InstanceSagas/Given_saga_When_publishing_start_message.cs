using System;
using System.Linq;
using System.Threading;
using CommonDomain;
using GridDomain.CQRS.Messaging;
using GridDomain.EventSourcing.Sagas.InstanceSagas;
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
    class Given_saga_When_publishing_start_message : ProgrammingSoftwareSagaTest   
{
        private Guid _sagaId;
        private GotTiredDomainEvent _sagaStartMessage;
        private SagaDataAggregate<SoftwareProgrammingSagaData> _sagaData;
    
        [TestFixtureSetUp]
        public void When_publishing_start_message()
        {
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
