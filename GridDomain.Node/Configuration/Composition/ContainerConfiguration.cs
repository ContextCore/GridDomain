using System;
using System.Linq;
using GridDomain.Common;
using Microsoft.Practices.Unity;

namespace GridDomain.Node.Configuration.Composition
{
    public class ContainerConfiguration : IContainerConfiguration
    {
        private readonly Action<IUnityContainer>[] _registrations;

        public ContainerConfiguration(params Action<IUnityContainer>[] registrations)
        {
            _registrations = registrations;
        }

        public ContainerConfiguration(params IContainerConfiguration[] configurations)
            : this(
                   configurations.Select(config => (Action<IUnityContainer>) (container => container.Register(config)))
                                 .ToArray()) {}

        public void Register(IUnityContainer container)
        {
            foreach (var reg in _registrations)
                reg.Invoke(container);
        }

        public static ContainerConfiguration Empty()
        {
            return new ContainerConfiguration(c => { });
        }
    }

    public class DomainConfiguration : IDomainConfiguration
    {
        private readonly Action<IDomainBuilder>[] _registrations;

        public DomainConfiguration(params Action<IDomainBuilder>[] registrations)
        {
            _registrations = registrations;
        }

        public DomainConfiguration(params IDomainConfiguration[] configurations)
            : this(configurations.Select(config => (Action<IDomainBuilder>)(container => container.Register(config)))
                                 .ToArray())
        { }

        public void Register(IDomainBuilder container)
        {
            foreach (var reg in _registrations)
                reg.Invoke(container);
        }

        public static IDomainConfiguration Empty()
        {
            return new DomainConfiguration(c => { });
        }
    }
}