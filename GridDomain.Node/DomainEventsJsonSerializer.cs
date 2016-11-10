using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.ExceptionServices;
using System.Runtime.Serialization;
using System.Text;
using Akka.Actor;
using Akka.Serialization;
using Akka.Util;
using GridDomain.Common;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.Adapters;
using GridDomain.Logging;
using Newtonsoft.Json;

namespace GridDomain.Node
{
    public class DomainEventsJsonSerializer : Serializer
    {
        private static readonly JsonSerializerSettings JsonSerializerSettings = DomainEventSerialization.GetDefaultSettings();
        private static readonly WireJsonSerializer Serializer = new WireJsonSerializer(JsonSerializerSettings);
        public bool UseWire { get; set; } = true;

        public DomainEventsJsonSerializer(ExtendedActorSystem system) : base(system)
        {
        }

        internal static void Register(JsonConverter converter)
        {
            JsonSerializerSettings.Converters.Add(converter);
        }

        public static void Clear()
        {
            JsonSerializerSettings.Converters.Clear();
        }

        internal static void Register<TFrom,TTo>(ObjectAdapter<TFrom, TTo> converter)
        {
            Register((JsonConverter)converter);
        }

        /// <summary>
        /// Determines whether the deserializer needs a type hint to deserialize
        /// an object.
        /// </summary>
        public override bool IncludeManifest => true;

        /// <summary>
        /// Completely unique value to identify this implementation of the
        /// <see cref="Serializer"/> used to optimize network traffic
        /// </summary>
        public override int Identifier => 21;


        // <summary>
        // Serializes the given object into a byte array
        // </summary>
        /// <param name="obj">The object to serialize </param>
        /// <returns>A byte array containing the serialized object</returns>
        public override byte[] ToBinary(object obj)
        {
            //TODO: use faster realization with reusable serializer
            return Serializer.ToBinary(obj, JsonSerializerSettings);
            
        }

        /// <summary>
        /// Deserializes a byte array into an object using the type hint
        // (if any, see "IncludeManifest" above)
        /// </summary>
        /// <param name="bytes">The array containing the serialized object</param>
        /// <param name="type">The type hint of the object contained in the array</param>
        /// <returns>The object contained in the array</returns>
        public override object FromBinary(byte[] bytes, Type type)
        {
            return Serializer.FromBinary(bytes, type);
        }
    }
}