namespace GridDomain.EventSourcing.Sagas.InstanceSagas
{
    public interface ISagaState<TState>
    {
       string CurrentState { get; set; }
    }
}