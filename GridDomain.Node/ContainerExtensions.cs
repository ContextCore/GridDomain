using Autofac;
using GridDomain.Common;
using GridDomain.Node.Configuration;

namespace GridDomain.Node
{
    public static class ContainerExtensions
    {
        public static void Register(this ContainerBuilder container, IContainerConfiguration configuration)
        {
            configuration.Register(container);
        }
    }
}