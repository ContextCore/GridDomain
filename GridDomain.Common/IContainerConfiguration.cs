using Microsoft.Practices.Unity;

namespace GridDomain.Common
{
    public interface IContainerConfiguration
    {
        void Register(IUnityContainer container);
    }
}