using System;
using System.IO;
using System.Runtime.ExceptionServices;
using System.Runtime.Serialization;
using System.Text;
using GridDomain.Common;
using GridDomain.EventSourcing.Adapters;
using GridDomain.Logging;
using Newtonsoft.Json;

namespace GridDomain.Scheduling.Integration
{
    public class QuartsWireJsonSerializer 
    {
        private static readonly JsonSerializerSettings JsonSerializerSettings = DomainEventSerialization.GetDefaultSettings();
        private static readonly LegacyWireSerializer OldWire = new LegacyWireSerializer();
        private readonly ISoloLogger _log = LogManager.GetLogger();
        public bool UseWire { get; set; } = true;




        // <summary>
        // Serializes the given object into a byte array
        // </summary>
        /// <param name="obj">The object to serialize </param>
        /// <returns>A byte array containing the serialized object</returns>
        public byte[] ToBinary(object obj)
        {
            //TODO: use faster realization with reusable serializer
            var stringJson = JsonConvert.SerializeObject(obj, JsonSerializerSettings);
            return Encoding.Unicode.GetBytes(stringJson);
        }

        /// <summary>
        /// Deserializes a byte array into an object using the type hint
        // (if any, see "IncludeManifest" above)
        /// </summary>
        /// <param name="bytes">The array containing the serialized object</param>
        /// <param name="type">The type hint of the object contained in the array</param>
        /// <returns>The object contained in the array</returns>
        public object FromBinary(byte[] bytes, Type type)
        {
            try
            {
                using (var stream = new MemoryStream(bytes))
                using (var reader = new StreamReader(stream, Encoding.Unicode))
                {
                    var readToEnd = reader.ReadToEnd();
                    var deserializeObject = JsonConvert.DeserializeObject(readToEnd, JsonSerializerSettings);
                    if (deserializeObject == null)
                        throw new SerializationException();

                    return deserializeObject;
                }
            }
            catch (Exception ex)
            {
                if (!UseWire) ExceptionDispatchInfo.Capture(ex).Throw();
                _log.Trace("Received an error while deserializing {type} by json, switching to legacy wire. {Error}", type, ex);
                return OldWire.Deserialize(bytes, type);

            }
        }
    }
}