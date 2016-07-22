using GridDomain.Node.Configuration.Composition;
using Microsoft.Practices.Unity;

namespace GridDomain.Node
{
    internal class EmptyContainerConfig : IContainerConfiguration
    {
        public void Register(IUnityContainer container)
        {
        }
    }
}