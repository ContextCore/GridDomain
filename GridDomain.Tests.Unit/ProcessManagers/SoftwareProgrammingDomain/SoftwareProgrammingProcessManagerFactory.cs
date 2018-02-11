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
        private readonly string _knownCoffeMachineId;

        public SoftwareProgrammingProcessStateFactory(string knownCoffeMachineId=null)
        {
            _knownCoffeMachineId = knownCoffeMachineId;
        }
        public virtual SoftwareProgrammingState Create(object message)
        {
            switch (message)
            {
                //creating new process instance from a start message
                case SleptWellEvent e: return new SoftwareProgrammingState(Guid.NewGuid().ToString(), nameof(SoftwareProgrammingProcess.Coding), null, _knownCoffeMachineId ?? Guid.NewGuid().ToString());
                //creating new process instance from a start message
                case GotTiredEvent e: return new SoftwareProgrammingState(Guid.NewGuid().ToString(), nameof(SoftwareProgrammingProcess.Coding), null, _knownCoffeMachineId ?? Guid.NewGuid().ToString());
            }
            throw new CannotCreateStateFromMessageException(message);
        }
    }

    public class CannotCreateStateFromMessageException : Exception
    {
        public object Msg { get; }

        public CannotCreateStateFromMessageException(object msg)
        {
            Msg = msg;
        }
    }
}