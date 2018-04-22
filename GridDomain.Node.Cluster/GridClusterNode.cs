using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Akka.Actor;
using GridDomain.Configuration;
using GridDomain.CQRS;
using GridDomain.Node.Cluster.CommandPipe;
using GridDomain.Node.Cluster.MessageWaiting;
using GridDomain.Node.Cluster.Transport;
using GridDomain.Node.Configuration.Composition;
using GridDomain.Transport;
using GridDomain.Transport.Extension;
using GridDomain.Transport.Remote;
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

        protected override IActorTransport CreateTransport()
        {
            return System.InitDistributedTransport().Transport;
        }

        protected override async Task StartMessageRouting()
        {
            await base.StartMessageRouting();
            _clusterCommandExecutor.Init(Pipe.CommandExecutor);
        }

        protected override IMessageWaiterFactory CreateMessageWaiterFactory()
        {
            return new ClusterMessageWaiterFactory(System, Transport, DefaultTimeout);
        }

        protected override DomainBuilder CreateDomainBuilder()
        {
            return new ClusterDomainBuilder();
        }
    }
}
