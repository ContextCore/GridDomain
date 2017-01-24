using GridDomain.Common;
using Microsoft.Practices.Unity;

namespace GridDomain.Node.Configuration.Composition
{
    internal class EmptyContainerConfiguration : IContainerConfiguration
    {
        public void Register(IUnityContainer container)
        {
        }
    }
}