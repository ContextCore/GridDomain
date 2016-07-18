using System;
using GridDomain.EventSourcing.Sagas;

namespace GridDomain.Tests.Sagas.StateSagas.SampleSaga
{
    public class SoftwareProgrammingSagaState :
        SagaStateAggregate<SoftwareProgrammingSaga.States, SoftwareProgrammingSaga.Triggers>
    {
        public SoftwareProgrammingSagaState(Guid id) : base(id)
        {
        }

        public SoftwareProgrammingSagaState(Guid id, SoftwareProgrammingSaga.States state) : base(id, state)
        {
        }
    }
}