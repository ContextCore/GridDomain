using System;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.Sagas.InstanceSagas;
using Microsoft.Practices.Unity;

namespace GridDomain.Node.Configuration.Composition {
    public class DomainBuilder : IDomainBuilder
    {
        private readonly IUnityContainer _unityContainer;

        public DomainBuilder(IUnityContainer container)
        {
            _unityContainer = container;
        }

        public void RegisterSaga<TState, TSaga>(ISagaDependencyFactory<TState, TSaga> factory) where TState : class, ISagaState
                                                                                               where TSaga : Process<TState>
        {
            _unityContainer.Register(SagaConfiguration.New(factory));
        }

        public void RegisterAggregate<TAggregate>(IAggregateDependencyFactory<TAggregate> factory) where TAggregate : Aggregate
        {
            throw new NotImplementedException();
        }
    }
}