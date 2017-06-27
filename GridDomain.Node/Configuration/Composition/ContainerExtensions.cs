
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
    }
}