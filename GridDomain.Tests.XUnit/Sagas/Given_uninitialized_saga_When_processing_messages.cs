using System;
using System.Threading.Tasks;
using GridDomain.EventSourcing.Sagas.InstanceSagas;
using GridDomain.Tests.XUnit.Sagas.SoftwareProgrammingDomain;
using GridDomain.Tests.XUnit.Sagas.SoftwareProgrammingDomain.Events;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.XUnit.Sagas
{
    public class Given_uninitialized_saga_When_processing_messages : SoftwareProgrammingSagaTest
    {
        public Given_uninitialized_saga_When_processing_messages(ITestOutputHelper helper) : base(helper) {}

        [Fact]
        public async Task Saga_data_should_not_be_changed()
        {
            var coffeMadeEvent = new CoffeMadeEvent(Guid.NewGuid(), Guid.NewGuid(), null, Guid.NewGuid());

            Node.Transport.Publish(coffeMadeEvent);
            await Task.Delay(200);
            var sagaDataAggregate =
                await this.LoadAggregate<SagaStateAggregate<SoftwareProgrammingSagaState>>(coffeMadeEvent.SagaId);
            Assert.Null(sagaDataAggregate.SagaState);
        }
    }
}