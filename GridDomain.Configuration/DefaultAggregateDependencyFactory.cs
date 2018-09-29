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
            return new AggregateDependencies<TAggregate>(mapProducer);
        }
        
        public static AggregateDependencies<TCommandAggregate> ForCommandAggregate<TCommandAggregate>(IAggregateFactory factory = null) where TCommandAggregate : ConventionAggregate
        {
            var depFactory = new AggregateDependencies<TCommandAggregate>();
            if (factory != null)
                depFactory.AggregateFactory = factory;
            return depFactory;
        }
    }


    public class AggregateDependencies<TAggregate> : IAggregateDependencies<TAggregate> where TAggregate : IAggregate
    {
        private readonly IMessageRouteMap _routeMap;

        public AggregateDependencies(IMessageRouteMap mapProducer = null)
        {
            _routeMap = mapProducer ?? new CustomRouteMap(r => r.RegisterAggregate(new AggregateCommandsHandlerDescriptor<TAggregate>()));
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