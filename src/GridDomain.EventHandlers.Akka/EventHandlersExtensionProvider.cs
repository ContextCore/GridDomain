using Akka.Actor;
using Autofac;

namespace GridDomain.EventHandlers.Akka {
    public class EventHandlersExtensionProvider : ExtensionIdProvider<EventHandlersExtension>
    {
        private readonly ContainerBuilder _container;

        public EventHandlersExtensionProvider(ContainerBuilder container=null)
        {
            _container = container;
        }

        public override EventHandlersExtension CreateExtension(ExtendedActorSystem system)
        {
            return new EventHandlersExtension(system, _container);
        }
    }
}