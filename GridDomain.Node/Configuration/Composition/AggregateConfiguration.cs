using System;
using Akka.Actor;
using GridDomain.Common;
using GridDomain.CQRS.Messaging;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.CommonDomain;
using GridDomain.EventSourcing.Sagas.InstanceSagas;
using GridDomain.Node.Actors;
using GridDomain.Node.Actors.CommandPipe;
using GridDomain.Scheduling.Integration;
using Microsoft.Practices.Unity;

namespace GridDomain.Node.Configuration.Composition
{
    public static class AggregateConfiguration
    {
        public static IContainerConfiguration New<TAggregate>(IAggregateDependencyFactory<TAggregate> factory) where TAggregate : Aggregate
        {
            var registrationName = typeof(TAggregate).Name;
            return new AggregateConfiguration<AggregateActor<TAggregate>, TAggregate>(c => factory.CreateCommandsHandler(registrationName),
                                                                                      () => factory.CreatePersistencePolicy(registrationName),
                                                                                      factory.CreateFactory(registrationName));
        }

        public static IContainerConfiguration
            New<TAggregate, TAggregateCommandsHandler>(Func<ISnapshotsPersistencePolicy> snapshotsPolicy = null,
                                                       Func<IMemento, TAggregate> snapshotsFactory = null)
            where TAggregate : Aggregate
            where TAggregateCommandsHandler : IAggregateCommandsHandler<TAggregate>
        {
            return new AggregateConfiguration<AggregateActor<TAggregate>, TAggregate>(c => c.Resolve<TAggregateCommandsHandler>(),
                                                                                      snapshotsPolicy,
                                                                                      new AggregateSnapshottingFactory<TAggregate>(snapshotsFactory));
        }

        public static IContainerConfiguration NewSagaState<TState>(Func<ISnapshotsPersistencePolicy> snapshotsPolicy,
                                                                   Func<IMemento, SagaStateAggregate<TState>> snapshotsFactory = null)
            where TState : ISagaState
        {
            return new AggregateConfiguration<SagaStateActor<TState>, SagaStateAggregate<TState>>(c => c.Resolve<SagaStateCommandHandler<TState>>(),
                                                                                                  snapshotsPolicy,
                                                                                                  new AggregateSnapshottingFactory<SagaStateAggregate<TState>>(snapshotsFactory));
        }
    }

    class AggregateConfiguration<TAggregateActor, TAggregate> : IContainerConfiguration
        where TAggregate : Aggregate
    {
        private readonly IConstructAggregates _factory;
        private readonly Func<ISnapshotsPersistencePolicy> _snapshotsPolicyFactory;
        private static readonly string RegistrationName = typeof(TAggregate).Name;
        private readonly Func<IUnityContainer, IAggregateCommandsHandler<TAggregate>> _commandsHandlerCreator;

        internal AggregateConfiguration(Func<IUnityContainer, IAggregateCommandsHandler<TAggregate>> commandsHandlerCreator,
                                        Func<ISnapshotsPersistencePolicy> snapshotsPolicy = null,
                                        IConstructAggregates snapshotsFactory = null)
        {
            _factory = snapshotsFactory ?? new AggregateFactory();
            _snapshotsPolicyFactory = snapshotsPolicy ?? (() => new NoSnapshotsPersistencePolicy());
            _commandsHandlerCreator = commandsHandlerCreator;
        }

        public void Register(IUnityContainer container)
        {
            container.RegisterType<AggregateHubActor<TAggregate>>();
            container.RegisterInstance<IAggregateCommandsHandler<TAggregate>>(_commandsHandlerCreator(container));
            container.RegisterType<ISnapshotsPersistencePolicy>(RegistrationName,
                                                                new InjectionFactory(c => _snapshotsPolicyFactory()));

            container.RegisterType<TAggregateActor>(new InjectionConstructor(new ResolvedParameter<IAggregateCommandsHandler<TAggregate>>(),
                                                                             new ResolvedParameter<IActorRef>(SchedulingActor.RegistrationName),
                                                                             new ResolvedParameter<IPublisher>(),
                                                                             new ResolvedParameter<ISnapshotsPersistencePolicy>(RegistrationName),
                                                                             new ResolvedParameter<IConstructAggregates>(RegistrationName),
                                                                             new ResolvedParameter<IActorRef>(HandlersPipeActor.CustomHandlersProcessActorRegistrationName)));

            container.RegisterInstance(RegistrationName, _factory);
        }
    }
}