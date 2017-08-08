using System;

namespace GridDomain.ProcessManagers.Creation
{
    public interface IProcessManagerCreator<TState, in TStartMessage> where TState : IProcessState
    {
        IProcessManager<TState> CreateNew(TStartMessage message, Guid? processId = null);
    }
}