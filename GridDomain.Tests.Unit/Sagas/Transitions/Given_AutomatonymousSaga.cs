using System;
using Automatonymous;
using GridDomain.Processes;
using GridDomain.Tests.Unit.Sagas.SoftwareProgrammingDomain;
using Serilog;

namespace GridDomain.Tests.Unit.Sagas.Transitions
{
    public class Given_AutomatonymousSaga
    {
        public readonly ProcessManager<SoftwareProgrammingState> ProcessManagerInstance;

        public Given_AutomatonymousSaga(Func<SoftwareProgrammingProcess, State> initialState, ILogger logger)
        {
            var sagaMachine = new SoftwareProgrammingProcess();
            var sagaData = new SoftwareProgrammingState(Guid.NewGuid(), initialState(sagaMachine).Name);
            ProcessManagerInstance = new ProcessManager<SoftwareProgrammingState>(sagaMachine,
                                                                 sagaData,
                                                                 logger);
        }
    }
}