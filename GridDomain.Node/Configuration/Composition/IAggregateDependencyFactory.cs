using System;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.CommonDomain;
using GridDomain.Node.Actors;

namespace GridDomain.Node.Configuration.Composition
{
    public interface IAggregateDependencyFactory<TAggregate> where TAggregate : Aggregate
    {
        IAggregateCommandsHandler<TAggregate> CreateCommandsHandler();
        ISnapshotsPersistencePolicy CreatePersistencePolicy();
        IConstructAggregates CreateFactory();
        IPersistentChildsRecycleConfiguration CreateRecycleConfiguration();
    }

    public abstract class DefaultAggregateDependencyFactory<TAggregate> : IAggregateDependencyFactory<TAggregate> where TAggregate : Aggregate
    {
        public abstract Func<IAggregateCommandsHandler<TAggregate>> HandlerCreator { protected get; set; }
        public Func<ISnapshotsPersistencePolicy> SnapshotPolicyCreator { protected get; set; }
        public Func<IConstructAggregates> AggregateFactoryCreator { protected get; set; }
        public Func<IPersistentChildsRecycleConfiguration> RecycleConfigurationCreator { protected get; set; }

        protected DefaultAggregateDependencyFactory()
        {
            SnapshotPolicyCreator = () => new SnapshotsPersistencePolicy();
            AggregateFactoryCreator = () => new AggregateFactory();
            RecycleConfigurationCreator = () => new DefaultPersistentChildsRecycleConfiguration();
        }

        public IAggregateCommandsHandler<TAggregate> CreateCommandsHandler()
        {
            return HandlerCreator();
        }

        public ISnapshotsPersistencePolicy CreatePersistencePolicy()
        {
            return SnapshotPolicyCreator();
        }

        public IConstructAggregates CreateFactory()
        {
            return AggregateFactoryCreator();
        }

        public IPersistentChildsRecycleConfiguration CreateRecycleConfiguration()
        {
            return RecycleConfigurationCreator();
        }
    }

}