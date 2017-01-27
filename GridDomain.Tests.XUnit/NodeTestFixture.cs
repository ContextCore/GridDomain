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
    public class NodeTestFixture : IDisposable
    {
        public static readonly AkkaConfiguration DefaultAkkaConfig = new AutoTestAkkaConfiguration();

        private GridDomainNode _node;
        public GridDomainNode Node => _node ?? CreateNode();

        public ActorSystem System { get; set; }

        private readonly IContainerConfiguration _containerConfiguration;
        private readonly IMessageRouteMap _routeMap;

        protected virtual IContainerConfiguration CreateContainerConfiguration()
        {
            return _containerConfiguration;
        }

        protected virtual IMessageRouteMap CreateRouteMap()
        {
            return _routeMap;
        }

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

        public NodeTestFixture(IContainerConfiguration containerConfiguration = null, IMessageRouteMap map = null)
        {
            _routeMap = map;
            _containerConfiguration = containerConfiguration;
        }

        private GridDomainNode CreateNode()
        {
            if (ClearDataOnStart)
                TestDbTools.ClearData(DefaultAkkaConfig.Persistence);

            var quartzConfig = InMemory ? (IQuartzConfig)new InMemoryQuartzConfig() : new PersistedQuartzConfig();

            var node = new GridDomainNode(_containerConfiguration ?? CreateContainerConfiguration(),
                                          _routeMap ?? CreateRouteMap(), 
                                          () => new[] { System ?? ActorSystem.Create(Name, GetConfig()) },
                                          quartzConfig);
            _node = node;
            OnNodeCreated();
            node.Start().Wait();
            return node;
        }

        protected virtual void OnNodeCreated()
        {
            
        }
        public void Dispose()
        {
            Node.Stop().Wait(DefaultTimeout);
        }
    }
}