using Microsoft.Practices.Unity;

namespace GridDomain.Node.Configuration.Composition
{
    public interface IContainerConfiguration
    {
        void Register(IUnityContainer container);
    }
}