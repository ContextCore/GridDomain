using CommonDomain.Core;
using GridDomain.Common;
using GridDomain.CQRS.Messaging;
using GridDomain.CQRS.Messaging.Akka;
using GridDomain.CQRS.Messaging.MessageRouting;
using GridDomain.Node.Actors;
using GridDomain.Scheduling.Akka.Messages;
using Microsoft.Practices.Unity;

namespace GridDomain.Node.Configuration.Composition
{


    public class AggregateConfiguration<TAggregate, TCommandAggregateLocator> :
        AggregateConfiguration<TAggregate, TCommandAggregateLocator, TCommandAggregateLocator> where TAggregate : AggregateBase where TCommandAggregateLocator : ICommandAggregateLocator<TAggregate>, IAggregateCommandsHandler<TAggregate>
    {
        
    }

    public class AggregateConfiguration<TAggregate, TCommandAggregateLocator, TAggregateCommandsHandler>:
        IContainerConfiguration
        where TCommandAggregateLocator : ICommandAggregateLocator<TAggregate>
        where TAggregateCommandsHandler : IAggregateCommandsHandler<TAggregate>
        where TAggregate : AggregateBase
    {
        private readonly SnapshotsSavePolicy _snapshotsPolicy;

        public AggregateConfiguration(SnapshotsSavePolicy snapshotsPolicy = null)
        {
            _snapshotsPolicy = snapshotsPolicy ?? new DefaultSnapshotsSavePolicy();
        }

        public void Register(IUnityContainer container)
        {
            container.RegisterType<AggregateHubActor<TAggregate>>();
            container.RegisterType<ICommandAggregateLocator<TAggregate>, TCommandAggregateLocator>();
            container.RegisterType<IAggregateCommandsHandler<TAggregate>, TAggregateCommandsHandler>();
            container.RegisterType<AggregateActor<TAggregate>>(
                new InjectionConstructor(
                    new ResolvedParameter<IAggregateCommandsHandler<TAggregate>>(),
                    new ResolvedParameter<TypedMessageActor<ScheduleCommand>>(),
                    new ResolvedParameter<TypedMessageActor<Unschedule>>(),
                    new ResolvedParameter<IPublisher>(),
                    _snapshotsPolicy
                    ));
        }
    }
}