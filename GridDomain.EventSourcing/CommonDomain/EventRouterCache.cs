using System;
using System.Collections.Concurrent;

namespace GridDomain.EventSourcing.CommonDomain {
    public class EventRouterCache 
    {
        private readonly ConcurrentDictionary<Type, Lazy<IRouteEvents>> _applyCache = new ConcurrentDictionary<Type, Lazy<IRouteEvents>>();

        public IRouteEvents Get(Type aggregateType)
        {
            var router = _applyCache.GetOrAdd(aggregateType, 
                                 t => new Lazy<IRouteEvents>(() => new ConventionEventRouter(aggregateType)));
            return router.Value;
        }

        //TODO: add warm up
        private EventRouterCache()
        {
            
        }
       
        public static EventRouterCache Instance { get; } = new EventRouterCache();
    }
}