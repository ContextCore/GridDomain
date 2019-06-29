using System;
using Akka.Actor;
using Autofac;

namespace GridDomain.EventHandlers.Akka {
    public static class EventHandlersExtensions
    {

        public static EventHandlersDomainExtension GetEventHandlersExtension(this ActorSystem sys)
        {
            return sys.GetExtension<EventHandlersDomainExtension>();
        }

            
        public static EventHandlersDomainExtension InitEventHandlersExtension(this ActorSystem system,
                                                                               ContainerBuilder container=null)
        {
            if(system == null)
                throw new ArgumentNullException(nameof(system));

            return (EventHandlersDomainExtension)system.RegisterExtension(new EventHandlersExtensionProvider(container));
        }
    }
}