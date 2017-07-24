using Microsoft.Practices.Unity;

namespace GridDomain.Node.Configuration
{
    public interface IContainerConfiguration
    {
        void Register(IUnityContainer container);
    }
}