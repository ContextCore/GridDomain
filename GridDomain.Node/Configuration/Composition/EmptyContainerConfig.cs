using GridDomain.Common;
using Microsoft.Practices.Unity;

namespace GridDomain.Node.Configuration.Composition
{
    internal class EmptyContainerConfig : IContainerConfiguration
    {
        public void Register(IUnityContainer container)
        {
        }
    }
}