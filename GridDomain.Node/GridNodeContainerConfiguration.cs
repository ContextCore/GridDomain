using System;
using Akka.Actor;
using GridDomain.Common;
using GridDomain.Node.Configuration.Composition;
using GridDomain.Node.Configuration.Persistence;
using GridDomain.Scheduling.Quartz;
using Microsoft.Practices.Unity;

namespace GridDomain.Node
{
    public class GridNodeContainerConfiguration : IContainerConfiguration
    {
        private readonly ActorSystem _actorSystem;
        private readonly IDbConfiguration _conf;
        private readonly TransportMode _transportMode;
        private readonly IQuartzConfig _config;

        public GridNodeContainerConfiguration(ActorSystem actorSystem,
            IDbConfiguration conf,
            TransportMode transportMode,
            IQuartzConfig config)
        {
            _config = config;
            _transportMode = transportMode;
            _actorSystem = actorSystem;
        }

        public void Register(IUnityContainer container)
        {
            CompositionRoot.Init(container, _actorSystem, _transportMode, _config);
        }
    }
}