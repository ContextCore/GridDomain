using GridDomain.EventSourcing.Sagas;
using GridDomain.EventSourcing.Sagas.InstanceSagas;
using GridDomain.Tests.XUnit.Sagas.SoftwareProgrammingDomain;
using GridDomain.Tests.XUnit.Sagas.SoftwareProgrammingDomain.Events;
using Serilog;

namespace GridDomain.Tests.XUnit.Sagas.CustomRoutesSoftwareProgrammingDomain
{
    public class CustomRoutesSoftwareProgrammingSagaFactory :
        ISagaFactory
            <ISagaInstance<CustomRoutesSoftwareProgrammingSaga, SoftwareProgrammingSagaData>,
            SagaStateAggregate<SoftwareProgrammingSagaData>>,
        ISagaFactory<ISagaInstance<CustomRoutesSoftwareProgrammingSaga, SoftwareProgrammingSagaData>, GotTiredEvent>,
        ISagaFactory<ISagaInstance<CustomRoutesSoftwareProgrammingSaga, SoftwareProgrammingSagaData>, SleptWellEvent>
    {
        private readonly ILogger _log;

        public CustomRoutesSoftwareProgrammingSagaFactory(ILogger log)
        {
            _log = log;
        }

        public ISagaInstance<CustomRoutesSoftwareProgrammingSaga, SoftwareProgrammingSagaData> Create(GotTiredEvent message)
        {
            var saga = new CustomRoutesSoftwareProgrammingSaga();
            var data =
                new SagaStateAggregate<SoftwareProgrammingSagaData>(new SoftwareProgrammingSagaData(message.PersonId,
                    saga.Coding.Name));
            return SagaInstance.New(saga, data, _log);
        }

        public ISagaInstance<CustomRoutesSoftwareProgrammingSaga, SoftwareProgrammingSagaData> Create(
            SagaStateAggregate<SoftwareProgrammingSagaData> message)
        {
            return SagaInstance.New(new CustomRoutesSoftwareProgrammingSaga(), message, _log);
        }

        public ISagaInstance<CustomRoutesSoftwareProgrammingSaga, SoftwareProgrammingSagaData> Create(SleptWellEvent message)
        {
            var saga = new CustomRoutesSoftwareProgrammingSaga();
            var data =
                new SagaStateAggregate<SoftwareProgrammingSagaData>(new SoftwareProgrammingSagaData(message.SofaId,
                    saga.Sleeping.Name));
            return SagaInstance.New(saga, data, _log);
        }
    }
}