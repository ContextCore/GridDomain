using System;
using System.IO;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters;
using System.Text;
using GridDomain.Common;
using GridDomain.EventSourcing.Adapters;
using GridDomain.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Serilog;
using Wire;

namespace GridDomain.EventSourcing
{
    public class DomainSerializer
    {
        //taken from https://github.com/danielwertheim/jsonnet-privatesetterscontractresolvers
        public class PrivateSetterContractResolver : DefaultContractResolver
        {
            protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
            {
                var jProperty = base.CreateProperty(member, memberSerialization);
                if (jProperty.Writable)
                    return jProperty;

                jProperty.Writable = IsPropertyWithSetter(member);

                return jProperty;
            }
            static bool IsPropertyWithSetter(MemberInfo member)
            {
                var property = member as PropertyInfo;

                return property?.GetSetMethod(true) != null;
            }
        }

        public static JsonSerializerSettings GetDefaultSettings()
        {
            return new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                PreserveReferencesHandling = PreserveReferencesHandling.All,
                TypeNameHandling = TypeNameHandling.All,
                TypeNameAssemblyFormat = FormatterAssemblyStyle.Simple,
                CheckAdditionalContent = false,
                ContractResolver = new PrivateSetterContractResolver(),
                ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor
            };
        }

        public JsonSerializerSettings JsonSerializerSettings { get; }

        public DomainSerializer(JsonSerializerSettings settings=null)
        {
            JsonSerializerSettings = settings ?? GetDefaultSettings();
        }
        private readonly ILogger _log = Log.Logger.ForContext<DomainSerializer>();

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

        public object FromBinary(byte[] bytes, Type type, JsonSerializerSettings settings = null)
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
    }
}