using System;
using Automatonymous;
using GridDomain.EventSourcing.Sagas.InstanceSagas;

namespace GridDomain.Tests.Unit.Sagas.InstanceSagas.Transitions
{
    class Given_AutomatonymousSaga
    {
        public readonly SoftwareProgrammingSaga SagaMachine = new SoftwareProgrammingSaga();
        public readonly SagaInstance<SoftwareProgrammingSaga,SoftwareProgrammingSagaData> SagaInstance;
        public readonly SagaStateAggregate<SoftwareProgrammingSagaData> SagaDataAggregate;

        public Given_AutomatonymousSaga(Func<SoftwareProgrammingSaga, State> initialState)
        {
            var sagaData = new SoftwareProgrammingSagaData(Guid.NewGuid(),initialState(SagaMachine).Name);
            SagaDataAggregate = new SagaStateAggregate<SoftwareProgrammingSagaData>(sagaData);
            SagaInstance = new SagaInstance<SoftwareProgrammingSaga, SoftwareProgrammingSagaData>(SagaMachine, SagaDataAggregate);
        }
    }
}