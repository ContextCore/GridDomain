using System;
using Akka.Actor;
using GridDomain.EventSourcing.Adapters;

namespace GridDomain.Node.Serializers
{
    public static class DomainEventsJsonSerializationExtensions
    {
        public static DomainEventsJsonSerializationExtension InitDomainEventsSerialization(this ActorSystem system,
                                                                                           EventsAdaptersCatalog
                                                                                               eventAdapters)
        {
            if (system == null)
                throw new ArgumentNullException(nameof(system));

            var ext =
                (DomainEventsJsonSerializationExtension)
                system.RegisterExtension(DomainEventsJsonSerializationExtensionProvider.Provider);
            ext.Converters = eventAdapters.JsonConverters;
            ext.EventsAdapterCatalog = eventAdapters;
            return ext;
        }
    }
}