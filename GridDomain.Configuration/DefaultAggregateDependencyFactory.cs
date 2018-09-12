using System;
using GridDomain.Configuration.MessageRouting;
using GridDomain.Configuration.SnapshotPolicies;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.CommonDomain;

namespace GridDomain.Configuration {
    public static class AggregateDependencies
    {
        public static AggregateDependencies<TAggregate> New<TAggregate>(IAggregateCommandsHandlerDescriptor handler, IMessageRouteMap mapProducer=null) where TAggregate : IAggregate
        {
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            var map = mapProducer ?? MessageRouteMap.New(handler);

            return new AggregateDependencies<TAggregate>(handler,map);
        }
        
//        public static AggregateDependencies<TAggregate> New<TAggregate, TDescriptor>()
//            where TAggregate : IAggregate
//            where TDescriptor : class
//        {
//            return New(new TDescriptor());
//        }

        public static AggregateDependencies<TCommandAggregate> ForCommandAggregate<TCommandAggregate>(IAggregateFactory factory = null) where TCommandAggregate : ConventionAggregate
        {
            var depFactory = new AggregateDependencies<TCommandAggregate>(null);
            if (factory != null)
                depFactory.AggregateFactory = factory;
            return depFactory;
        }
    }


    public class AggregateDependencies<TAggregate> : IAggregateDependencies<TAggregate> where TAggregate : IAggregate
    {
        private readonly IMessageRouteMap _routeMap;

        public AggregateDependencies(IAggregateCommandsHandlerDescriptor descriptor,
                                     IMessageRouteMap mapProducer = null)
        {
            _routeMap = mapProducer ?? MessageRouteMap.New(descriptor);
        }


        public ISnapshotsPersistencePolicy SnapshotPolicy { get; set; }  = new NoSnapshotsPersistencePolicy();
        public IAggregateFactory AggregateFactory { get; set; } = EventSourcing.AggregateFactory.Default;

        public ISnapshotFactory SnapshotFactory { get; set; }= EventSourcing.AggregateFactory.Default;

        public IRecycleConfiguration RecycleConfiguration { get; set; }= new DefaultRecycleConfiguration();
        
        public IMessageRouteMap CreateRouteMap()
        {
            return _routeMap;
        }
    }
}