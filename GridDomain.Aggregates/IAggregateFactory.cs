using System;

namespace GridDomain.Aggregates
{
    public interface IAggregateFactory
    {
        IAggregate Build(Type type, string id);
    }
    
    public interface IAggregateFactory<T> where T:IAggregate
    {
        T Build(string id=null);
    }

    
}