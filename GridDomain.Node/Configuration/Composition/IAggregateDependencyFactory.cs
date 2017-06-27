using System;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.CommonDomain;
using GridDomain.Node.Actors;

namespace GridDomain.Node.Configuration.Composition
{
    public interface IAggregateDependencyFactory<TAggregate> where TAggregate : Aggregate
    {
        IAggregateCommandsHandler<TAggregate> CreateCommandsHandler(string name);
        ISnapshotsPersistencePolicy CreatePersistencePolicy(string name);
        IConstructAggregates CreateFactory(string name);
        IPersistentChildsRecycleConfiguration CreateRecycleConfiguration(string name);
    }

    public abstract class DefaultDependencyFactory<TAggregate> : IAggregateDependencyFactory<TAggregate> where TAggregate : Aggregate
    {
        public abstract Func<IAggregateCommandsHandler<TAggregate>> HandlerCreator { protected get; set; }
        public Func<ISnapshotsPersistencePolicy> SnapshotPolicyCreator { protected get; set; }
        public Func<IConstructAggregates> AggregateFactoryCreator { protected get; set; }
        public Func<IPersistentChildsRecycleConfiguration> RecycleConfigurationCreator { protected get; set; }

        protected DefaultDependencyFactory()
        {
            SnapshotPolicyCreator = () => new SnapshotsPersistencePolicy();
            AggregateFactoryCreator = () => new AggregateFactory();
            RecycleConfigurationCreator = () => new DefaultPersistentChildsRecycleConfiguration();
        }

        public IAggregateCommandsHandler<TAggregate> CreateCommandsHandler(string name)
        {
            return HandlerCreator();
        }

        public ISnapshotsPersistencePolicy CreatePersistencePolicy(string name)
        {
            return SnapshotPolicyCreator();
        }

        public IConstructAggregates CreateFactory(string name)
        {
            return AggregateFactoryCreator();
        }

        public IPersistentChildsRecycleConfiguration CreateRecycleConfiguration(string name)
        {
            return RecycleConfigurationCreator();
        }
    }

}