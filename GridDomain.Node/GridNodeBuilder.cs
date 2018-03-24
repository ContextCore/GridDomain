using System;
using GridDomain.Configuration;
using GridDomain.Node.Configuration;
using Serilog;

namespace GridDomain.Node {
    public class GridNodeBuilder
    {
        protected IActorCommandPipeFactory _actorCommandPipeFactory;
        protected ILogger _logger;
        protected IDomainConfiguration[] _domainConfigurations;
        protected TimeSpan _timeout;
        
        public GridNodeBuilder()
        {
            _logger = new DefaultLoggerConfiguration().CreateLogger()
                                                      .ForContext<GridDomainNode>();
            _timeout = TimeSpan.FromSeconds(10);
            _actorCommandPipeFactory = new LocalCommadPipeFactory(new HoconActorSystemFactory("system",""));
            _domainConfigurations = new IDomainConfiguration[] { };
        }
        
        public IGridDomainNode Build()
        {
            return new GridDomainNode(_domainConfigurations,_actorCommandPipeFactory,_logger,_timeout);
        }

        public GridNodeBuilder PipeFactory(IActorCommandPipeFactory factory)
        {
            _actorCommandPipeFactory = factory;
            return this;
        }

        public GridNodeBuilder Log(ILogger log)
        {
            _logger = log;
            return this;
        }
        
        public GridNodeBuilder DomainConfigurations(params IDomainConfiguration[] domainConfigurations)
        {
            _domainConfigurations = domainConfigurations;
            return this;
        } 

        public GridNodeBuilder Timeout(TimeSpan timeout)
        {
            this._timeout = timeout;
            return this;
        }

    }
}