using GridDomain.Common;
using GridDomain.Node.Configuration;
using Microsoft.Practices.Unity;

namespace GridDomain.Node
{
    public static class ContainerExtensions
    {
        public static void Register(this IUnityContainer container, IContainerConfiguration configuration)
        {
            configuration.Register(container);
        }
    }
}