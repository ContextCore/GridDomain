using System;
using Akka.Actor;
using GridDomain.Common;
using GridDomain.Configuration;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.CommonDomain;
using GridDomain.Node.Actors;
using GridDomain.Node.Actors.Aggregates;
using GridDomain.Node.Actors.CommandPipe;
using GridDomain.Node.Actors.EventSourced;
using GridDomain.Node.Actors.PersistentHub;
using GridDomain.Routing;
using GridDomain.Scheduling.Akka;
using Microsoft.Practices.Unity;

namespace GridDomain.Node.Configuration.Composition
{
    internal class AggregateConfiguration<TAggregateActor, TAggregate> : IContainerConfiguration
        where TAggregate : Aggregate
    {
        private readonly IConstructAggregates _factory;
        private readonly Func<ISnapshotsPersistencePolicy> _snapshotsPolicyFactory;
        private static readonly string RegistrationName = typeof(TAggregate).BeautyName();
        private readonly Func<IUnityContainer, IAggregateCommandsHandler<TAggregate>> _commandsHandlerCreator;
        private readonly IPersistentChildsRecycleConfiguration _persistencChildsRecycleConfiguration;

        internal AggregateConfiguration(Func<IUnityContainer, IAggregateCommandsHandler<TAggregate>> commandsHandlerCreator,
                                        Func<ISnapshotsPersistencePolicy> snapshotsPolicy,
                                        IConstructAggregates snapshotsFactory,
                                        IPersistentChildsRecycleConfiguration persistencChildsRecycleConfiguration)
        {
            _persistencChildsRecycleConfiguration = persistencChildsRecycleConfiguration;
            _factory = snapshotsFactory;
            _snapshotsPolicyFactory = snapshotsPolicy;
            _commandsHandlerCreator = commandsHandlerCreator;
        }

        public void Register(IUnityContainer container)
        {
            container.RegisterInstance(RegistrationName, _persistencChildsRecycleConfiguration);
            container.RegisterType<AggregateHubActor<TAggregate>>(new InjectionConstructor(new ResolvedParameter<IPersistentChildsRecycleConfiguration>(RegistrationName)));

            container.RegisterInstance<IAggregateCommandsHandler<TAggregate>>(_commandsHandlerCreator(container));
            container.RegisterType<ISnapshotsPersistencePolicy>(RegistrationName,
                                                                new InjectionFactory(c => _snapshotsPolicyFactory()));

            container.RegisterType<TAggregateActor>(new InjectionConstructor(new ResolvedParameter<IAggregateCommandsHandler<TAggregate>>(),
                                                                             new ResolvedParameter<IPublisher>(),
                                                                             new ResolvedParameter<ISnapshotsPersistencePolicy>(RegistrationName),
                                                                             new ResolvedParameter<IConstructAggregates>(RegistrationName),
                                                                             new ResolvedParameter<IActorRef>(HandlersPipeActor.CustomHandlersProcessActorRegistrationName)));

            container.RegisterInstance(RegistrationName, _factory);
        }
    }
}