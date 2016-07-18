namespace GridDomain.EventSourcing.Sagas
{
    public interface IEmptySagaFactory<TSaga> where TSaga : ISagaInstance
    {
        TSaga Create();
    }
}