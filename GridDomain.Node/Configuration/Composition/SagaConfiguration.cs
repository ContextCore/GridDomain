using System;
using GridDomain.Common;
using GridDomain.CQRS.Messaging;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.CommonDomain;
using GridDomain.EventSourcing.Sagas;
using GridDomain.EventSourcing.Sagas.InstanceSagas;
using GridDomain.Node.Actors;
using Microsoft.Practices.Unity;

namespace GridDomain.Node.Configuration.Composition
{
    internal class SagaConfiguration<TState> : IContainerConfiguration where TState : class, ISagaState
    {
        private readonly IConstructAggregates _aggregateFactory;
        private readonly Func<ISnapshotsPersistencePolicy> _snapShotsPolicy;
        private readonly Func<IUnityContainer, ISagaCreatorCatalog<TState>> _sagaCatalogCreator;
        private readonly string _registrationName;
        private readonly IPersistentChildsRecycleConfiguration _persistentChildsRecycleConfiguration;

        internal SagaConfiguration(Func<IUnityContainer, ISagaCreatorCatalog<TState>> factoryCreator,
                                   string registrationName,
                                   Func<ISnapshotsPersistencePolicy> snapShotsPolicy,
                                   IConstructAggregates factory,
                                   IPersistentChildsRecycleConfiguration configuration)
        {
            _persistentChildsRecycleConfiguration = configuration;
            _registrationName = registrationName;
            _sagaCatalogCreator = factoryCreator;
            _aggregateFactory = factory;
            _snapShotsPolicy = snapShotsPolicy;
        }

        private void Register(IUnityContainer container, ISagaCreatorCatalog<TState> catalog)
        {
            container.RegisterInstance<IPersistentChildsRecycleConfiguration>(_registrationName, _persistentChildsRecycleConfiguration);
            container.RegisterInstance<IConstructAggregates>(_registrationName, _aggregateFactory);
            container.RegisterInstance<ISagaCreatorCatalog<TState>>(catalog);
            container.RegisterType<SagaActor<TState>>();

            RegisterStateAggregate<SagaStateActor<TState>>(container);
            container.RegisterType<SagaHubActor<TState>>(new InjectionConstructor(new ResolvedParameter<IPersistentChildsRecycleConfiguration>(_registrationName)));

            //for direct access to saga state from repositories and for generalization
            RegisterStateAggregate<AggregateActor<SagaStateAggregate<TState>>>(container);
            container.RegisterType<AggregateHubActor<SagaStateAggregate<TState>>>(new InjectionConstructor(new ResolvedParameter<IPersistentChildsRecycleConfiguration>(_registrationName)));
        }

        private void RegisterStateAggregate<TStateActorType>(IUnityContainer container)
        {
            container.Register(new AggregateConfiguration<TStateActorType, SagaStateAggregate<TState>>(c => c.Resolve<SagaStateCommandHandler<TState>>(),
                                                                                                              _snapShotsPolicy,
                                                                                                              _aggregateFactory,
                                                                                                              _persistentChildsRecycleConfiguration));
        }

        public void Register(IUnityContainer container)
        {
            Register(container, _sagaCatalogCreator(container));
        }
    }
}