using System;
using System.IO;
using System.Runtime.ExceptionServices;
using System.Runtime.Serialization;
using System.Security.Policy;
using System.Text;
using GridDomain.Common;
using GridDomain.EventSourcing.Adapters;
using GridDomain.Logging;
using Newtonsoft.Json;
using Wire;

namespace GridDomain.Scheduling.Integration
{
    public class QuartsWireJsonSerializer 
    {
        private static readonly JsonSerializerSettings JsonSerializerSettings = DomainEventSerialization.GetDefaultSettings();
        private static readonly LegacyWireSerializer OldWire_Default = new LegacyWireSerializer();
        private static readonly LegacyWireSerializer OldWire_Tolerance = new LegacyWireSerializer(true,false);
        private static readonly LegacyWireSerializer OldWire_Not_tolerance_do_not_preserve = new LegacyWireSerializer(false,false);
        private static readonly LegacyWireSerializer OldWire_not_tolerant_preserve = new LegacyWireSerializer(false,true);
        private static readonly Serializer NewWire_Tolerance_References = new Serializer(new SerializerOptions(true,true));
        private static readonly Serializer NewWire_Tolerance = new Serializer(new SerializerOptions(true,false));
        private static readonly Serializer NewWire_Default = new Serializer(new SerializerOptions(false, false));
        private static readonly Serializer NewWire_NotTolerante_References = new Serializer(new SerializerOptions(false, true));

        private static readonly Tuple<string,Serializer>[] wireSerializers = 
        {
            Tuple.Create(nameof(NewWire_Default),NewWire_Default),
            Tuple.Create(nameof(NewWire_Tolerance), NewWire_Tolerance),
            Tuple.Create(nameof(NewWire_Tolerance_References), NewWire_Tolerance_References),
            Tuple.Create(nameof(NewWire_NotTolerante_References), NewWire_NotTolerante_References)
        };

        private static readonly Tuple<string, LegacyWireSerializer>[] oldWireSerializers =
       {
            Tuple.Create(nameof(OldWire_Default),OldWire_Default),
            Tuple.Create(nameof(OldWire_Tolerance), OldWire_Tolerance),
            Tuple.Create(nameof(OldWire_Not_tolerance_do_not_preserve), OldWire_Not_tolerance_do_not_preserve),
            Tuple.Create(nameof(OldWire_not_tolerant_preserve), OldWire_not_tolerant_preserve)
        };



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
                        throw new SerializationException("json string: " + readToEnd);

                    return deserializeObject;
                }
            }
            catch (Exception ex)
            {
                if (!UseWire) ExceptionDispatchInfo.Capture(ex).Throw();
                _log.Trace("Received an error while deserializing {type} by json, switching to legacy wire. {Error}", type, ex);

                foreach (var serializer in oldWireSerializers)
                   try
                   {
                       return serializer.Item2.Deserialize(bytes, type);
                   }
                   catch (Exception ex1)
                   {
                       _log.Trace(
                           "Received an error while deserializing {type} by legacy wire {wireName}, switching to next options variant. {Error}",
                           type, serializer.Item1, ex1);
                   }

                _log.Trace("Received an error while deserializing {type} by old wire, switching to new wire.",type);

                foreach (var serializer in wireSerializers)
                    try
                    {
                        using (var stream = new MemoryStream(bytes))
                            return serializer.Item2.Deserialize(stream);
                    }
                    catch (Exception ex1)
                    {
                        _log.Trace(
                            "Received an error while deserializing {type} by new wire {wireName}, switching to next options variant. {Error}",
                            type, serializer.Item1, ex1);
                    }

                throw new SerializationException("Cannot deserialize message with any serializer");
            }
        }
    }
}