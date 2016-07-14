using System;

namespace GridDomain.EventSourcing.Sagas
{
    public interface ISagaFactory<out TSaga, in TStartMessage> where TSaga : ISagaInstance
    {
        TSaga Create(TStartMessage message);
    }
    
}