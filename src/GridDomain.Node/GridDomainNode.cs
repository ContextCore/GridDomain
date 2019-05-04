using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Cluster;
using Autofac;
using DotNetty.Common.Concurrency;
using GridDomain.Aggregates;
using GridDomain.Domains;
using GridDomain.Node.Akka;
using GridDomain.Node.Akka.Cluster;
using GridDomain.Node.Akka.Configuration;
using GridDomain.Node.Tests;
using Microsoft.Extensions.Logging;

namespace GridDomain.Node
{

    public class GridDomainNode : INode, IExtension
    {
        private bool _stopping;

        public ActorSystem System;
        private IContainer Container { get; set; }
        private readonly IActorSystemFactory _actorSystemFactory;
        private readonly List<IDomainConfiguration> _domainConfigurations;
        public TimeSpan DefaultTimeout { get; }
        public string Name;

        public static GridDomainNode New(ActorSystem system, params IDomainConfiguration[] domains)
        {
            return new GridDomainNode(domains, new DelegateActorSystemFactory(() => system), 
                TimeSpan.FromSeconds(5));
        }

        public GridDomainNode(IEnumerable<IDomainConfiguration> domainConfigurations,
            IActorSystemFactory actorSystemFactory,
            TimeSpan defaultTimeout)
        {
            _domainConfigurations = domainConfigurations.ToList();
            if (!_domainConfigurations.Any())
                throw new NoDomainConfigurationException();
            if (_domainConfigurations.Any(d => d == null))
                throw new InvalidDomainConfigurationException();

            DefaultTimeout = defaultTimeout;
            _actorSystemFactory = actorSystemFactory;
        }


        public void Dispose()
        {
            Stop().Wait();
        }

        public async Task<IDomain> Start()
        {
            System = _actorSystemFactory.CreateSystem();
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

        private async Task Stop()
        {
            if (_stopping)
                return;

            System.Log.Info("GridDomain node {Id} is stopping", Name);
            _stopping = true;

            if (System != null)
            {
                await System.Terminate();
                System.Dispose();
            }
            
            System.Log.Info("GridDomain node {Id} stopped", Name);
            System = null;
            Container?.Dispose();
        }

        internal class NoDomainConfigurationException : Exception
        {
        }

        public class InvalidDomainConfigurationException : Exception
        {
        }
    }
}