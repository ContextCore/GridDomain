using System;
using System.Reflection;
using Akka.Actor;
using Autofac;
using Autofac.Core;
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
        private readonly  IAggregateCommandsHandler<TAggregate> _commandsHandler;
        private readonly IPersistentChildsRecycleConfiguration _persistencChildsRecycleConfiguration;

        internal AggregateConfiguration(IAggregateCommandsHandler<TAggregate> commandsHandler,
                                        Func<ISnapshotsPersistencePolicy> snapshotsPolicy,
                                        IConstructAggregates snapshotsFactory,
                                        IPersistentChildsRecycleConfiguration persistencChildsRecycleConfiguration)
        {
            _persistencChildsRecycleConfiguration = persistencChildsRecycleConfiguration;
            _factory = snapshotsFactory;
            _snapshotsPolicyFactory = snapshotsPolicy;
            _commandsHandler = commandsHandler;
        }

        public void Register(ContainerBuilder container)
        {
            container.Register<AggregateHubActor<TAggregate>>(c => new AggregateHubActor<TAggregate>(_persistencChildsRecycleConfiguration));

            container.RegisterType<TAggregateActor>()
                     .WithParameters(new Parameter[] { 
                                     new TypedParameter(typeof(IAggregateCommandsHandler<TAggregate>), _commandsHandler),
                                     new ResolvedParameter((pi, ctx) => pi.ParameterType == typeof(IPublisher),
                                         (pi, ctx) => ctx.Resolve<IPublisher>()),
                                     new ResolvedParameter((pi, ctx) => pi.ParameterType == typeof(ISnapshotsPersistencePolicy),
                                         (pi, ctx) => _snapshotsPolicyFactory()),
                                     new TypedParameter(typeof(IConstructAggregates), _factory),
                                     new ResolvedParameter((pi, ctx) => pi.ParameterType == typeof(IActorRef),
                                         (pi, ctx) => ctx.ResolveNamed<IActorRef>(HandlersPipeActor.CustomHandlersProcessActorRegistrationName))
                                });
    }
    }
}