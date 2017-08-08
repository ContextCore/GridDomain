using System;
using System.Linq;

namespace GridDomain.Configuration {
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