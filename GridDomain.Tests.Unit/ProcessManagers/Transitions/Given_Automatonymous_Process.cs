using System;
using Automatonymous;
using GridDomain.ProcessManagers;
using GridDomain.Tests.Unit.ProcessManagers.SoftwareProgrammingDomain;
using Serilog;

namespace GridDomain.Tests.Unit.ProcessManagers.Transitions
{
    public class Given_Automatonymous_Process
    {
        public readonly ProcessManager<SoftwareProgrammingState> ProcessManagerInstance;

        public Given_Automatonymous_Process(Func<SoftwareProgrammingProcess, State> initialState, ILogger logger)
        {
            var process = new SoftwareProgrammingProcess();
            var state = new SoftwareProgrammingState(Guid.NewGuid(), initialState(process).Name);
            ProcessManagerInstance = new ProcessManager<SoftwareProgrammingState>(process,
                                                                 state,
                                                                 logger);
        }
    }
}