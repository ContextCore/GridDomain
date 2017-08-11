using System;
using Akka.Actor;
using Autofac;
using GridDomain.Common;
using GridDomain.Configuration;
using GridDomain.Configuration.MessageRouting;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.CommonDomain;
using GridDomain.Node.Actors;
using GridDomain.Node.Actors.Aggregates;
using GridDomain.Node.Actors.CommandPipe;
using GridDomain.Node.Actors.EventSourced;
using GridDomain.Node.Actors.PersistentHub;

namespace GridDomain.Node.Configuration.Composition
{
    internal class AggregateConfiguration<TAggregateActor, TAggregate> : IContainerConfiguration
        where TAggregate : Aggregate
    {
        private readonly IConstructAggregates _factory;
        private readonly Func<ISnapshotsPersistencePolicy> _snapshotsPolicyFactory;
        private static readonly string RegistrationName = typeof(TAggregate).BeautyName();
        private readonly Func<ContainerBuilder, IAggregateCommandsHandler<TAggregate>> _commandsHandlerCreator;
        private readonly IPersistentChildsRecycleConfiguration _persistencChildsRecycleConfiguration;

        internal AggregateConfiguration(Func<IContainer, IAggregateCommandsHandler<TAggregate>> commandsHandlerCreator,
                                        Func<ISnapshotsPersistencePolicy> snapshotsPolicy,
                                        IConstructAggregates snapshotsFactory,
                                        IPersistentChildsRecycleConfiguration persistencChildsRecycleConfiguration)
        {
            _persistencChildsRecycleConfiguration = persistencChildsRecycleConfiguration;
            _factory = snapshotsFactory;
            _snapshotsPolicyFactory = snapshotsPolicy;
            _commandsHandlerCreator = commandsHandlerCreator;
        }

        public void Register(ContainerBuilder container)
        {
            container.RegisterInstance(_persistencChildsRecycleConfiguration).Named<IPersistentChildsRecycleConfiguration>(RegistrationName);
            container.Register<AggregateHubActor<TAggregate>>(c => 
            new AggregateHubActor<TAggregate>(c.ResolveNamed<IPersistentChildsRecycleConfiguration>(RegistrationName)));

            container.RegisterInstance<IAggregateCommandsHandler<TAggregate>>(_commandsHandlerCreator(container));
            container.Register<Func<ISnapshotsPersistencePolicy>>(c => () => _snapshotsPolicyFactory())
                     .Named<Func<ISnapshotsPersistencePolicy>>(RegistrationName);

            container.Register<TAggregateActor>(c => 
                
                c.Resolve<TAggregateActor>(c.Resolve<<IAggregateCommandsHandler<TAggregate>>>()))

                (new InjectionConstructor(new ResolvedParameter(),
                                                                             new ResolvedParameter<IPublisher>(),
                                                                             new ResolvedParameter<ISnapshotsPersistencePolicy>(RegistrationName),
                                                                             new ResolvedParameter<IConstructAggregates>(RegistrationName),
                                                                             new ResolvedParameter<IActorRef>(HandlersPipeActor.CustomHandlersProcessActorRegistrationName)));

            container.RegisterInstance(RegistrationName, _factory);
        }
    }
}