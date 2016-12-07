using System.Collections.Generic;
using Akka.Actor;
using GridDomain.EventSourcing.Adapters;
using Newtonsoft.Json;
using NEventStore.Conversion;

namespace GridDomain.Node
{
    public class DomainEventsJsonSerializationExtensionProvider : ExtensionIdProvider<DomainEventsJsonSerializationExtension>
    {
        /// <summary>
        /// A static reference to the current provider.
        /// </summary>
        public static readonly DomainEventsJsonSerializationExtensionProvider Provider = new DomainEventsJsonSerializationExtensionProvider();

       
       
                                                                                                         
        public override DomainEventsJsonSerializationExtension CreateExtension(ExtendedActorSystem system)
        {
            return new DomainEventsJsonSerializationExtension();
        }
    }
}