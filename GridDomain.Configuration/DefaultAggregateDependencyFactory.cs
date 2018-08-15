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

            return new AggregateDependencies<TAggregate>(() => handler,() => map);
        }
        
        public static AggregateDependencies<TAggregate> New<TAggregate, TAggregateCommandsHandler>()
            where TAggregate : IAggregate
            where TAggregateCommandsHandler : class, IAggregateCommandsHandler<TAggregate>, new()
        {
            return New(new TAggregateCommandsHandler());
        }

        public static AggregateDependencies<TCommandAggregate> ForCommandAggregate<TCommandAggregate>(IAggregateFactory factory = null) where TCommandAggregate : CommandAggregate
        {
            var depFactory = new AggregateDependencies<TCommandAggregate>(() => CommandAggregateHandler.New<TCommandAggregate>(factory));
            if (factory != null)
                depFactory.AggregateFactoryCreator = () => factory;
            return depFactory;
        }
    }


    public class AggregateDependencies<TAggregate> : IAggregateDependencies<TAggregate> where TAggregate : IAggregate
    {
        public Func<IMessageRouteMap> MapProducer { protected get; set; }
        public Func<IAggregateCommandsHandler<TAggregate>> HandlerCreator { protected get; set; }
        public Func<ISnapshotsPersistencePolicy> SnapshotPolicyCreator { protected get; set; }
        public Func<IAggregateFactory> AggregateFactoryCreator { protected get; set; }
        public Func<IConstructSnapshots> SnapshotsFactoryCreator { protected get; set; }
        public Func<IRecycleConfiguration> RecycleConfigurationCreator { protected get; set; }

        public AggregateDependencies(Func<IAggregateCommandsHandler<TAggregate>> handler, 
                                                 Func<IMessageRouteMap> mapProducer = null)
        {
            MapProducer = mapProducer ?? (() => MessageRouteMap.New(handler()));
            HandlerCreator = handler ?? throw new ArgumentNullException(nameof(handler));
            SnapshotPolicyCreator = () => new NoSnapshotsPersistencePolicy();
            AggregateFactoryCreator = () => AggregateFactory.Default;
            SnapshotsFactoryCreator = () => AggregateFactory.Default;
            RecycleConfigurationCreator = () => new DefaultRecycleConfiguration();
        }

       

        public virtual IAggregateCommandsHandler<TAggregate> CreateCommandsHandler()
        {
            return HandlerCreator();
        }

        public virtual ISnapshotsPersistencePolicy CreatePersistencePolicy()
        {
            return SnapshotPolicyCreator();
        }

        public virtual IAggregateFactory CreateAggregateFactory()
        {
            return AggregateFactoryCreator();
        }

        public IConstructSnapshots CreateSnapshotsFactory()
        {
            return SnapshotsFactoryCreator();
        }

        public virtual IRecycleConfiguration CreateRecycleConfiguration()
        {
            return RecycleConfigurationCreator();
        }

        public IMessageRouteMap CreateRouteMap()
        {
            return MapProducer();
        }
    }
}