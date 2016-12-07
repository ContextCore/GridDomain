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
        /// <summary>
        /// Registers a dependency resolver with a given actor system.
        /// </summary>
        /// <param name="system">The actor system in which to register the given dependency resolver.</param>
        /// <param name="dependencyResolver">The dependency resolver being registered to the actor system.</param>
        /// <exception cref="ArgumentNullException">
        /// Either the <paramref name="system"/> or the <paramref name="dependencyResolver"/> was null.
        /// </exception>
        public static DomainEventsJsonSerializationExtension AddDomainEventsJsonSerialization(this ActorSystem system)
        {
            if (system == null) throw new ArgumentNullException(nameof(system));
            return (DomainEventsJsonSerializationExtension)system.RegisterExtension(DomainEventsJsonSerializationExtensionProvider.Provider);
            
        }
    }
    public class DomainEventsJsonSerializationExtension : IExtension
    {
        public JsonSerializerSettings Settings { get; set; }
        public IReadOnlyCollection<JsonConverter> Ñonverters { get; set; }
    }
}