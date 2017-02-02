using System;
using GridDomain.EventSourcing.Sagas;
using GridDomain.EventSourcing.Sagas.InstanceSagas;
using GridDomain.Tests.Framework;
using GridDomain.Tests.XUnit.Sagas.SoftwareProgrammingDomain.Events;
using Xunit;

namespace GridDomain.Tests.XUnit.Sagas.InstanceSagas
{
   
    public class Given_saga_When_publishing_any_of_start_messages : SoftwareProgrammingInstanceSagaTest
    {
        private static readonly Guid SagaId = Guid.NewGuid();
        private SagaStateAggregate<SoftwareProgrammingSagaData> _sagaData;

        [Fact]
        public void When_publishing_start_message()
        {
            GridNode.NewDebugWaiter(DefaultTimeout)
                    .Expect<SagaCreatedEvent<SoftwareProgrammingSagaData>>()
                    .Create()
                    .SendToSagas(new SleptWellEvent(Guid.NewGuid(), Guid.NewGuid(), SagaId))
                    .Wait();

            _sagaData = LoadAggregate<SagaStateAggregate<SoftwareProgrammingSagaData>>(SagaId);
     //Saga_data_is_not_null()
            Assert.NotNull(_sagaData.Data);
       // Saga_has_correct_id()
            Assert.Equal(SagaId, _sagaData.Id);
        }
    }
}