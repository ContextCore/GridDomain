using System;
using System.Collections.Concurrent;

namespace GridDomain.EventSourcing.CommonDomain {
    public class EventRouterCache 
    {
        private readonly ConcurrentDictionary<Type, Lazy<IRouteEvents>> _applyCache = new ConcurrentDictionary<Type, Lazy<IRouteEvents>>();

        public IRouteEvents Get(IAggregate aggregate)
        {
            var type = aggregate.GetType();
            var router = _applyCache.GetOrAdd(type, 
                t => new Lazy<IRouteEvents>(() => new ConventionEventRouter(aggregate)));
            return router.Value;
        }

        //TODO: add warm up
        private EventRouterCache()
        {
            
        }
       
        public static EventRouterCache Instance { get; } = new EventRouterCache();
    }
}