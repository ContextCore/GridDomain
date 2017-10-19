using System;
using GridDomain.Node.Actors.ProcessManagers;
using GridDomain.ProcessManagers;
using GridDomain.ProcessManagers.Creation;
using GridDomain.Tests.Unit.ProcessManagers.SoftwareProgrammingDomain.Events;
using Serilog;

namespace GridDomain.Tests.Unit.ProcessManagers.SoftwareProgrammingDomain
{
    public class SoftwareProgrammingProcessStateFactory : IProcessStateFactory<SoftwareProgrammingState>
    {
        private readonly Guid? _knownCoffeMachineId;

        public SoftwareProgrammingProcessStateFactory(Guid? knownCoffeMachineId=null)
        {
            _knownCoffeMachineId = knownCoffeMachineId;
        }
        public virtual SoftwareProgrammingState Create(object message, SoftwareProgrammingState state)
        {
            switch (message)
            {
                //creating new process instance from a start message
                case SleptWellEvent e: return new SoftwareProgrammingState(Guid.NewGuid(), nameof(SoftwareProgrammingProcess.Coding), Guid.Empty, _knownCoffeMachineId ?? Guid.NewGuid());
                //creating new process instance from a start message
                case GotTiredEvent e: return new SoftwareProgrammingState(Guid.NewGuid(), nameof(SoftwareProgrammingProcess.Coding), Guid.Empty, _knownCoffeMachineId ?? Guid.NewGuid());
            }
            return state;
        }
    }
}