using System;
using System.Threading.Tasks;
using Akka.Actor;
using GridDomain.Common;
using GridDomain.CQRS.Messaging;
using GridDomain.Node;
using GridDomain.Node.Configuration.Akka;
using GridDomain.Node.Configuration.Composition;
using GridDomain.Scheduling.Quartz;
using GridDomain.Tests.Framework;
using GridDomain.Tests.Framework.Configuration;

namespace GridDomain.Tests.XUnit
{
    public abstract class NodeTestFixture : IDisposable
    {
        public static readonly AkkaConfiguration DefaultAkkaConfig = new AutoTestAkkaConfiguration();

        private GridDomainNode _node;
        public GridDomainNode GridNode => _node ?? (_node = CreateNode());

        public ActorSystem ExternalSystem { get; set; }
        protected abstract IContainerConfiguration ContainerConfiguration { get; }
        protected abstract IMessageRouteMap RouteMap { get; }

        private AkkaConfiguration AkkaConfig { get; } = DefaultAkkaConfig;
        private bool ClearDataOnStart => !InMemory;
        private bool InMemory { get; } = true;
        public string Name => AkkaConfig.Network.SystemName;
        
        public TimeSpan DefaultTimeout { get; } = TimeSpan.FromSeconds(3);

        public string GetConfig()
        {
            return InMemory ? AkkaConfig.ToStandAloneInMemorySystemConfig() : 
                AkkaConfig.ToStandAloneSystemConfig();
        }


        private GridDomainNode CreateNode()
        {
            if (ClearDataOnStart)
                TestDbTools.ClearData(DefaultAkkaConfig.Persistence);

            var quartzConfig = InMemory ? (IQuartzConfig)new InMemoryQuartzConfig() : new PersistedQuartzConfig();

            var node = new GridDomainNode(ContainerConfiguration, 
                                          RouteMap, 
                                          () => new[] { ExternalSystem ?? ActorSystem.Create(Name, GetConfig()) },
                                          quartzConfig);

            node.Start().Wait();
            return node;
        }

        public void Dispose()
        {
            GridNode.Stop().Wait(DefaultTimeout);
        }
    }
}