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
        public static void AddDomainEventsJsonSerialization(this ActorSystem system)
        {
            if (system == null) throw new ArgumentNullException(nameof(system));
            system.RegisterExtension(DomainEventsJsonSerializationExtensionProvider.Provider);
        }
    }


    public class DomainEventsJsonSerializationExtension : IExtension, IObjectsAdapter
    {
        private readonly List<JsonConverter> _converters = new List<JsonConverter>();

        public IReadOnlyCollection<JsonConverter> Converters => _converters;
        public JsonSerializerSettings Settings { get; set; }

        void IObjectsAdapter.Register(JsonConverter converter)
        {
            _converters.Add(converter);
        }

        public void Clear()
        {
            _converters.Clear();
        }

        public void Register<TFrom, TTo>(ObjectAdapter<TFrom, TTo> converter)
        {
            ((IObjectsAdapter)this).Register((JsonConverter)converter);
        }
    }
}