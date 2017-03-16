using GridDomain.EventSourcing.Sagas;
using GridDomain.EventSourcing.Sagas.InstanceSagas;
using GridDomain.Tests.XUnit.Sagas.SoftwareProgrammingDomain;
using GridDomain.Tests.XUnit.Sagas.SoftwareProgrammingDomain.Events;
using Serilog;

namespace GridDomain.Tests.XUnit.Sagas.CustomRoutesSoftwareProgrammingDomain
{
    public class CustomRoutesSoftwareProgrammingSagaFactory :
        ISagaFactory
        <ISaga<SoftwareProgrammingSagaData>,
            SagaStateAggregate<SoftwareProgrammingSagaData>>,
        ISagaFactory<ISaga<SoftwareProgrammingSagaData>, GotTiredEvent>,
        ISagaFactory<ISaga<SoftwareProgrammingSagaData>, SleptWellEvent>
    {
        private readonly ILogger _log;

        public CustomRoutesSoftwareProgrammingSagaFactory(ILogger log)
        {
            _log = log;
        }

        public ISaga<SoftwareProgrammingSagaData> Create(GotTiredEvent message)
        {
            var saga = new CustomRoutesSoftwareProgrammingSaga();
            var data =
                new SagaStateAggregate<SoftwareProgrammingSagaData>(new SoftwareProgrammingSagaData(message.PersonId,
                                                                                                    saga.Coding.Name));
            return Saga.New(saga, data, _log);
        }

        public ISaga<SoftwareProgrammingSagaData> Create(
            SagaStateAggregate<SoftwareProgrammingSagaData> message)
        {
            return Saga.New(new CustomRoutesSoftwareProgrammingSaga(), message, _log);
        }

        public ISaga<SoftwareProgrammingSagaData> Create(SleptWellEvent message)
        {
            var saga = new CustomRoutesSoftwareProgrammingSaga();
            var data =
                new SagaStateAggregate<SoftwareProgrammingSagaData>(new SoftwareProgrammingSagaData(message.SofaId,
                                                                                                    saga.Sleeping.Name));
            return Saga.New(saga, data, _log);
        }
    }
}