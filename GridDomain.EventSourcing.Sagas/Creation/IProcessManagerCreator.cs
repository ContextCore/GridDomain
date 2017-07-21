namespace GridDomain.Processes.Creation {
    public interface IProcessManagerCreator<TState> where TState : IProcessState
    {
        IProcessManager<TState> Create(TState message);
    }
}