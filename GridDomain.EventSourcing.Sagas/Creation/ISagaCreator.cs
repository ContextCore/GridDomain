using System;

namespace GridDomain.Processes.Creation
{
    public interface IProcessManagerCreator<TState, in TStartMessage> where TState : IProcessState
    {
        IProcessManager<TState> CreateNew(TStartMessage message, Guid? id = null);
    }
}