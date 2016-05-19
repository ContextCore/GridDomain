using Microsoft.Practices.Unity;

namespace GridDomain.Node
{
    public interface IContainerConfiguration
    {
        void Register(IUnityContainer container);
    }
}