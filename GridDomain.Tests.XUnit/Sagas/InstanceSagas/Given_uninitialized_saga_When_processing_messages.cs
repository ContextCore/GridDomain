using System;
using System.Threading;
using System.Threading.Tasks;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.Sagas.InstanceSagas;
using GridDomain.Tests.XUnit.Sagas.SoftwareProgrammingDomain.Events;
using Xunit;

namespace GridDomain.Tests.XUnit.Sagas.InstanceSagas
{
    public class Given_uninitialized_saga_When_processing_messages : SoftwareProgrammingInstanceSagaTest
    {
        [Theory]
        [InlineData(false, true)] //"Saga id is empty and it has data")]
        [InlineData(false, false)] // Description = "Saga id is empty and no data")]
        [InlineData(true, false)] // Description = "Saga has id and no data")]
        public async Task Given_saga_when_publishing_known_message_without_saga_data(bool sagaHasId, bool sagaHasData)
        {
            var softwareProgrammingSaga = new SoftwareProgrammingSaga();

            var coffeMadeEvent = new CoffeMadeEvent(Guid.NewGuid(), Guid.NewGuid());

            var sagaId = !sagaHasId ? Guid.Empty : Guid.NewGuid();
            var sagaDataAggregate = Aggregate.Empty<SagaStateAggregate<SoftwareProgrammingSagaData>>(sagaId);
            sagaDataAggregate.RememberEvent(softwareProgrammingSaga.CoffeReady,
                !sagaHasData ? null : new SoftwareProgrammingSagaData(sagaId, ""),
                null);

            var saga = SagaInstance.New(softwareProgrammingSaga, sagaDataAggregate);
            await saga.Transit(coffeMadeEvent);
            //No exception is raised
        }

        [Fact]
        public void Saga_data_should_not_be_changed()
        {
            var coffeMadeEvent = new CoffeMadeEvent(Guid.NewGuid(), Guid.NewGuid(),null,Guid.NewGuid());

            GridNode.Transport.Publish(coffeMadeEvent);
            Thread.Sleep(200);
            var sagaDataAggregate = LoadAggregate<SagaStateAggregate<SoftwareProgrammingSagaData>>(coffeMadeEvent.SagaId);
            Assert.Null(sagaDataAggregate.Data);
        }

        public Given_uninitialized_saga_When_processing_messages() : base(true) {}
    }
}