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
        public class MostSpecifiedContructorResolver : DefaultContractResolver
        {
            protected override JsonObjectContract CreateObjectContract(Type objectType)
            {
                var contract = base.CreateObjectContract(objectType);
                if (!IsCustomStruct(objectType)) return contract;

                IList<ConstructorInfo> list =
                    objectType.GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                        .OrderBy(e => e.GetParameters().Length)
                        .ToList();
                var mostSpecific = list.LastOrDefault();
                if (mostSpecific == null) return contract;

                contract.OverrideCreator = CreateParameterizedConstructor(mostSpecific);
                var constructorParameters = CreateConstructorParameters(mostSpecific, contract.Properties);
                foreach (var p in constructorParameters)
                    contract.CreatorParameters.Add(p);

                return contract;
            }

            protected bool IsCustomStruct(Type objectType)
            {
                return objectType.IsValueType &&
                      !objectType.IsPrimitive &&
                      !objectType.IsEnum &&
                      !string.IsNullOrEmpty(objectType.Namespace) &&
                      !objectType.Namespace.StartsWith("System.");
            }

            private ObjectConstructor<object> CreateParameterizedConstructor(MethodBase method)
            {
                if (method == null) throw new ArgumentNullException(nameof(method));

                var c = method as ConstructorInfo;
                if (c != null)
                    return a => c.Invoke(a);
                return a => method.Invoke(null, a);
            }
        }

        //taken from https://github.com/danielwertheim/jsonnet-privatesetterscontractresolvers
        public class PrivateSetterContractResolver : MostSpecifiedContructorResolver
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