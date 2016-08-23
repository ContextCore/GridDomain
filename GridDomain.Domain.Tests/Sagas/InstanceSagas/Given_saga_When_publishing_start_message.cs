using System;
using System.Threading;
using GridDomain.EventSourcing.Sagas.InstanceSagas;
using GridDomain.Tests.Sagas.SoftwareProgrammingDomain.Events;
using NUnit.Framework;

namespace GridDomain.Tests.Sagas.InstanceSagas
{
    [TestFixture]
    class Given_saga_When_publishing_start_message_A : Given_saga_When_publishing_any_of_start_messages
    {
        private static readonly GotTiredEvent StartMessage = new GotTiredEvent(Guid.NewGuid(), Guid.NewGuid(),
            Guid.NewGuid(), Guid.NewGuid());
        public Given_saga_When_publishing_start_message_A():base(StartMessage.SagaId, StartMessage)
        {
            

        }

        [Then]
        public void Saga_has_correct_data()
        {
            Assert.AreEqual(StartMessage.PersonId, SagaData.Data.PersonId);
        }
    }

    [TestFixture]
    class Given_saga_When_publishing_several_start_messages : Given_saga_When_publishing_any_of_start_messages
    {
        private static readonly Guid SagaId = Guid.NewGuid();
        private static  GotTiredEvent firstMessage;
        private static  SleptWellEvent secondMessage;
        private static object[] GetMessages(Guid sagaId)
        {
            firstMessage = new GotTiredEvent(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), sagaId);
            secondMessage = new SleptWellEvent(Guid.NewGuid(), Guid.NewGuid(), sagaId);

            return new object[]
            {
                firstMessage,
                secondMessage
            };
        }

        public Given_saga_When_publishing_several_start_messages(): base(SagaId,GetMessages(SagaId))
        {

        }

        [Then]
        public void Saga_reinitialized_from_last_start_message()
        {
            Assert.AreEqual(secondMessage.PersonId, SagaData.Data.PersonId);
        }
    }

    [TestFixture]
    class Given_saga_When_publishing_start_message_B : Given_saga_When_publishing_any_of_start_messages
    {
        private static readonly SleptWellEvent StartMessage = new SleptWellEvent(Guid.NewGuid(),Guid.NewGuid(),Guid.NewGuid(), null);

        public Given_saga_When_publishing_start_message_B() : base(StartMessage.SagaId, StartMessage)
        {


        }

        [Then]
        public void Saga_has_correct_data()
        {
            Assert.AreEqual(StartMessage.SofaId, SagaData.Data.SofaId);
        }
    }

    class Given_saga_When_publishing_any_of_start_messages : ProgrammingSoftwareSagaTest   
{
        private readonly Guid _sagaId;
        private readonly object[] _sagaMessages;
        protected SagaDataAggregate<SoftwareProgrammingSagaData> SagaData;
       

     
        public Given_saga_When_publishing_any_of_start_messages(Guid sagaId, params object[] messages)
        {
            _sagaMessages = messages;
            _sagaId = sagaId;

        }
      
        [TestFixtureSetUp]
        public void When_publishing_start_message()
        {
            foreach(var msg in _sagaMessages)
                GridNode.Transport.Publish(msg);

            Thread.Sleep(Timeout);

            SagaData = LoadAggregate<SagaDataAggregate<SoftwareProgrammingSagaData>>(_sagaId);
        }

        [Then]
        public void Saga_data_is_not_null()
        {
            Assert.NotNull(SagaData.Data);
        }

        [Then]
        public void Saga_has_correct_state()
        {
            var saga = new SoftwareProgrammingSaga();
            Assert.AreEqual(saga.MakingCoffee.Name,SagaData.Data.CurrentStateName);
        }

        [Then]
        public void Saga_has_correct_id()
        {
            Assert.AreEqual(_sagaId,SagaData.Id);
        }

    }
}
