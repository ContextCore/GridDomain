namespace GridDomain.EventSourcing.Sagas.InstanceSagas
{
    public interface ISagaState
    {
       string CurrentStateName { get; set; }
    }
}