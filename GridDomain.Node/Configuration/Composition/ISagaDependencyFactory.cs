using System;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.CommonDomain;
using GridDomain.EventSourcing.Sagas;
using GridDomain.EventSourcing.Sagas.InstanceSagas;

namespace GridDomain.Node.Configuration.Composition {
    public interface ISagaDependencyFactory<TProcess, TState> where TState : class, ISagaState
                                                             where TProcess : Process<TState>
    {
        ISaga—reatorCatalog<TState> CreateCatalog(string name);
        IAggregateDependencyFactory<SagaStateAggregate<TState>> StateDependencyFactory { get; }
    }

    public static class SagaDependencyFactory
    {
        public static ISagaDependencyFactory<TProcess, TState> FromSagaCreator<TProcess, TState>(ISagaCreator<TState> creator, ISagaDescriptor descriptor)
            where TState : class, ISagaState
            where TProcess : Process<TState>
        {
            return new DefaultSagaDependencyFactory<TProcess, TState>(CreateCatalog(creator, descriptor));
        }

        private static Saga—reatorsCatalog<TState> CreateCatalog<TState>(ISagaCreator<TState> factoryCreator, ISagaDescriptor descriptor) where TState : class, ISagaState
        {
            var producer = new Saga—reatorsCatalog<TState>(descriptor, factoryCreator);
            producer.RegisterAll(factoryCreator);
            return producer;
        }
    }
    public class DefaultSagaDependencyFactory<TProcess, TState> : ISagaDependencyFactory<TProcess, TState>
        where TState : class, ISagaState
        where TProcess : Process<TState>
    {
        private readonly ISaga—reatorCatalog<TState> _saga—reatorCatalog;

        public DefaultSagaDependencyFactory(ISaga—reatorCatalog<TState> catalog)
        {
            _saga—reatorCatalog = catalog;
        }
        public ISaga—reatorCatalog<TState> CreateCatalog(string name)
        {
            return _saga—reatorCatalog;
        }

        public virtual IAggregateDependencyFactory<SagaStateAggregate<TState>> StateDependencyFactory { get; } = new SagaStateDependencyFactory<TState>();
    }

    public class SagaStateDependencyFactory<TState> : DefaultDependencyFactory<SagaStateAggregate<TState>> where TState : ISagaState
    {
        public override Func<IAggregateCommandsHandler<SagaStateAggregate<TState>>> HandlerCreator { protected get; set; }
            = () => new SagaStateCommandHandler<TState>();
    }

}