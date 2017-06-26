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
    public interface IDomainBuilder
    {
        void RegisterSaga<TState, TSaga>(ISagaDependencyFactory<TState, TSaga> factory) where TSaga : Process<TState>
                                                                                        where TState : class, ISagaState;

        void RegisterAggregate<TAggregate>(IAggregateDependencyFactory<TAggregate> factory) where TAggregate : Aggregate;
    }

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

    public static class SagaConfiguration
    {
        public static SagaConfiguration<TSaga, TState> New<TSaga, TState, TFactoryA>(ISagaDescriptor descriptor,
                                                                                     Func<ISnapshotsPersistencePolicy> s = null,
                                                                                     IConstructAggregates factory = null) where TFactoryA : ISagaCreator<TState>
                                                                                                                          where TState : class, ISagaState
                                                                                                                          where TSaga : Process<TState>
        {
            return new SagaConfiguration<TSaga, TState>(c => CreateCatalog(c.Resolve<TFactoryA>(), descriptor),
                                                        typeof(TSaga).Name,
                                                        s,
                                                        factory);
        }

        public static SagaConfiguration<TSaga, TState> New<TSaga, TState>(ISagaDependencyFactory<TState, TSaga> factory) where TState : class, ISagaState
                                                                                                                         where TSaga : Process<TState>
        {
            var registrationName = typeof(TSaga).Name;
            return new SagaConfiguration<TSaga, TState>(c => factory.CreateCatalog(registrationName),
                                                        registrationName,
                                                        () => factory.StateDependencyFactory.CreatePersistencePolicy(registrationName),
                                                        factory.StateDependencyFactory.CreateFactory(registrationName));
        }

        private static Saga—reatorsCatalog<TState> CreateCatalog<TState>(ISagaCreator<TState> factoryCreator, ISagaDescriptor descriptor) where TState : class, ISagaState
        {
            var producer = new Saga—reatorsCatalog<TState>(descriptor, factoryCreator);
            producer.RegisterAll(factoryCreator);
            return producer;
        }
    }

    public class SagaConfiguration<TSaga, TState> : IContainerConfiguration where TSaga : Process<TState>
                                                                            where TState : class, ISagaState
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
            container.Register(new SagaStateConfiguration<TState>(_snapShotsPolicy));
        }

        public void Register(IUnityContainer container)
        {
            Register(container, _sagaCatalogCreator(container));
        }
    }
}