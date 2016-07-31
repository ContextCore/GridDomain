using CommonDomain.Core;
using GridDomain.Common;
using GridDomain.CQRS.Messaging.MessageRouting;
using GridDomain.Node.Actors;
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
        public void Register(IUnityContainer container)
        {
            container.RegisterType<AggregateActor<TAggregate>>();
            container.RegisterType<AggregateHubActor<TAggregate>>();
            container.RegisterType<ICommandAggregateLocator<TAggregate>, TCommandAggregateLocator>();
            container.RegisterType<IAggregateCommandsHandler<TAggregate>, TAggregateCommandsHandler>();
        }
    }
}