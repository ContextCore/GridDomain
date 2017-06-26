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

    public static class AggregateBaseConfiguration
    {
       //public AggregateBaseConfiguration(Type aggregateType, IAggregateDependencyFactory factory)
       //{
       //    
       //}
    }
    public class AggregateBaseConfiguration<TAggregateActor, TAggregate, TAggregateCommandsHandler> : IContainerConfiguration
        where TAggregate : Aggregate
        where TAggregateCommandsHandler : IAggregateCommandsHandler<TAggregate>
    {
        private readonly IConstructAggregates _factory;

        private readonly Func<ISnapshotsPersistencePolicy> _snapshotsPolicyFactory;
        private readonly IAggregateDependencyFactory<TAggregate> _aggregateDependencyFactory;
        private static readonly string RegistrationName = typeof(TAggregate).Name;

        public AggregateBaseConfiguration(Func<ISnapshotsPersistencePolicy> snapshotsPolicy = null,
                                          Func<IMemento, TAggregate> snapshotsFactory = null)
            : this(snapshotsPolicy, new AggregateSnapshottingFactory<TAggregate>(snapshotsFactory)) {}


        public AggregateBaseConfiguration(IAggregateDependencyFactory<TAggregate> factory)
            :this(() => factory.CreatePersistencePolicy(RegistrationName),
                  factory.CreateFactory(RegistrationName))
        {
            _aggregateDependencyFactory = factory;
        }

        private AggregateBaseConfiguration(Func<ISnapshotsPersistencePolicy> snapshotsPolicy = null,
                                           IConstructAggregates snapshotsFactory = null)
        {
            _factory = snapshotsFactory ?? new AggregateFactory();
            _snapshotsPolicyFactory = snapshotsPolicy ?? (() => new NoSnapshotsPersistencePolicy());
        }

        public void Register(IUnityContainer container)
        {
            container.RegisterType<AggregateHubActor<TAggregate>>();

            if(_aggregateDependencyFactory == null)
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