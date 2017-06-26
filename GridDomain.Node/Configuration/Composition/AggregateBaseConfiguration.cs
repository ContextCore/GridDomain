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
        public static AggregateBaseConfiguration<TAggregateActor, TAggregate, TAggregateCommandsHandler>
            New<TAggregateActor, TAggregate, TAggregateCommandsHandler>(IAggregateDependencyFactory<TAggregate> factory)
            where TAggregate : Aggregate
            where TAggregateCommandsHandler : IAggregateCommandsHandler<TAggregate>
        {
            var registrationName = typeof(TAggregate).Name;
            return new AggregateBaseConfiguration<TAggregateActor, TAggregate, TAggregateCommandsHandler>(() => factory.CreatePersistencePolicy(registrationName),
                                                                                                          factory.CreateFactory(registrationName));
        }
    
        public static AggregateBaseConfiguration<TAggregateActor, TAggregate, TAggregateCommandsHandler>
            New<TAggregateActor, TAggregate, TAggregateCommandsHandler>(Func<ISnapshotsPersistencePolicy> snapshotsPolicy=null,
                                                                        Func<IMemento, TAggregate> snapshotsFactory=null)
            where TAggregate : Aggregate
            where TAggregateCommandsHandler : IAggregateCommandsHandler<TAggregate>
        {
            return new AggregateBaseConfiguration<TAggregateActor, TAggregate, TAggregateCommandsHandler>(snapshotsPolicy,
                                                                                                          new AggregateSnapshottingFactory<TAggregate>(snapshotsFactory));
        }

        public static AggregateBaseConfiguration<AggregateActor<TAggregate>, TAggregate, TAggregateCommandsHandler>
            New<TAggregate, TAggregateCommandsHandler>(Func<ISnapshotsPersistencePolicy> snapshotsPolicy = null,
                                                                        Func<IMemento, TAggregate> snapshotsFactory = null)
            where TAggregate : Aggregate
            where TAggregateCommandsHandler : IAggregateCommandsHandler<TAggregate>
        {
            return new AggregateBaseConfiguration<AggregateActor<TAggregate>, TAggregate, TAggregateCommandsHandler>(snapshotsPolicy,
                                                                                                          new AggregateSnapshottingFactory<TAggregate>(snapshotsFactory));
        }

        public static AggregateBaseConfiguration<SagaStateActor<TState>, SagaStateAggregate<TState>, SagaStateCommandHandler<TState>>
            NewSagaState<TState>(Func<ISnapshotsPersistencePolicy> snapshotsPolicy,
                                 Func<IMemento, SagaStateAggregate<TState>> snapshotsFactory = null)
            where TState : ISagaState
        {
            return AggregateConfiguration.New<SagaStateActor<TState>, SagaStateAggregate<TState>, SagaStateCommandHandler<TState>>(snapshotsPolicy,
                                                                                                                                       snapshotsFactory);
        }

    }

    public class AggregateBaseConfiguration<TAggregateActor, TAggregate, TAggregateCommandsHandler> : IContainerConfiguration
        where TAggregate : Aggregate
        where TAggregateCommandsHandler : IAggregateCommandsHandler<TAggregate>
    {
        private readonly IConstructAggregates _factory;

        private readonly Func<ISnapshotsPersistencePolicy> _snapshotsPolicyFactory;
        private readonly IAggregateDependencyFactory<TAggregate> _aggregateDependencyFactory;
        private static readonly string RegistrationName = typeof(TAggregate).Name;

        internal AggregateBaseConfiguration(Func<ISnapshotsPersistencePolicy> snapshotsPolicy = null,
                                            IConstructAggregates snapshotsFactory = null)
        {
            _factory = snapshotsFactory ?? new AggregateFactory();
            _snapshotsPolicyFactory = snapshotsPolicy ?? (() => new NoSnapshotsPersistencePolicy());
        }

        public void Register(IUnityContainer container)
        {
            container.RegisterType<AggregateHubActor<TAggregate>>();

            if (_aggregateDependencyFactory == null)
                container.RegisterType<IAggregateCommandsHandler<TAggregate>, TAggregateCommandsHandler>();
            else
                container.RegisterInstance<IAggregateCommandsHandler<TAggregate>>(
                                                                                  _aggregateDependencyFactory.CreateCommandsHandler(RegistrationName));


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