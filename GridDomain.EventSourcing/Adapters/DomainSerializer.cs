using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace GridDomain.EventSourcing.Adapters
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

        private readonly JsonSerializerSettings Settings;

        public static DomainSerializer Instance { get; } = new DomainSerializer();

        public DomainSerializer(JsonSerializerSettings settings = null)
        {
            Settings = settings ?? GetDefaultSettings();
        }


        public string SerializeObject(object obj)
        {
            return JsonConvert.SerializeObject(obj, Settings);
        }

        public static string Serialize(object obj)
        {
            return Instance.SerializeObject(obj);
        }

        public T DeserializeObject<T>(string jsonString)
        {
            return JsonConvert.DeserializeObject<T>(jsonString, Settings);
        }

        public static T Deserialize<T>(string jsonString)
        {
            return Instance.DeserializeObject<T>(jsonString);
        }

        public byte[] ToBinary(object obj)
        {
            return Encoding.Unicode.GetBytes(SerializeObject(obj));
        }

        public object FromBinary(byte[] bytes, Type type = null)
        {
            using (var stream = new MemoryStream(bytes))
            using (var reader = new StreamReader(stream, Encoding.Unicode))
            {
                return JsonConvert.DeserializeObject(reader.ReadToEnd(), Settings);
            }
        }
    }
}