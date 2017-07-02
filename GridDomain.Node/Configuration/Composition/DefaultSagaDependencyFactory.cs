using System;
using GridDomain.CQRS.Messaging;
using GridDomain.EventSourcing.Sagas;
using GridDomain.EventSourcing.Sagas.InstanceSagas;

namespace GridDomain.Node.Configuration.Composition {
    public class DefaultSagaDependencyFactory<TProcess, TState> : ISagaDependencyFactory<TProcess, TState>
        where TState : class, ISagaState
        where TProcess : Process<TState>
    {
        private readonly ISagaCreatorCatalog<TState> _saga—reatorCatalog;
        public Func<IMessageRouteMap> RouteMapCreator { get; set; }
       
        public DefaultSagaDependencyFactory(ISagaCreatorCatalog<TState> catalog)
        {
            _saga—reatorCatalog = catalog;
        }
        public DefaultSagaDependencyFactory(ISagaCreator<TState> creator, ISagaDescriptor descriptor):this(BuildCatalog(creator,descriptor))
        {
            RouteMapCreator = () => MessageRouteMap.New(descriptor);
        }

        public ISagaCreatorCatalog<TState> CreateCatalog()
        {
            return _saga—reatorCatalog;
        }

        private static Saga—reatorsCatalog<TState> BuildCatalog(ISagaCreator<TState> factoryCreator, ISagaDescriptor descriptor)
        {
            var producer = new Saga—reatorsCatalog<TState>(descriptor, factoryCreator);
            producer.RegisterAll(factoryCreator);
            return producer;
        }
     
        public virtual SagaStateDependencyFactory<TState> StateDependencyFactory { get; } = new SagaStateDependencyFactory<TState>();

        IAggregateDependencyFactory<SagaStateAggregate<TState>> ISagaDependencyFactory<TProcess, TState>.StateDependencyFactory => StateDependencyFactory;
        public virtual IMessageRouteMap CreateRouteMap()
        {
            return RouteMapCreator();
        }
    }
}