using System;
using Automatonymous;
using GridDomain.EventSourcing.Sagas.InstanceSagas;
using GridDomain.Tests.XUnit.Sagas.SoftwareProgrammingDomain;
using Serilog;

namespace GridDomain.Tests.XUnit.Sagas.Transitions
{
    public class Given_AutomatonymousSaga
    {
        public readonly Saga<SoftwareProgrammingSagaState> SagaInstance;

        public Given_AutomatonymousSaga(Func<SoftwareProgrammingSaga, State> initialState, ILogger logger)
        {
            var sagaMachine = new SoftwareProgrammingSaga();
            var sagaData = new SoftwareProgrammingSagaState(Guid.NewGuid(), initialState(sagaMachine).Name);
            SagaInstance = new Saga<SoftwareProgrammingSagaState>(sagaMachine,
                                                                 sagaData,
                                                                 logger);
        }
    }
}