using System;


namespace GridDomain.EventSourcing.CommonDomain
{
    public interface IConstructAggregates
    {
        IAggregate Build(Type type, Guid id, IMemento snapshot=null);
    }
}