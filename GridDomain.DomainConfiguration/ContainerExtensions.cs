using GridDomain.Common;
using Microsoft.Practices.Unity;

namespace GridDomain.Configuration
{
    public static class ContainerExtensions
    {
        public static void Register(this IUnityContainer container, IContainerConfiguration configuration)
        {
            configuration.Register(container);
        }
    }
}