using System;
using Akka.Actor;


using GridDomain.Common;
using GridDomain.CQRS.Messaging;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.CommonDomain;
using GridDomain.Node.Actors;
using GridDomain.Node.Actors.CommandPipe;
using GridDomain.Scheduling.Integration;
using Microsoft.Practices.Unity;

namespace GridDomain.Node.Configuration.Composition
{
    public class AggregateBaseConfiguration<TAggregateActor, TAggregate, TAggregateCommandsHandler> : IContainerConfiguration
        where TAggregate : Aggregate
        where TAggregateCommandsHandler : IAggregateCommandsHandler<TAggregate>
    {
        private readonly IConstructAggregates _factory;

        private readonly Func<ISnapshotsPersistencePolicy> _snapshotsPolicyFactory;

        public AggregateBaseConfiguration(Func<ISnapshotsPersistencePolicy> snapshotsPolicy = null,
                                          Func<IMemento, TAggregate> snapshotsFactory = null)
            : this(snapshotsPolicy, new AggregateSnapshottingFactory<TAggregate>(snapshotsFactory)) {}

        private AggregateBaseConfiguration(Func<ISnapshotsPersistencePolicy> snapshotsPolicy = null,
                                           IConstructAggregates snapshotsFactory = null)
        {
            _factory = snapshotsFactory ?? new AggregateFactory();
            _snapshotsPolicyFactory = snapshotsPolicy ?? (() => new NoSnapshotsPersistencePolicy());
        }

        public void Register(IUnityContainer container)
        {
            container.RegisterType<AggregateHubActor<TAggregate>>();
            container.RegisterType<IAggregateCommandsHandler<TAggregate>, TAggregateCommandsHandler>();

            var aggregateRegistrationName = typeof(TAggregate).Name;
            container.RegisterType<ISnapshotsPersistencePolicy>(aggregateRegistrationName,
                                                                new InjectionFactory(c => _snapshotsPolicyFactory()));

            container.RegisterType<TAggregateActor>(new InjectionConstructor(new ResolvedParameter<IAggregateCommandsHandler<TAggregate>>(),
                                                                             new ResolvedParameter<IActorRef>(SchedulingActor.RegistrationName),
                                                                             new ResolvedParameter<IPublisher>(),
                                                                             new ResolvedParameter<ISnapshotsPersistencePolicy>(aggregateRegistrationName),
                                                                             new ResolvedParameter<IConstructAggregates>(aggregateRegistrationName),
                                                                             new ResolvedParameter<IActorRef>(HandlersPipeActor.CustomHandlersProcessActorRegistrationName)));

            container.RegisterInstance(aggregateRegistrationName, _factory);
        }
    }
}