using CommonDomain.Core;
using GridDomain.CQRS.Messaging.MessageRouting;
using GridDomain.Node.Configuration.Composition;
using Microsoft.Practices.Unity;

namespace GridDomain.Node
{
    public static class ContainerExtensions
    {
        public static void Register(this IUnityContainer container, IContainerConfiguration configuration)
        {
            configuration.Register(container);
        }

        public static void Register<TConfiguration>(this IUnityContainer container) where TConfiguration: IContainerConfiguration, new()
        {
            new TConfiguration().Register(container);
        }

        public static void RegisterAggregate<TAggregate, TCommandsHandler>(this IUnityContainer container) where TCommandsHandler : ICommandAggregateLocator<TAggregate>, IAggregateCommandsHandler<TAggregate> where TAggregate : AggregateBase
        {
            Register<AggregateConfiguration<TAggregate, TCommandsHandler>>(container);
        }

    }
}