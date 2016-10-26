using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Akka.Actor;
using Akka.Serialization;
using Akka.Util;
using GridDomain.Common;
using GridDomain.EventSourcing.Adapters;
using GridDomain.Logging;
using Newtonsoft.Json;

namespace GridDomain.Node
{
    public class DomainEventsJsonSerializer : Serializer
    {
        private JsonSerializer _serializer;
        private readonly List<JsonConverter> _converters = new List<JsonConverter>();
        private JsonSerializerSettings _jsonSerializerSettings;
        private readonly LegacyWireSerializer _oldWire;
        private ISoloLogger _log;
        public bool SupportLegacyWire = true;

        public DomainEventsJsonSerializer(ExtendedActorSystem system) : base(system)
        {
            Init();
            _oldWire = new LegacyWireSerializer();
        }

        public void Register(JsonConverter converter)
        {
            _converters.Add(converter);
            Init();
            _log = LogManager.GetLogger();
        }
        public void Register<TFrom,TTo>(ObjectAdapter<TFrom, TTo> converter)
        {
            Register((JsonConverter)converter);
        }

        public void Init()
        {
            _jsonSerializerSettings = DomainEventSerialization.GetDefaultSettings();
            _jsonSerializerSettings.Converters = _converters;
            _serializer = JsonSerializer.Create(_jsonSerializerSettings);
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
            var stringJson = JsonConvert.SerializeObject(obj, _jsonSerializerSettings);
            return Encoding.Unicode.GetBytes(stringJson);
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
            try
            {
                using (var stream = new MemoryStream(bytes))
                using (var reader = new StreamReader(stream, Encoding.Unicode))
                    return _serializer.Deserialize(reader, type);
            }
            catch(Exception ex)
            {
               _log.Trace("Received an error while deserializing {type} by json, switching to legacy wire. {Error}",type,ex);
            }

            return _oldWire.Deserialize(bytes, type);
        }
    }
}