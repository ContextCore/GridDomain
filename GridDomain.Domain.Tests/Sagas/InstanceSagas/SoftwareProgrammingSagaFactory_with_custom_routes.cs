using GridDomain.EventSourcing.Sagas;
using GridDomain.EventSourcing.Sagas.InstanceSagas;
using GridDomain.Tests.Sagas.SoftwareProgrammingDomain.Events;

namespace GridDomain.Tests.Sagas.InstanceSagas
{
    class SoftwareProgrammingSagaFactory_with_custom_routes :
        ISagaFactory<ISagaInstance<CustomRoutesSoftwareProgrammingSaga, SoftwareProgrammingSagaData>, SagaDataAggregate<SoftwareProgrammingSagaData>>,
        ISagaFactory<ISagaInstance<CustomRoutesSoftwareProgrammingSaga, SoftwareProgrammingSagaData>, GotTiredEvent>,
        ISagaFactory<ISagaInstance<CustomRoutesSoftwareProgrammingSaga, SoftwareProgrammingSagaData>, SleptWellEvent>
    {
        public ISagaInstance<CustomRoutesSoftwareProgrammingSaga, SoftwareProgrammingSagaData> Create(SagaDataAggregate<SoftwareProgrammingSagaData> message)
        {
            return SagaInstance.New(new CustomRoutesSoftwareProgrammingSaga(), message);
        }

        public ISagaInstance<CustomRoutesSoftwareProgrammingSaga, SoftwareProgrammingSagaData> Create(GotTiredEvent message)
        {
            var saga = new CustomRoutesSoftwareProgrammingSaga();
            var data = new SagaDataAggregate<SoftwareProgrammingSagaData>(message.SagaId,
                new SoftwareProgrammingSagaData(saga.Coding.Name));
            return SagaInstance.New(saga, data);
        }

        public ISagaInstance<CustomRoutesSoftwareProgrammingSaga, SoftwareProgrammingSagaData> Create(SleptWellEvent message)
        {
            var saga = new CustomRoutesSoftwareProgrammingSaga();
            var data = new SagaDataAggregate<SoftwareProgrammingSagaData>(message.SagaId,
                new SoftwareProgrammingSagaData(saga.Sleeping.Name));
            return SagaInstance.New(saga, data);
        }
    }
}