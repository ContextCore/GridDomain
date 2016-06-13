using System;

namespace GridDomain.EventSourcing.Sagas
{
    public interface ISagaFactory<TSaga, TStartMessage> where TSaga : IDomainSaga
    {
        TSaga Create(TStartMessage message);
    }
}