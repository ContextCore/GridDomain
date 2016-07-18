using System;
using GridDomain.EventSourcing.Sagas;
using GridDomain.Tests.Sagas.StateSagas.SampleSaga.Events;

namespace GridDomain.Tests.Sagas.StateSagas.SampleSaga
{
    class SoftwareProgrammingSagaFactory : 
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
                SoftwareProgrammingSaga.States.Working));
        }

        public SoftwareProgrammingSaga Create(Guid id)
        {
            return new SoftwareProgrammingSaga(new SoftwareProgrammingSagaState(Guid.Empty,SoftwareProgrammingSaga.States.Working));
        }
    }
}