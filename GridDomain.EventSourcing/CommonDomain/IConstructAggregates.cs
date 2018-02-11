using System;


namespace GridDomain.EventSourcing.CommonDomain
{
    public interface IConstructAggregates
    {
        IAggregate Build(Type type, string id, IMemento snapshot=null);
    }
    
    public interface IConstructSnapshots
    {
        IMemento GetSnapshot(IAggregate aggregate);
    }

    public static class ConstructAggregateExtensions
    {
        public static T Build<T>(this IConstructAggregates constructor, string id, IMemento snapshot = null) where T:IAggregate
        {
            return (T) constructor.Build(typeof(T), id, snapshot);
        }
        
    }
}