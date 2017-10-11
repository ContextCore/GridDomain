using System;
using Automatonymous;
using GridDomain.ProcessManagers;
using GridDomain.Tests.Unit.ProcessManagers.SoftwareProgrammingDomain;

namespace GridDomain.Tests.Unit.ProcessManagers.Transitions
{
    public class Given_Automatonymous_Process
    {
        public readonly Process<SoftwareProgrammingState> Process;
        public SoftwareProgrammingState State;

        public Given_Automatonymous_Process(Func<SoftwareProgrammingProcess, State> initialState)
        {
            var process = new SoftwareProgrammingProcess();
            State = new SoftwareProgrammingState(Guid.NewGuid(), initialState(process).Name);
            Process = new SoftwareProgrammingProcess();
        }
        public Given_Automatonymous_Process(State initialState)
        {
            State = new SoftwareProgrammingState(Guid.NewGuid(), initialState.Name);
            Process = new SoftwareProgrammingProcess();
        }
    }
}