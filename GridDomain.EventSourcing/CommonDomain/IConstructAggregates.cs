using System;


namespace GridDomain.EventSourcing.CommonDomain
{
    public interface IConstructAggregates
    {
        IAggregate Build(Type type, string id, IMemento snapshot=null);
    }
}