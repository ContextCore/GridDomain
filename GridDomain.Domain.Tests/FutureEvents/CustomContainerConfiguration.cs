using System;
using GridDomain.Node.Configuration.Composition;
using Microsoft.Practices.Unity;

namespace GridDomain.Tests.FutureEvents
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