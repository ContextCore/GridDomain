using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Cluster;
using Autofac;
using GridDomain.Aggregates;
using GridDomain.Node.Akka;
using GridDomain.Node.Akka.Cluster;
using Serilog;
using IContainer = System.ComponentModel.IContainer;

namespace GridDomain.Node {



      public interface INode : IDisposable
      {
          Task<IDomain> Start();
      }
    
    
    public class GridDomainNode : INode
    {
        private bool _stopping;
        
        public ActorSystem System;
        private IContainer Container { get; set; }
        private readonly IActorSystemFactory _actorSystemFactory;
        public ILogger Log { get; set; }
        private readonly List<IDomainConfiguration> _domainConfigurations;
        public TimeSpan DefaultTimeout { get; }
        public string Name;


        public GridDomainNode(IEnumerable<IDomainConfiguration> domainConfigurations, 
                                 IActorSystemFactory actorSystemFactory,
                                 ILogger log, 
                                 TimeSpan defaultTimeout)
        {
            _domainConfigurations = domainConfigurations.ToList();
            if(!_domainConfigurations.Any())
                throw new NoDomainConfigurationException();
            if (_domainConfigurations.Any(d => d == null))
                throw new InvalidDomainConfigurationException();
            
            DefaultTimeout = defaultTimeout;
            Log = log;
            _actorSystemFactory = actorSystemFactory;
        }

   
        public void Dispose()
        {
            Stop().Wait();
        }

        public async Task<IDomain> Start()
        {
            System = _actorSystemFactory.CreateSystem();
            Name = System.Name;

            Log.Information("Starting GridDomain node {Id}", Name);

            var cluster = Cluster.Get(System);
            
            var containerBuilder = new ContainerBuilder();
            var domainBuilder = new ClusterDomainBuilder(System, containerBuilder);
            foreach (var configuration in _domainConfigurations)
            {
               await configuration.Register(domainBuilder);
            }

            var domain = await domainBuilder.Build();
            return domain;
        }

        private async Task Stop()
        {
            if(_stopping)
                return;

            Log.Information("GridDomain node {Id} is stopping", Name);
            _stopping = true;

            if(System != null)
            {
                await System.Terminate();
                System.Dispose();
            }
            System = null;
            Container?.Dispose();
            Log.Information("GridDomain node {Id} stopped", Name);
        }

        internal class NoDomainConfigurationException : Exception { }
        public class InvalidDomainConfigurationException : Exception { }

    }
}