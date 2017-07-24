using System;
using System.Threading.Tasks;
using Akka.DI.Core;
using Akka.DI.Unity;
using GridDomain.Common;
using GridDomain.Configuration;
using GridDomain.Configuration.MessageRouting;
using GridDomain.CQRS;

using GridDomain.Node.Actors;
using GridDomain.Node.AkkaMessaging;
using GridDomain.Node.Configuration.Composition;
using GridDomain.Node.Transports;
using Microsoft.Practices.Unity;
using Serilog;

namespace GridDomain.Node
{
    public class GridNodeContainerConfiguration : IContainerConfiguration
    {
        private readonly ILogger _log;
        private readonly IActorTransport _actorTransport;

        public GridNodeContainerConfiguration(IActorTransport transportMode, ILogger settingsLog)
        {
            _log = settingsLog;
            _actorTransport = transportMode;
        }

        public void Register(IUnityContainer container)
        {
            container.RegisterInstance(_log);
            container.RegisterInstance<IPublisher>(_actorTransport);
            container.RegisterInstance<IActorSubscriber>(_actorTransport);
            container.RegisterInstance(_actorTransport);
            container.RegisterInstance<IMessageProcessContext>(new MessageProcessContext(_actorTransport, _log));
        }
    }
}