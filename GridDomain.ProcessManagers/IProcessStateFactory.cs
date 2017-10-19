namespace GridDomain.ProcessManagers {
    public interface IProcessStateFactory<out TState> where TState : IProcessState
    {
        TState Create(object message);
    }
}