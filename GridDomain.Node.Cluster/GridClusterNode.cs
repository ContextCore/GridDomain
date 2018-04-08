using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GridDomain.Configuration;
using GridDomain.CQRS;
using GridDomain.Node.Configuration.Composition;
using GridDomain.Transport;
using Serilog;

namespace GridDomain.Node.Cluster {
    public class GridClusterNode : GridDomainNode
    {
        private ClusterCommandExecutor _clusterCommandExecutor;
        public GridClusterNode(IEnumerable<IDomainConfiguration> domainConfigurations, IActorSystemFactory actorSystemFactory, ILogger log, TimeSpan defaultTimeout) : base(domainConfigurations, actorSystemFactory, log, defaultTimeout) { }
        protected override ICommandExecutor CreateCommandExecutor()
        {
            _clusterCommandExecutor = new ClusterCommandExecutor(System,Transport,DefaultTimeout);
            return _clusterCommandExecutor;
        }

        protected override IActorCommandPipe CreateCommandPipe()
        {
            return new ClusterCommandPipe(System,Log);
        }

        protected override async Task ConfigurePipe(DomainBuilder domainBuilder)
        {
            await base.ConfigurePipe(domainBuilder);
            _clusterCommandExecutor.Init(Pipe.CommandExecutor);
        }

        protected override IActorTransport CreateTransport()
        {
            var ext =  System.InitDistributedTransport();
            return ext.Transport;
        }
        protected override IMessageWaiterFactory CreateMessageWaiterFactory()
        {
            return new ClusterMessageWaiterFactory(System, Transport, DefaultTimeout);
        }
    }
}