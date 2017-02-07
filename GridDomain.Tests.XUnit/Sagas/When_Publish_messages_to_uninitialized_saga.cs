using System;
using System.Threading.Tasks;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.Sagas.InstanceSagas;
using GridDomain.Tests.XUnit.Sagas.SoftwareProgrammingDomain;
using GridDomain.Tests.XUnit.Sagas.SoftwareProgrammingDomain.Events;
using Serilog.Core;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.XUnit.Sagas
{
    public class When_Publish_messages_to_uninitialized_saga
    {
        private readonly Logger _log;

        public When_Publish_messages_to_uninitialized_saga(ITestOutputHelper output)
        {
            _log = new XUnitAutoTestLoggerConfiguration(output).CreateLogger();
        }

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

            var saga = SagaInstance.New(softwareProgrammingSaga, sagaDataAggregate, _log);
            await saga.Transit(coffeMadeEvent);
            //No exception is raised
        }

    }
}