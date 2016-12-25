using GridDomain.EventSourcing.Sagas;
using GridDomain.EventSourcing.Sagas.InstanceSagas;
using GridDomain.Tests.Unit.Sagas.SoftwareProgrammingDomain.Events;

namespace GridDomain.Tests.Unit.Sagas.InstanceSagas
{
    public class SoftwareProgrammingSagaFactory:
             ISagaFactory<ISagaInstance<SoftwareProgrammingSaga, SoftwareProgrammingSagaData>, SagaDataAggregate<SoftwareProgrammingSagaData>>,
             ISagaFactory<ISagaInstance<SoftwareProgrammingSaga, SoftwareProgrammingSagaData>, GotTiredEvent>,
             ISagaFactory<ISagaInstance<SoftwareProgrammingSaga, SoftwareProgrammingSagaData>, SleptWellEvent>
    {
        public ISagaInstance<SoftwareProgrammingSaga, SoftwareProgrammingSagaData> Create(SagaDataAggregate<SoftwareProgrammingSagaData> message)
        {
           return SagaInstance.New(new SoftwareProgrammingSaga(), message);
        }

        public ISagaInstance<SoftwareProgrammingSaga, SoftwareProgrammingSagaData> Create(GotTiredEvent message)
        {
            var data = new SagaDataAggregate<SoftwareProgrammingSagaData>(message.SagaId,
                                                                          new SoftwareProgrammingSagaData(
                                                                              nameof(SoftwareProgrammingSaga.Coding)));
            return Create(data);
        }

        public ISagaInstance<SoftwareProgrammingSaga, SoftwareProgrammingSagaData> Create(SleptWellEvent message)
        {
            var data = new SagaDataAggregate<SoftwareProgrammingSagaData>(message.SagaId,
                                                                          new SoftwareProgrammingSagaData(
                                                                              nameof(SoftwareProgrammingSaga.Coding)));
            return Create(data);
        }
    }
}