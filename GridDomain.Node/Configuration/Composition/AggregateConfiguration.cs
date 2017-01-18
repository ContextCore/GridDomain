using System;
using Akka.Actor;
using CommonDomain;
using CommonDomain.Core;
using CommonDomain.Persistence;
using GridDomain.Common;
using GridDomain.CQRS.Messaging;
using GridDomain.CQRS.Messaging.Akka;
using GridDomain.CQRS.Messaging.MessageRouting;
using GridDomain.EventSourcing;
using GridDomain.Node.Actors;
using GridDomain.Node.Actors.CommandPipe;
using GridDomain.Scheduling.Akka.Messages;
using GridDomain.Scheduling.Integration;
using Microsoft.Practices.Unity;

namespace GridDomain.Node.Configuration.Composition
{


    public class AggregateConfiguration<TAggregate, TCommandAggregateLocator> :
        AggregateConfiguration<TAggregate, TCommandAggregateLocator, TCommandAggregateLocator> 
        where TAggregate : AggregateBase where TCommandAggregateLocator : ICommandAggregateLocator<TAggregate>, IAggregateCommandsHandler<TAggregate>
    {
        public AggregateConfiguration(Func<ISnapshotsPersistencePolicy> snapshotsPolicy, IConstructAggregates snapshotsFactory) : base(snapshotsPolicy,snapshotsFactory)
        {
            
        }

        public AggregateConfiguration(Func<ISnapshotsPersistencePolicy> snapshotsPolicy, Func<IMemento,TAggregate> snapshotsFactory) : base(snapshotsPolicy, 
                                                                                                           new AggregateSnapshottingFactory<TAggregate>(snapshotsFactory))
        {

        }


        public AggregateConfiguration() : base(null,null)
        {

        }
    }

    public class AggregateConfiguration<TAggregate, TCommandAggregateLocator, TAggregateCommandsHandler>:
                     IContainerConfiguration
                     where TCommandAggregateLocator : ICommandAggregateLocator<TAggregate>
                     where TAggregateCommandsHandler : IAggregateCommandsHandler<TAggregate>
                     where TAggregate : AggregateBase
    {
        private readonly Func<ISnapshotsPersistencePolicy> _snapshotsPolicyFactory;
        private readonly IConstructAggregates _factory;

        public AggregateConfiguration(Func<ISnapshotsPersistencePolicy> snapshotsPolicy = null, IConstructAggregates snapshotsFactory = null)
        {
            _factory = snapshotsFactory ?? new AggregateFactory();
            _snapshotsPolicyFactory = snapshotsPolicy ?? (() => new NoSnapshotsPersistencePolicy());
        }

        public void Register(IUnityContainer container)
        {
            container.RegisterType<AggregateHubActor<TAggregate>>();
            container.RegisterType<ICommandAggregateLocator<TAggregate>, TCommandAggregateLocator>();
            container.RegisterType<IAggregateCommandsHandler<TAggregate>, TAggregateCommandsHandler>();

            var aggregateRegistrationName = typeof(TAggregate).Name;
            container.RegisterType<ISnapshotsPersistencePolicy>(aggregateRegistrationName, new InjectionFactory(c => _snapshotsPolicyFactory()));
            
            container.RegisterType<AggregateActor<TAggregate>>(
                                    new InjectionConstructor(
                                        new ResolvedParameter<IAggregateCommandsHandler<TAggregate>>(),
                                        new ResolvedParameter<IActorRef>(SchedulingActor.RegistrationName),
                                        new ResolvedParameter<IPublisher>(),
                                        new ResolvedParameter<ISnapshotsPersistencePolicy>(aggregateRegistrationName),
                                        new ResolvedParameter<IConstructAggregates>(aggregateRegistrationName),
                                        new ResolvedParameter<IActorRef>(HandlersProcessActor.CustomHandlersProcessActorRegistrationName)
                                        ));

            container.RegisterInstance(aggregateRegistrationName, _factory);
        }
    }
}