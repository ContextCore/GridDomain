using System.Collections.Generic;
using Akka.Actor;
using GridDomain.EventSourcing.Adapters;
using Newtonsoft.Json;

namespace GridDomain.Node
{
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