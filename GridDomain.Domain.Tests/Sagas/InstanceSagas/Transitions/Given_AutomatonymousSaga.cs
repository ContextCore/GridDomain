using System;
using Automatonymous;
using GridDomain.EventSourcing.Sagas.InstanceSagas;

namespace GridDomain.Tests.Sagas.InstanceSagas.Transitions
{
    class Given_AutomatonymousSaga
    {
        public readonly SoftwareProgrammingSaga SagaMachine = new SoftwareProgrammingSaga();
        public readonly SagaInstance<SoftwareProgrammingSaga,SoftwareProgrammingSagaData> SagaInstance;
        public readonly SagaDataAggregate<SoftwareProgrammingSagaData> SagaDataAggregate;

        public Given_AutomatonymousSaga(Func<SoftwareProgrammingSaga, State> initialState)
        {
            var sagaData = new SoftwareProgrammingSagaData(initialState(SagaMachine));
            SagaDataAggregate = new SagaDataAggregate<SoftwareProgrammingSagaData>(Guid.NewGuid(), sagaData);
            SagaInstance = new SagaInstance<SoftwareProgrammingSaga, SoftwareProgrammingSagaData>(SagaMachine, SagaDataAggregate);
        }
    }
}