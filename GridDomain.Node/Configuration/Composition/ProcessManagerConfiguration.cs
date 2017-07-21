using System;
using GridDomain.Common;
using GridDomain.Configuration;
using GridDomain.CQRS.Messaging;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.CommonDomain;
using GridDomain.Node.Actors;
using GridDomain.Node.Actors.Aggregates;
using GridDomain.Node.Actors.EventSourced;
using GridDomain.Node.Actors.PersistentHub;
using GridDomain.Node.Actors.ProcessManagers;
using GridDomain.ProcessManagers;
using GridDomain.ProcessManagers.Creation;
using GridDomain.ProcessManagers.State;
using Microsoft.Practices.Unity;

namespace GridDomain.Node.Configuration.Composition
{
    internal class ProcessManagerConfiguration<TState> : IContainerConfiguration where TState : class, IProcessState
    {
        private readonly IConstructAggregates _aggregateFactory;
        private readonly Func<ISnapshotsPersistencePolicy> _snapShotsPolicy;
        private readonly Func<IUnityContainer, IProcessManagerCreatorCatalog<TState>> _processManagersCatalogCreator;
        private readonly string _registrationName;
        private readonly IPersistentChildsRecycleConfiguration _persistentChildsRecycleConfiguration;

        internal ProcessManagerConfiguration(Func<IUnityContainer, IProcessManagerCreatorCatalog<TState>> factoryCreator,
                                   string registrationName,
                                   Func<ISnapshotsPersistencePolicy> snapShotsPolicy,
                                   IConstructAggregates factory,
                                   IPersistentChildsRecycleConfiguration configuration)
        {
            _persistentChildsRecycleConfiguration = configuration;
            _registrationName = registrationName;
            _processManagersCatalogCreator = factoryCreator;
            _aggregateFactory = factory;
            _snapShotsPolicy = snapShotsPolicy;
        }

        private void Register(IUnityContainer container, IProcessManagerCreatorCatalog<TState> catalog)
        {
            container.RegisterInstance<IPersistentChildsRecycleConfiguration>(_registrationName, _persistentChildsRecycleConfiguration);
            container.RegisterInstance<IConstructAggregates>(_registrationName, _aggregateFactory);
            container.RegisterInstance<IProcessManagerCreatorCatalog<TState>>(catalog);
            container.RegisterType<ProcessManagerActor<TState>>();

            RegisterStateAggregate<ProcessStateActor<TState>>(container);
            container.RegisterType<ProcessManagerHubActor<TState>>(new InjectionConstructor(new ResolvedParameter<IPersistentChildsRecycleConfiguration>(_registrationName)));

            //for direct access to process state from repositories and for generalization
            RegisterStateAggregate<AggregateActor<ProcessStateAggregate<TState>>>(container);
            container.RegisterType<AggregateHubActor<ProcessStateAggregate<TState>>>(new InjectionConstructor(new ResolvedParameter<IPersistentChildsRecycleConfiguration>(_registrationName)));
        }

        private void RegisterStateAggregate<TStateActorType>(IUnityContainer container)
        {
            container.Register(new AggregateConfiguration<TStateActorType, ProcessStateAggregate<TState>>(c => c.Resolve<ProcessStateCommandHandler<TState>>(),
                                                                                                              _snapShotsPolicy,
                                                                                                              _aggregateFactory,
                                                                                                              _persistentChildsRecycleConfiguration));
        }

        public void Register(IUnityContainer container)
        {
            Register(container, _processManagersCatalogCreator(container));
        }
    }
}