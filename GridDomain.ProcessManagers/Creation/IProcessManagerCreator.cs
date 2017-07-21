namespace GridDomain.ProcessManagers.Creation {
    public interface IProcessManagerCreator<TState> where TState : IProcessState
    {
        IProcessManager<TState> Create(TState message);
    }
}