using System;

namespace GridDomain.EventSourcing.Sagas
{
    public interface ISagaFactory<out TSaga, in TStartMessage> where TSaga : IDomainSaga
    {
        TSaga Create(TStartMessage message);
    }
    
}