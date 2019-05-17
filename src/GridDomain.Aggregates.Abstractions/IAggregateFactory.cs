using System;

namespace GridDomain.Aggregates.Abstractions
{
    public interface IAggregateFactory
    {
        IAggregate Build(Type type, string id);
    }
    
    public interface IAggregateFactory<out T> where T:IAggregate
    {
        T Build(string id=null);
    }

    
}