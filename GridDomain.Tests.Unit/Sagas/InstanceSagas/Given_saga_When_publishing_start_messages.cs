using System;
using GridDomain.CQRS;
using GridDomain.EventSourcing.Sagas.InstanceSagas;
using GridDomain.Tests.Framework;
using NUnit.Framework;

namespace GridDomain.Tests.Unit.Sagas.InstanceSagas
{
    abstract class Given_saga_When_publishing_start_messages : SoftwareProgrammingInstanceSagaTest   
    {
        protected readonly Guid _sagaId;
        private readonly object[] _sagaMessages;
        protected SagaDataAggregate<SoftwareProgrammingSagaData> SagaData;
       

        public Given_saga_When_publishing_start_messages(Guid sagaId, params object[] messages)
        {
            _sagaMessages = messages;
            _sagaId = sagaId;
        }

        [OneTimeSetUp]
        public void When_publishing_start_message()
        {
            var waiter = GridNode.NewDebugWaiter(Timeout);

            ConfigureWait(waiter)
                        .Create()
                        .Publish(_sagaMessages)
                        .Wait();

            SagaData = LoadAggregate<SagaDataAggregate<SoftwareProgrammingSagaData>>(_sagaId);
        }

        protected abstract IExpectBuilder<AnyMessagePublisher> ConfigureWait(IMessageWaiter<AnyMessagePublisher> waiter);
    }
}