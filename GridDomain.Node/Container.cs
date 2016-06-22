using System;
using Microsoft.Practices.Unity;

namespace GridDomain.Node
{
    public class Container
    {
        public static UnityContainer Current => Factory.Value;
        private static readonly Lazy<UnityContainer> Factory = new Lazy<UnityContainer>(() => new UnityContainer());

        public static IUnityContainer CreateChildScope()
        {
            var child = Current.CreateChildContainer();
            child.RegisterType<IUnityContainer>(new InjectionFactory(x => child));
            return child;
        }
    }
}