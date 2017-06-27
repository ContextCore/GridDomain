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
        private readonly Func<IUnityContainer, ISaga—reatorCatalog<TState>> _sagaCatalogCreator;
        private readonly string RegistrationName;
        private readonly IPersistentChildsRecycleConfiguration _persistentChildsRecycleConfiguration;

        internal SagaConfiguration(Func<IUnityContainer, ISaga—reatorCatalog<TState>> factoryCreator,
                                   string registrationName,
                                   Func<ISnapshotsPersistencePolicy> snapShotsPolicy,
                                   IConstructAggregates factory,
                                   IPersistentChildsRecycleConfiguration configuration)
        {
            _persistentChildsRecycleConfiguration = configuration;
            RegistrationName = registrationName;
            _sagaCatalogCreator = factoryCreator;
            _aggregateFactory = factory;
            _snapShotsPolicy = snapShotsPolicy;
        }

        private void Register(IUnityContainer container, ISaga—reatorCatalog<TState> catalog)
        {
            container.RegisterInstance<IPersistentChildsRecycleConfiguration>(RegistrationName, _persistentChildsRecycleConfiguration);
            container.RegisterInstance<IConstructAggregates>(RegistrationName, _aggregateFactory);
            container.RegisterInstance<ISaga—reatorCatalog<TState>>(catalog);
            container.RegisterType<SagaActor<TState>>();
            container.Register(AggregateConfiguration.NewSagaState<TState>(_snapShotsPolicy));

            container.RegisterType<SagaHubActor<TState>>(new InjectionConstructor(new ResolvedParameter<IPersistentChildsRecycleConfiguration>(RegistrationName)));
        }

        public void Register(IUnityContainer container)
        {
            Register(container, _sagaCatalogCreator(container));
        }
    }
}