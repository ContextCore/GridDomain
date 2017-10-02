using Autofac;
using GridDomain.Common;

namespace GridDomain.Node.Configuration.Composition
{
    internal class EmptyContainerConfiguration : IContainerConfiguration
    {
        public void Register(ContainerBuilder container) {}
    }
}