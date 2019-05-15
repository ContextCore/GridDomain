using System;
using System.Linq;
using System.Threading.Tasks;
using Akka.Actor;
using Autofac;
using GridDomain.Domains;
using GridDomain.Node.Akka.Cluster;

namespace GridDomain.Node.Akka
{
    public class GridDomainNode : INode, IExtension
    {
        public ActorSystem System;
        private IContainer Container { get; set; }
        private readonly IDomainConfiguration[] _domainConfigurations;
        public TimeSpan DefaultTimeout { get; }
        public string Name;

        public GridDomainNode(ActorSystem system, params IDomainConfiguration[] domains) : this(system,
            TimeSpan.FromSeconds(5), domains)
        {
        }

        public GridDomainNode(ActorSystem actorSystem,
                              TimeSpan defaultTimeout,
                              params IDomainConfiguration[] domains)
        {
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

            var containerBuilder = new ContainerBuilder();
            var domainBuilder = new ClusterDomainBuilder(System, containerBuilder);
            foreach (var configuration in _domainConfigurations)
            {
                await configuration.Register(domainBuilder);
            }

            var domain = await domainBuilder.Build();
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