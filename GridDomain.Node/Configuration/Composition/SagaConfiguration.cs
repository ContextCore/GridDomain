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
    public static class SagaConfiguration
    {
        public static IContainerConfiguration New<TProcess, TState, TFactoryA>(ISagaDescriptor descriptor, Func<ISagaCreator<TState>> factoryCreator) where TFactoryA : ISagaCreator<TState>
                                                                                                                                                      where TState : class, ISagaState
                                                                                                                                                      where TProcess : Process<TState>
        {
            return New(SagaDependencyFactory.FromSagaCreator<TProcess, TState>(factoryCreator(), descriptor));
        }

        public static IContainerConfiguration New<TProcess, TState>(ISagaDependencyFactory<TProcess, TState> factory) where TState : class, ISagaState
                                                                                                                      where TProcess : Process<TState>
        {
            var registrationName = typeof(TProcess).Name;
            return new SagaConfiguration<TState>(c => factory.CreateCatalog(registrationName),
                                                 registrationName,
                                                 () => factory.StateDependencyFactory.CreatePersistencePolicy(registrationName),
                                                 factory.StateDependencyFactory.CreateFactory(registrationName));
        }
    }

    public class SagaConfiguration<TState> : IContainerConfiguration where TState : class, ISagaState
    {
        private readonly IConstructAggregates _aggregateFactory;
        private readonly Func<ISnapshotsPersistencePolicy> _snapShotsPolicy;
        private readonly Func<IUnityContainer, ISaga—reatorCatalog<TState>> _sagaCatalogCreator;
        private readonly string _sagaProcessName;

        internal SagaConfiguration(Func<IUnityContainer, ISaga—reatorCatalog<TState>> factoryCreator,
                                   string sagaProcessName,
                                   Func<ISnapshotsPersistencePolicy> snapShotsPolicy = null,
                                   IConstructAggregates factory = null)
        {
            _sagaProcessName = sagaProcessName;
            _sagaCatalogCreator = factoryCreator;
            _aggregateFactory = factory ?? new AggregateFactory();
            _snapShotsPolicy = snapShotsPolicy ?? (() => new NoSnapshotsPersistencePolicy());
        }

        private void Register(IUnityContainer container, ISaga—reatorCatalog<TState> catalog)
        {
            var sagaSpecificRegistrationsName = _sagaProcessName;
            container.RegisterInstance(sagaSpecificRegistrationsName, _aggregateFactory);
            container.RegisterInstance<ISaga—reatorCatalog<TState>>(catalog);
            container.RegisterType<SagaActor<TState>>();
            container.Register(AggregateConfiguration.NewSagaState<TState>(_snapShotsPolicy));
        }

        public void Register(IUnityContainer container)
        {
            Register(container, _sagaCatalogCreator(container));
        }
    }
}