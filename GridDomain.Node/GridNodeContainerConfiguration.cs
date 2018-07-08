using System;
using System.Threading.Tasks;
using Akka.DI.Core;
using Autofac;
using GridDomain.Common;
using GridDomain.Configuration;
using GridDomain.Configuration.MessageRouting;
using GridDomain.CQRS;

using GridDomain.Node.Actors;
using GridDomain.Node.AkkaMessaging;
using GridDomain.Node.Configuration;
using GridDomain.Node.Configuration.Composition;
using GridDomain.Transport;
using Serilog;

namespace GridDomain.Node
{
    public class GridNodeContainerConfiguration : IContainerConfiguration
    {
        private INodeContext Context { get; }

        public GridNodeContainerConfiguration(INodeContext context)
        {
            Context = context;
        }

        public void Register(ContainerBuilder container)
        {
            container.RegisterInstance(Context.Log);
            container.RegisterInstance<IPublisher>(Context.Transport);
            container.RegisterInstance<IActorSubscriber>(Context.Transport);
            container.RegisterInstance(Context.Transport);
            container.RegisterInstance<IMessageProcessContext>(Context);
            container.RegisterInstance<INodeContext>(Context);
        }
    }
}