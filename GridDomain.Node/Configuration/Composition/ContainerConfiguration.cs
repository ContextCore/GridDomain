using System;
using System.Linq;
using Autofac;
using GridDomain.Common;
using GridDomain.Configuration;
namespace GridDomain.Node.Configuration.Composition
{
    public class ContainerConfiguration : IContainerConfiguration
    {
        private readonly Action<ContainerBuilder>[] _registrations;

        public ContainerConfiguration(params Action<ContainerBuilder>[] registrations)
        {
            _registrations = registrations;
        }

        public ContainerConfiguration(params IContainerConfiguration[] configurations)
            : this(
                   configurations.Select(config => (Action<ContainerBuilder>) (container => config.Register(container)))
                                 .ToArray()) {}

        public void Register(ContainerBuilder container)
        {
            foreach (var reg in _registrations)
                reg.Invoke(container);
        }

        public static ContainerConfiguration Empty()
        {
            return new ContainerConfiguration(c => { });
        }
    }
}