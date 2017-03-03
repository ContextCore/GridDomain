using CommonDomain.Core;
using GridDomain.Common;
using GridDomain.CQRS.Messaging.MessageRouting;
using GridDomain.EventSourcing;
using Microsoft.Practices.Unity;

namespace GridDomain.Node.Configuration.Composition
{
    public static class ContainerExtensions
    {
        public static void Register(this IUnityContainer container, IContainerConfiguration configuration)
        {
            configuration.Register(container);
        }

        public static void RegisterAggregate<TAggregate, TCommandsHandler>(this IUnityContainer container)
            where TCommandsHandler : IAggregateCommandsHandler<TAggregate> where TAggregate : Aggregate
        {
            new AggregateConfiguration<TAggregate, TCommandsHandler>().Register(container);
        }
    }
}