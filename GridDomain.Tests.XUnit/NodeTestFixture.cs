using System;
using System.Collections.Generic;
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

        private readonly List<IContainerConfiguration> _containerConfiguration = new List<IContainerConfiguration>();
        private readonly List<IMessageRouteMap> _routeMap = new List<IMessageRouteMap>();

        protected void Add(IMessageRouteMap map)
        {
            _routeMap.Add(map);
        }

        protected void Add(IContainerConfiguration config)
        {
            _containerConfiguration.Add(config);
        }

        protected virtual IContainerConfiguration CreateContainerConfiguration()
        {
            return new CustomContainerConfiguration(_containerConfiguration.ToArray());
        }

        protected virtual IMessageRouteMap CreateRouteMap()
        {
            return new CompositeRouteMap(_routeMap.ToArray());
        }

        private AkkaConfiguration AkkaConfig { get; } = DefaultAkkaConfig;
        private bool ClearDataOnStart => !InMemory;
        protected bool InMemory { get; } = true;
        public string Name => AkkaConfig.Network.SystemName;
        
        public TimeSpan DefaultTimeout { get; }

        public string GetConfig()
        {
            return InMemory ? AkkaConfig.ToStandAloneInMemorySystemConfig() : 
                              AkkaConfig.ToStandAloneSystemConfig();
        }

        public NodeTestFixture(IContainerConfiguration containerConfiguration = null, IMessageRouteMap map = null, TimeSpan? defaultTimeout = null)
        {
            if(map!= null)
                Add(map);
            if(containerConfiguration != null)
                Add(containerConfiguration);

            DefaultTimeout = defaultTimeout ?? TimeSpan.FromSeconds(3);
        }

        private GridDomainNode CreateNode()
        {
            if (ClearDataOnStart)
                TestDbTools.ClearData(DefaultAkkaConfig.Persistence);

            var quartzConfig = InMemory ? (IQuartzConfig)new InMemoryQuartzConfig() : new PersistedQuartzConfig();

            var node = new GridDomainNode(CreateContainerConfiguration(),
                                          CreateRouteMap(), 
                                          () => new[] { System ?? ActorSystem.Create(Name, GetConfig()) },
                                          quartzConfig,
                                          DefaultTimeout);
            _node = node;
            OnNodeCreated();
            node.Start().Wait();
            OnNodeStarted();
            return node;
        }

        protected virtual void OnNodeCreated()
        {
            
        }
        protected virtual void OnNodeStarted()
        {

        }
        public void Dispose()
        {
            Node.Stop().Wait(DefaultTimeout);
        }
    }
}