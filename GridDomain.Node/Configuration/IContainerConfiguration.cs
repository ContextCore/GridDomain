using Autofac;

namespace GridDomain.Node.Configuration
{
    public interface IContainerConfiguration
    {
        void Register(ContainerBuilder container);
    }
}