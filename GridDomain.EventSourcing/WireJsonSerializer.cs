using System;
using System.IO;
using System.Runtime.ExceptionServices;
using System.Runtime.Serialization;
using System.Text;
using GridDomain.Common;
using GridDomain.EventSourcing.Adapters;
using GridDomain.Logging;
using Newtonsoft.Json;
using Wire;

namespace GridDomain.EventSourcing
{
    public class WireJsonSerializer
    {
        public JsonSerializerSettings JsonSerializerSettings { get; }

        //old wire tends to leak and does not support IDisposable
        private static readonly Func<LegacyWireSerializer> OldWire_Default = () => new LegacyWireSerializer();
        private static readonly Func<LegacyWireSerializer> OldWire_Tolerance = () =>new LegacyWireSerializer(true,false);
        private static readonly Func<LegacyWireSerializer> OldWire_Not_tolerance_do_not_preserve = () => new LegacyWireSerializer(false,false);
        private static readonly Func<LegacyWireSerializer> OldWire_not_tolerant_preserve = () =>new LegacyWireSerializer(false,true);
        //just to be sure new wire will not leak
        private static readonly Func<Serializer> NewWire_Tolerance_References = ()=>new Serializer(new SerializerOptions(true,true));
        private static readonly Func<Serializer> NewWire_Tolerance = () => new Serializer(new SerializerOptions(true,false));
        private static readonly Func<Serializer> NewWire_Default = () => new Serializer(new SerializerOptions(false, false));
        private static readonly Func<Serializer> NewWire_NotTolerante_References = () => new Serializer(new SerializerOptions(false, true));

        private static readonly Tuple<string,Func<Serializer>>[] WireSerializers = 
        {
            Tuple.Create(nameof(NewWire_Default),NewWire_Default),
            Tuple.Create(nameof(NewWire_Tolerance), NewWire_Tolerance),
            Tuple.Create(nameof(NewWire_Tolerance_References), NewWire_Tolerance_References),
            Tuple.Create(nameof(NewWire_NotTolerante_References), NewWire_NotTolerante_References)
        };

        private static readonly Tuple<string, Func<LegacyWireSerializer>>[] OldWireSerializers =
       {
            Tuple.Create(nameof(OldWire_Default),OldWire_Default),
            Tuple.Create(nameof(OldWire_Tolerance), OldWire_Tolerance),
            Tuple.Create(nameof(OldWire_Not_tolerance_do_not_preserve), OldWire_Not_tolerance_do_not_preserve),
            Tuple.Create(nameof(OldWire_not_tolerant_preserve), OldWire_not_tolerant_preserve)
        };



        public WireJsonSerializer(JsonSerializerSettings settings=null, bool useWire = true)
        {
            JsonSerializerSettings = settings ?? DomainSerializer.GetDefaultSettings();
            UseWire = useWire;
        }
        private readonly ILogger _log = LogManager.GetLogger();
        public bool UseWire { get; set; }

        // <summary>
        // Serializes the given object into a byte array
        // </summary>
        /// <param name="obj">The object to serialize </param>
        /// <param name="jsonSerializerSettings"></param>
        /// <returns>A byte array containing the serialized object</returns>
        public byte[] ToBinary(object obj, JsonSerializerSettings jsonSerializerSettings=null)
        {
            //TODO: use faster realization with reusable serializer
            var stringJson = JsonConvert.SerializeObject(obj, jsonSerializerSettings ?? JsonSerializerSettings);
            return Encoding.Unicode.GetBytes(stringJson);
        }

        /// <summary>
        /// Deserializes a byte array into an object using the type hint
        // (if any, see "IncludeManifest" above)
        /// </summary>
        /// <param name="bytes">The array containing the serialized object</param>
        /// <param name="type">The type hint of the object contained in the array</param>
        /// <returns>The object contained in the array</returns>
        [HandleProcessCorruptedStateExceptions] // sometimes legacy wire deserializer can throw System.AccessViolationException

        public object FromBinary(byte[] bytes, Type type, JsonSerializerSettings settings = null)
        {
            try
            {
                using (var stream = new MemoryStream(bytes))
                using (var reader = new StreamReader(stream, Encoding.Unicode))
                {
                    var jsonString = reader.ReadToEnd();
                    if (string.IsNullOrEmpty(jsonString))
                        return null;

                    var deserializeObject = JsonConvert.DeserializeObject(jsonString, settings ?? JsonSerializerSettings);
                    if (deserializeObject == null)
                        throw new SerializationException("json string: " + jsonString);

                    return deserializeObject;
                }
            }
            catch (Exception ex)
            {
                if (!UseWire) ExceptionDispatchInfo.Capture(ex).Throw();
                _log.Trace("Received an error while deserializing {type} by json, switching to legacy wire. {Error}", type, ex);

                foreach (var serializer in WireSerializers)
                    try
                    {
                        using (var stream = new MemoryStream(bytes))
                            return serializer.Item2().Deserialize(stream);
                    }
                    catch (Exception ex1)
                    {
                        _log.Trace(
                            "Received an error while deserializing {type} by new wire {wireName}, switching to next options variant. {Error}",
                            type, serializer.Item1, ex1);
                    }


                foreach (var serializer in OldWireSerializers)
                    try
                    {
                        return serializer.Item2().Deserialize(bytes, type);
                    }
                    catch (Exception ex1)
                    {
                        _log.Trace(
                            "Received an error while deserializing {type} by legacy wire {wireName}, switching to next options variant. {Error}",
                            type, serializer.Item1, ex1);
                    }

                _log.Trace("Received an error while deserializing {type} by old wire, switching to new wire.", type);


                throw new SerializationException("Cannot deserialize message with any serializer");
            }
        }

    }
}