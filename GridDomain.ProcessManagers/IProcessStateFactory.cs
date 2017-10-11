namespace GridDomain.ProcessManagers {
    public interface IProcessStateFactory<TState> where TState : IProcessState
    {
        TState Create(object message, TState state);
    }
}