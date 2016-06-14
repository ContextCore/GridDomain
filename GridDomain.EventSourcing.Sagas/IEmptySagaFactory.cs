namespace GridDomain.EventSourcing.Sagas
{
    public interface IEmptySagaFactory<TSaga> where TSaga : IDomainSaga
    {
        TSaga Create();
    }
}