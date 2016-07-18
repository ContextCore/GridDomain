using System;

namespace GridDomain.EventSourcing.Sagas
{
    [Obsolete("Not needed, was replaced by AggregateFactory")]
    public interface IEmptySagaFactory<TSaga> where TSaga : ISagaInstance
    {
        TSaga Create();
    }
}