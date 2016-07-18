namespace GridDomain.EventSourcing.Sagas.InstanceSagas
{
    public interface ISagaState<TState>
    {
        TState CurrentState { get; set; }
    }
}