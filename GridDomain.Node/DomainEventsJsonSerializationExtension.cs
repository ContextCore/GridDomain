using System;
using System.Collections.Generic;
using Akka.Actor;
using Akka.DI.Core;
using GridDomain.EventSourcing.Adapters;
using Newtonsoft.Json;

namespace GridDomain.Node
{

    public static class DomainEventsJsonSerializationExtensions
    {
        public static DomainEventsJsonSerializationExtension InitDomainEventsSerialization(
            this ActorSystem system,
            EventsAdaptersCatalog eventAdapters
            )
        {
            if (system == null)
                throw new ArgumentNullException(nameof(system));

            var ext = (DomainEventsJsonSerializationExtension)system.RegisterExtension(DomainEventsJsonSerializationExtensionProvider.Provider);
            ext.Converters = eventAdapters.JsonConverters;
            ext.EventsAdapterCatalog = eventAdapters;
            return ext;
            
        }
    }
    public class DomainEventsJsonSerializationExtension : IExtension
    {
        public JsonSerializerSettings Settings { get; set; }
        public IReadOnlyCollection<JsonConverter> Converters { get; set; } 
        public IObjectUpdateChain EventsAdapterCatalog { get; set; }

        //to avoid errors when default values are used unintentionally - it is hard to debug
        public void InitEmpty()
        {
            Settings = null;
            Converters = new JsonConverter[] {};
            EventsAdapterCatalog = new EventsAdaptersCatalog();
        }
    }
}