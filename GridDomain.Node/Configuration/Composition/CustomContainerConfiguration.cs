using System;
using Microsoft.Practices.Unity;

namespace GridDomain.Node.Configuration.Composition
{
    public class CustomContainerConfiguration : IContainerConfiguration
    {
        private readonly Action<IUnityContainer>[] _registrations;

        public CustomContainerConfiguration(params Action<IUnityContainer>[] registrations)
        {
            _registrations = registrations;
        }

        public void Register(IUnityContainer container)
        {
            foreach(var reg in _registrations)
                reg.Invoke(container);
        }
    }
}