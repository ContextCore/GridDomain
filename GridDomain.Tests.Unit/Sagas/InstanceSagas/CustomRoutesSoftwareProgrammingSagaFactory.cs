using GridDomain.EventSourcing.Sagas;
using GridDomain.EventSourcing.Sagas.InstanceSagas;
using GridDomain.Tests.Unit.Sagas.SoftwareProgrammingDomain.Events;

namespace GridDomain.Tests.Unit.Sagas.InstanceSagas
{
    class CustomRoutesSoftwareProgrammingSagaFactory :
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
            var data = new SagaDataAggregate<SoftwareProgrammingSagaData>(message.PersonId,
                                          new SoftwareProgrammingSagaData(saga.Coding.Name));
            return SagaInstance.New(saga, data);
        }

        public ISagaInstance<CustomRoutesSoftwareProgrammingSaga, SoftwareProgrammingSagaData> Create(SleptWellEvent message)
        {
            var saga = new CustomRoutesSoftwareProgrammingSaga();
            var data = new SagaDataAggregate<SoftwareProgrammingSagaData>(message.SofaId,
                new SoftwareProgrammingSagaData(saga.Sleeping.Name));
            return SagaInstance.New(saga, data);
        }
    }
}