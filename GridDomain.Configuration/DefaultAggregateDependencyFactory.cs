using System;
using GridDomain.Configuration.MessageRouting;
using GridDomain.Configuration.SnapshotPolicies;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.CommonDomain;

namespace GridDomain.Configuration {
    public static class AggregateDependencies
    {
        public static AggregateDependencies<TAggregate> New<TAggregate>(IAggregateCommandsHandler<TAggregate> handler, IMessageRouteMap mapProducer=null) where TAggregate : IAggregate
        {
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            var map = mapProducer ?? MessageRouteMap.New(handler);

            return new AggregateDependencies<TAggregate>(handler,map);
        }
        
        public static AggregateDependencies<TAggregate> New<TAggregate, TAggregateCommandsHandler>()
            where TAggregate : IAggregate
            where TAggregateCommandsHandler : class, IAggregateCommandsHandler<TAggregate>, new()
        {
            return New(new TAggregateCommandsHandler());
        }

        public static AggregateDependencies<TCommandAggregate> ForCommandAggregate<TCommandAggregate>(IAggregateFactory factory = null) where TCommandAggregate : CommandAggregate
        {
            var depFactory = new AggregateDependencies<TCommandAggregate>(CommandAggregateHandler.New<TCommandAggregate>(factory));
            if (factory != null)
                depFactory.AggregateFactory = factory;
            return depFactory;
        }
    }


    public class AggregateDependencies<TAggregate> : IAggregateDependencies<TAggregate> where TAggregate : IAggregate
    {
        private readonly IMessageRouteMap _routeMap;

        public AggregateDependencies(IAggregateCommandsHandler<TAggregate> handler, 
                                     IMessageRouteMap mapProducer = null)
        {
            CommandHandler = handler ?? throw new ArgumentNullException(nameof(handler));
            _routeMap = mapProducer ?? MessageRouteMap.New(handler);
        }

        public IAggregateCommandsHandler<TAggregate> CommandHandler { get; set; }

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