using GridDomain.CQRS.Messaging;
using Microsoft.Practices.Unity;

namespace GridDomain.Node
{
    class UnityServiceLocator : IServiceLocator
    {
        private readonly IUnityContainer _container;

        public UnityServiceLocator(IUnityContainer container)
        {
            _container = container;
        }

        public T Resolve<T>()
        {
            return _container.Resolve<T>();
        }
    }
}