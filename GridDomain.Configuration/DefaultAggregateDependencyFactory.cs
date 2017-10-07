using System;
using GridDomain.Configuration.MessageRouting;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.CommonDomain;

namespace GridDomain.Configuration {
    public static class DefaultAggregateDependencyFactory
    {
        public static DefaultAggregateDependencyFactory<TAggregate> New<TAggregate>(IAggregateCommandsHandler<TAggregate> handler, IMessageRouteMap mapProducer=null) where TAggregate : IAggregate
        {
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            var map = mapProducer ?? MessageRouteMap.New(handler);

            return new DefaultAggregateDependencyFactory<TAggregate>(() => handler,() => map);
        }
        
        public static DefaultAggregateDependencyFactory<TAggregate> New<TAggregate, TAggregateCommandsHandler>()
            where TAggregate : Aggregate
            where TAggregateCommandsHandler : IAggregateCommandsHandler<TAggregate>, new()
        {
            return New(new TAggregateCommandsHandler());
        }
    }


    public class DefaultAggregateDependencyFactory<TAggregate> : IAggregateDependencyFactory<TAggregate> where TAggregate : IAggregate
    {
        public Func<IMessageRouteMap> MapProducer { protected get; set; }
        public Func<IAggregateCommandsHandler<TAggregate>> HandlerCreator { protected get; set; }
        public Func<ISnapshotsPersistencePolicy> SnapshotPolicyCreator { protected get; set; }
        public Func<IConstructAggregates> AggregateFactoryCreator { protected get; set; }
        public Func<IPersistentChildsRecycleConfiguration> RecycleConfigurationCreator { protected get; set; }

        public DefaultAggregateDependencyFactory(Func<IAggregateCommandsHandler<TAggregate>> handler, Func<IMessageRouteMap> mapProducer = null)
        {
            MapProducer = mapProducer ?? (() => MessageRouteMap.New(handler()));
            HandlerCreator = handler ?? throw new ArgumentNullException(nameof(handler));
            SnapshotPolicyCreator = () => new NoSnapshotsPersistencePolicy();
            AggregateFactoryCreator = () => new AggregateFactory();
            RecycleConfigurationCreator = () => new DefaultPersistentChildsRecycleConfiguration();
        }

        public static DefaultAggregateDependencyFactory<TCommandAggregate> ForCommandAggregate<TCommandAggregate>(Func<IMessageRouteMap> mapProducer = null) where TCommandAggregate : CommandAggregate, TAggregate
        {
            return new DefaultAggregateDependencyFactory<TCommandAggregate>(() => new CommandAggregateHandlerProxy<TCommandAggregate>(Aggregate.Empty<TCommandAggregate>(Guid.Empty)));
        }

        public virtual IAggregateCommandsHandler<TAggregate> CreateCommandsHandler()
        {
            return HandlerCreator();
        }

        public virtual ISnapshotsPersistencePolicy CreatePersistencePolicy()
        {
            return SnapshotPolicyCreator();
        }

        public virtual IConstructAggregates CreateAggregateFactory()
        {
            return AggregateFactoryCreator();
        }

        public virtual IPersistentChildsRecycleConfiguration CreateRecycleConfiguration()
        {
            return RecycleConfigurationCreator();
        }

        public IMessageRouteMap CreateRouteMap()
        {
            return MapProducer();
        }
    }
}