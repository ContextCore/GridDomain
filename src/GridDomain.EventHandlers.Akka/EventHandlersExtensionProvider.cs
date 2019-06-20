using Akka.Actor;
using Autofac;

namespace GridDomain.EventHandlers.Akka {
    public class EventHandlersExtensionProvider : ExtensionIdProvider<EventHandlersExtension>
    {
        private readonly IContainer _container;

        public EventHandlersExtensionProvider(IContainer container)
        {
            _container = container;
        }

        public override EventHandlersExtension CreateExtension(ExtendedActorSystem system)
        {
            return new EventHandlersExtension(_container);
        }
    }
}