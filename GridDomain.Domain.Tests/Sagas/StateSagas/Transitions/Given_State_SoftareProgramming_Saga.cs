using System;
using Automatonymous;
using GridDomain.Tests.Sagas.StateSagas.SampleSaga;


namespace GridDomain.Tests.Sagas.StateSagas.Transitions
{
    class Given_State_SoftareProgramming_Saga
    {
        public readonly SoftwareProgrammingSaga SagaMachine;
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