using System;
using System.Linq;
using GridDomain.Common;
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

        public CustomContainerConfiguration(params IContainerConfiguration[] configurations)
            : this(configurations.Select(config => (Action<IUnityContainer>) (container => container.Register(config)))
                                 .ToArray()) {}

        public void Register(IUnityContainer container)
        {
            foreach (var reg in _registrations)
                reg.Invoke(container);
        }

        public static CustomContainerConfiguration Empty()
        {
            return new CustomContainerConfiguration(c => { });
        }
    }
}