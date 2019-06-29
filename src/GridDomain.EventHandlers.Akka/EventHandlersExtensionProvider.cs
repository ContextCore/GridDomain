using Akka.Actor;
using Autofac;

namespace GridDomain.EventHandlers.Akka {
    public class EventHandlersExtensionProvider : ExtensionIdProvider<EventHandlersDomainExtension>
    {
        private readonly ContainerBuilder _container;

        public EventHandlersExtensionProvider(ContainerBuilder container=null)
        {
            _container = container;
        }

        public override EventHandlersDomainExtension CreateExtension(ExtendedActorSystem system)
        {
            return new EventHandlersDomainExtension(system, _container);
        }
    }
}