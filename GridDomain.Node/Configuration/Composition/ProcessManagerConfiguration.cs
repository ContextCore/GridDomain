using System;
using Autofac;
using GridDomain.Common;
using GridDomain.Configuration;
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

namespace GridDomain.Node.Configuration.Composition
{
    internal class ProcessManagerConfiguration<TState> : IContainerConfiguration where TState : class, IProcessState
    {
        private readonly IConstructAggregates _aggregateFactory;
        private readonly Func<ISnapshotsPersistencePolicy> _snapShotsPolicy;
        private readonly Func<IProcessManagerCreatorCatalog<TState>> _processManagersCatalogCreator;
        private readonly string _registrationName;
        private readonly IPersistentChildsRecycleConfiguration _persistentChildsRecycleConfiguration;

        internal ProcessManagerConfiguration(Func<IProcessManagerCreatorCatalog<TState>> factoryCreator,
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

        private void Register(ContainerBuilder container, IProcessManagerCreatorCatalog<TState> catalog)
        {
            container.RegisterInstance<IProcessManagerCreatorCatalog<TState>>(catalog);
            container.RegisterType<ProcessManagerActor<TState>>();

            RegisterStateAggregate<ProcessStateActor<TState>>(container);
            container.Register<ProcessManagerHubActor<TState>>(c => new ProcessManagerHubActor<TState>(_persistentChildsRecycleConfiguration));//new InjectionConstructor(new ResolvedParameter<IPersistentChildsRecycleConfiguration>(_registrationName))));

            //for direct access to process state from repositories and for generalization
            RegisterStateAggregate<AggregateActor<ProcessStateAggregate<TState>>>(container);
            container.Register<AggregateHubActor<ProcessStateAggregate<TState>>>(c => new AggregateHubActor<ProcessStateAggregate<TState>>(_persistentChildsRecycleConfiguration));
          
        }

        private void RegisterStateAggregate<TStateActorType>(ContainerBuilder container)
        {
            container.Register(new AggregateConfiguration<TStateActorType, ProcessStateAggregate<TState>>(new ProcessStateCommandHandler<TState>(),
                                                                                                              _snapShotsPolicy,
                                                                                                              _aggregateFactory,
                                                                                                              _persistentChildsRecycleConfiguration));
        }

        public void Register(ContainerBuilder container)
        {
            Register(container, _processManagersCatalogCreator());
        }
    }
}