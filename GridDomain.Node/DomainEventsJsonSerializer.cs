using System;
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

    class DomainEventsJsonAkkaSerializer : Serializer
    {
        private static readonly LegacyWireSerializer OldWire = new LegacyWireSerializer();
        private readonly ISoloLogger _log;
        private readonly Lazy<DomainSerializer> _serializer;

        public DomainEventsJsonAkkaSerializer(ExtendedActorSystem system) : base(system)
        {
            _log = LogManager.GetLogger();

            _serializer = new Lazy<DomainSerializer>(() =>
            {
                var settings = DomainSerializer.GetDefaultSettings();
                var ext = DomainEventsJsonSerializationExtensionProvider.Provider.Get(this.system);
                if (ext == null)
                    throw new ArgumentNullException(nameof(ext),
                        $"Cannot get {typeof(DomainEventsJsonSerializationExtension).Name} extension");

                foreach (var c in ext.Converters)
                    settings.Converters.Add(c);

                return new DomainSerializer(settings);
            });
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
            return _serializer.Value.ToBinary(obj);
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
               var deserializeObject = _serializer.Value.FromBinary(bytes,type);
               return deserializeObject ?? OldWire.Deserialize(bytes, type);
            }
            catch(Exception ex)
            {
               _log.Trace("Received an error while deserializing {type} by json, switching to legacy wire. {Error}",type,ex);
            }

            return OldWire.Deserialize(bytes, type);
        }
    }
}