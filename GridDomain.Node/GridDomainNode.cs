using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Akka.Actor;
using GridDomain.Aggregates;
using Serilog;

namespace GridDomain.Node {



      public interface INode : ICommandHandler<ICommand>, IDisposable
      {
          Task Start();
      }
    
    
    public class GridDomainNode : INode
    {
        private bool _stopping;

        protected GridDomainNode(IEnumerable<IDomainConfiguration> domainConfigurations, 
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

        public ActorSystem System;

        private IContainer Container { get; set; }
        private readonly IActorSystemFactory _actorSystemFactory;
        public ILogger Log { get; set; }
        
        private readonly List<IDomainConfiguration> _domainConfigurations;
        public TimeSpan DefaultTimeout { get; }
        
        public string Name;

   
        public void Dispose()
        {
            Stop().Wait();
        }

        public async Task Start()
        {
            Log.Information("Starting GridDomain node {Id}", Name);
            await Task.CompletedTask;
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

        public Task Execute(ICommand command)
        {
            throw new NotImplementedException();
        }
    }
}