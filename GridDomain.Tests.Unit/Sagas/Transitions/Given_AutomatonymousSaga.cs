using System;
using Automatonymous;
using GridDomain.EventSourcing.Sagas.InstanceSagas;
using GridDomain.Tests.XUnit.Sagas.SoftwareProgrammingDomain;
using Serilog;

namespace GridDomain.Tests.XUnit.Sagas.Transitions
{
    public class Given_AutomatonymousSaga
    {
        public readonly Saga<SoftwareProgrammingState> SagaInstance;

        public Given_AutomatonymousSaga(Func<SoftwareProgrammingProcess, State> initialState, ILogger logger)
        {
            var sagaMachine = new SoftwareProgrammingProcess();
            var sagaData = new SoftwareProgrammingState(Guid.NewGuid(), initialState(sagaMachine).Name);
            SagaInstance = new Saga<SoftwareProgrammingState>(sagaMachine,
                                                                 sagaData,
                                                                 logger);
        }
    }
}