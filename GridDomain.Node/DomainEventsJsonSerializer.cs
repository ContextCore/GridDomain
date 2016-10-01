using System;
using System.Collections.Generic;
using System.IO;
using Akka.Actor;
using Akka.Serialization;
using GridDomain.EventSourcing.Adapters;
using Newtonsoft.Json;

namespace GridDomain.Node
{

    public class DomainEventsJsonSerializer : Serializer
    {
        private JsonSerializer _serializer;
        private readonly List<JsonConverter> _converters = new List<JsonConverter>();

    
        public DomainEventsJsonSerializer(ExtendedActorSystem system) : base(system)
        {
            Init();
        }

        public void Register(JsonConverter converter)
        {
            _converters.Add(converter);
        }

        public void Init()
        {
            var jsonSerializerSettings = DomainEventSerialization.GetDefault();
            foreach (var converter in _converters)
                jsonSerializerSettings.Converters.Add(converter);
            _serializer = JsonSerializer.Create(jsonSerializerSettings);
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
            using (var stream = new MemoryStream())
            using (var writer = new StreamWriter(stream))
            {
                _serializer.Serialize(writer,obj);
                return stream.ToArray();
            }
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
             using (var stream = new MemoryStream(bytes))
             using (var reader = new StreamReader(stream))
                  return _serializer.Deserialize(reader, type);;
        }
    }
}