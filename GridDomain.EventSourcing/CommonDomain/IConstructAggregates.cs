using System;


namespace GridDomain.EventSourcing.CommonDomain
{
    public interface IAggregateFactory
    {
        IAggregate Build(Type type, string id, IMemento snapshot=null);
    }
}