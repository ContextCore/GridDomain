using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters;
using System.Text;
using Newtonsoft.Json;

namespace GridDomain.EventSourcing
{


    public class DomainSerializer
    {
        public DomainSerializer(JsonSerializerSettings settings = null)
        {
            JsonSerializerSettings = settings ?? GetDefaultSettings();
        }

        public JsonSerializerSettings JsonSerializerSettings { get; }

        public static JsonSerializerSettings GetDefaultSettings()
        {
            return new JsonSerializerSettings
                   {
                       Formatting = Formatting.Indented,
                       TypeNameHandling = TypeNameHandling.Objects,
                       TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple,
                       CheckAdditionalContent = false,
                       ContractResolver = new DomainContractResolver(),
                       ConstructorHandling = ConstructorHandling.Default
                   };
        }

        // <summary>
        // Serializes the given object into a byte array
        // </summary>
        /// <param name="obj">The object to serialize </param>
        /// <param name="jsonSerializerSettings"></param>
        /// <returns>A byte array containing the serialized object</returns>
        public byte[] ToBinary(object obj, JsonSerializerSettings jsonSerializerSettings = null)
        {
            //TODO: use faster realization with reusable serializer
            var stringJson = JsonConvert.SerializeObject(obj, jsonSerializerSettings ?? JsonSerializerSettings);
            return Encoding.Unicode.GetBytes(stringJson);
        }

        /// <summary>
        ///     Deserializes a byte array into an object using the type hint
        /// </summary>
        /// <param name="bytes">The array containing the serialized object</param>
        /// <param name="type">The type hint of the object contained in the array</param>
        /// <returns>The object contained in the array</returns>
        public object FromBinary(byte[] bytes, Type type, JsonSerializerSettings settings = null)
        {
            using (var stream = new MemoryStream(bytes))
            using (var reader = new StreamReader(stream, Encoding.Unicode))
            {
                var jsonString = reader.ReadToEnd();
                if (string.IsNullOrEmpty(jsonString))
                    return null;

                var deserializeObject = JsonConvert.DeserializeObject(jsonString, type, settings ?? JsonSerializerSettings);
                if (deserializeObject == null)
                    throw new SerializationException("json string: " + jsonString);

                return deserializeObject;
            }
        }
    }
}