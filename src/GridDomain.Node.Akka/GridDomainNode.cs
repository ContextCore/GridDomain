using System;
using System.Linq;
using System.Threading.Tasks;
using Akka.Actor;
using Autofac;
using GridDomain.Abstractions;
using GridDomain.Domains;
using GridDomain.Node.Akka.Cluster;

namespace GridDomain.Node.Akka
{
    public class GridDomainNode : INode, IExtension
    {
        private ActorSystem System;
        private IContainer Container { get; set; }
        private readonly IDomainConfiguration[] _domainConfigurations;
        public TimeSpan DefaultTimeout { get; }
        public string Name;
        private readonly IDomainBuilder _domainBuilder;

        public GridDomainNode(ActorSystem system, IDomainBuilder domainBuilder, params IDomainConfiguration[] aggregatesDomains) : this(system, domainBuilder,
            TimeSpan.FromSeconds(5), aggregatesDomains)
        {
        }

        public GridDomainNode(ActorSystem actorSystem,
                                IDomainBuilder domainBuilder,
                              TimeSpan defaultTimeout,
                              params IDomainConfiguration[] domains)
        {
            _domainBuilder = domainBuilder;
            _domainConfigurations = domains;
            if (!_domainConfigurations.Any())
                throw new NoDomainConfigurationException();
            if (_domainConfigurations.Any(d => d == null))
                throw new InvalidDomainConfigurationException();

            DefaultTimeout = defaultTimeout;
            System = actorSystem;
        }


        public void Dispose()
        {
            Container?.Dispose();
        }

        public async Task<IDomain> Start()
        {
            Address = System.GetAddress().ToString();
            Name = System.Name;

            System.Log.Info("Starting GridDomain node {Id}", Name);

            
            foreach (var configuration in _domainConfigurations)
            {
                await configuration.Configure(_domainBuilder);
            }

            var domain = await _domainBuilder.Build();
            return domain;
        }

        public string Address { get; private set; }


        internal class NoDomainConfigurationException : Exception
        {
        }

        public class InvalidDomainConfigurationException : Exception
        {
        }
    }
}