using System;
using GridDomain.Tests.Unit.Sagas.StateSagas.SampleSaga;

namespace GridDomain.Tests.Unit.Sagas.StateSagas.Transitions
{
    class Given_State_SoftareProgramming_Saga
    {
        public readonly SoftwareProgrammingSaga SagaMachine;
        public SoftwareProgrammingSaga SagaInstance => SagaMachine;
        public readonly SoftwareProgrammingSagaState SagaDataAggregate;

        public Given_State_SoftareProgramming_Saga(SoftwareProgrammingSagaState initialState)
        {
            SagaMachine = new SoftwareProgrammingSaga(initialState);
            SagaDataAggregate = initialState;
        }

        public Given_State_SoftareProgramming_Saga(SoftwareProgrammingSaga.States machineState)
            :this(new SoftwareProgrammingSagaState(Guid.NewGuid(),machineState))
        {
        }
    }
}