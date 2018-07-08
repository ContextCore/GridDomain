using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GridDomain.Configuration;
using GridDomain.CQRS;
using GridDomain.Node.Actors.ProcessManagers;
using GridDomain.Node.Configuration.Composition;
using GridDomain.Transport;
using GridDomain.Transport.Extension;
using Serilog;

namespace GridDomain.Node {
    public class GridDomainLocalNode : GridDomainNode
    {
        private AkkaCommandExecutor _akkaCommandExecutor;
        public GridDomainLocalNode(IEnumerable<IDomainConfiguration> domainConfigurations, IActorSystemFactory actorSystemFactory, ILogger log, TimeSpan defaultTimeout) : base(domainConfigurations, actorSystemFactory, log, defaultTimeout) { }
        
        protected override ICommandExecutor CreateCommandExecutor()
        {
            _akkaCommandExecutor = new AkkaCommandExecutor(System,Transport,DefaultTimeout);
            return _akkaCommandExecutor;
        }

        protected override IActorCommandPipe CreateCommandPipe()
        {
            return new LocalCommandPipe(System);
        }

        protected override IActorTransport CreateTransport()
        {
            return System.InitLocalTransportExtension().Transport;
        }

        protected override async Task StartMessageRouting()
        {
            await base.StartMessageRouting();
            _akkaCommandExecutor.Init(Pipe.CommandExecutor);
        }

        protected override DomainBuilder CreateDomainBuilder()
        {
            return new DomainBuilder(ProcessHubActor.GetProcessStateActorSelection);
        }
    }
}