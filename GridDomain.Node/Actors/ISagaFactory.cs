using GridDomain.EventSourcing.Sagas;

namespace GridDomain.Node.Actors
{
    public interface ISagaFactory<TSaga, TStartMessage> where TSaga : IDomainSaga
    {
        TSaga Create(TStartMessage message);
    }
}