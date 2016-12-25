using System;
using GridDomain.EventSourcing.Sagas;
using GridDomain.Tests.Unit.Sagas.SoftwareProgrammingDomain.Events;

namespace GridDomain.Tests.Unit.Sagas.StateSagas.SampleSaga
{
    public class SoftwareProgrammingSagaFactory : 
        ISagaFactory<SoftwareProgrammingSaga, SoftwareProgrammingSagaState>,
        ISagaFactory<SoftwareProgrammingSaga, GotTiredEvent>,
        ISagaFactory<SoftwareProgrammingSaga, Guid>
    {
        public SoftwareProgrammingSaga Create(SoftwareProgrammingSagaState message)
        {
            return new SoftwareProgrammingSaga(message);
        }

        public SoftwareProgrammingSaga Create(GotTiredEvent message)
        {
            return new SoftwareProgrammingSaga(new SoftwareProgrammingSagaState(message.SagaId,
                SoftwareProgrammingSaga.States.Coding));
        }

        public SoftwareProgrammingSaga Create(Guid id)
        {
            return new SoftwareProgrammingSaga(new SoftwareProgrammingSagaState(id,SoftwareProgrammingSaga.States.Coding));
        }
    }
}