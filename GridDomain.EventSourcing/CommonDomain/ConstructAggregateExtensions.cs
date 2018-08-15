namespace GridDomain.EventSourcing.CommonDomain {
    public static class ConstructAggregateExtensions
    {
        public static T Build<T>(this IAggregateFactory constructor, string id, IMemento snapshot = null) where T:IAggregate
        {
            return (T) constructor.Build(typeof(T), id, snapshot);
        }
        
    }
}