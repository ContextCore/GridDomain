using System;
using Akka.Actor;
using Autofac;

namespace GridDomain.EventHandlers.Akka {
    public static class EventHandlersExtensions
    {

        public static EventHandlersExtension GetEventHandlersExtension(this ActorSystem sys)
        {
            return sys.GetExtension<EventHandlersExtension>();
        }

            
        public static EventHandlersExtension InitEventHandlersExtension(this ActorSystem system,
                                                                  IContainer container)
        {
            if(system == null)
                throw new ArgumentNullException(nameof(system));

            return (EventHandlersExtension)system.RegisterExtension(new EventHandlersExtensionProvider(container));
        }
    }
}