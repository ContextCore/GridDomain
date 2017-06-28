using System;
using GridDomain.CQRS.Messaging;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.CommonDomain;
using GridDomain.Node.Actors;

namespace GridDomain.Node.Configuration.Composition
{
    public interface IRouteMapFactory
    {
        IMessageRouteMap CreateRouteMap();
    }
    public interface IAggregateDependencyFactory<TAggregate> : IRouteMapFactory where TAggregate : Aggregate
    {
        IAggregateCommandsHandler<TAggregate> CreateCommandsHandler();
        ISnapshotsPersistencePolicy CreatePersistencePolicy();
        IConstructAggregates CreateFactory();
        IPersistentChildsRecycleConfiguration CreateRecycleConfiguration();
    }

    public static class DefaultAggregateDependencyFactory
    {
        public static DefaultAggregateDependencyFactory<TAggregate> New<TAggregate>(IAggregateCommandsHandler<TAggregate> handler, IMessageRouteMap mapProducer) where TAggregate : Aggregate
        {
            var map = mapProducer ?? throw new ArgumentNullException(nameof(mapProducer));
            if(mapProducer==null)
                throw new ArgumentNullException(nameof(handler));

            return new DefaultAggregateDependencyFactory<TAggregate>(() => handler,() => map);
        }
    }
    public class DefaultAggregateDependencyFactory<TAggregate> : IAggregateDependencyFactory<TAggregate> where TAggregate : Aggregate
    {
        public Func<IMessageRouteMap> MapProducer{protected get;set;}
        public Func<IAggregateCommandsHandler<TAggregate>> HandlerCreator { protected get; set; }
        public Func<ISnapshotsPersistencePolicy> SnapshotPolicyCreator { protected get; set; }
        public Func<IConstructAggregates> AggregateFactoryCreator { protected get; set; }
        public Func<IPersistentChildsRecycleConfiguration> RecycleConfigurationCreator { protected get; set; }

        public DefaultAggregateDependencyFactory(Func<IAggregateCommandsHandler<TAggregate>> handler, Func<IMessageRouteMap> mapProducer)
        {
            MapProducer = mapProducer ?? throw new ArgumentNullException(nameof(mapProducer));
            HandlerCreator = handler ?? throw new ArgumentNullException(nameof(handler));
            SnapshotPolicyCreator = () => new NoSnapshotsPersistencePolicy();
            AggregateFactoryCreator = () => new AggregateFactory();
            RecycleConfigurationCreator = () => new DefaultPersistentChildsRecycleConfiguration();
        }

        public virtual IAggregateCommandsHandler<TAggregate> CreateCommandsHandler()
        {
            return HandlerCreator();
        }

        public virtual ISnapshotsPersistencePolicy CreatePersistencePolicy()
        {
            return SnapshotPolicyCreator();
        }

        public virtual IConstructAggregates CreateFactory()
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