using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace GridDomain.EventSourcing.Adapters
{
    /// <summary>
    /// Used to adapt objects from old versions to new one. 
    /// Support sub-property adaptation. 
    /// </summary>
    /// <typeparam name="TDeclared"></typeparam>
    /// <typeparam name="TFrom"></typeparam>
    /// <typeparam name="TTo"></typeparam>
    public abstract class ObjectAdapter<TDeclared, TFrom, TTo> : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.Value != null && reader.ValueType == typeof(TFrom))
            {
                TFrom convertFromValue = (TFrom)reader.Value;
                return Convert(convertFromValue);
            }
            if (reader.TokenType == JsonToken.StartObject && existingValue == null)
            {
                object value;
                var removed = serializer.Converters.Remove(this);
                try
                {
                    // Kludge to prevent infinite recursion when using JsonConverterAttribute on the type: deserialize to object.
                    value = serializer.Deserialize(reader);
                }
                finally
                {
                    if (removed) serializer.Converters.Add(this);
                }

                if (value is TFrom)
                    return Convert((TFrom) value);
                
                return value;

            }
            if (reader.TokenType == JsonToken.Null)
            {
                return null;
            }
            throw new JsonSerializationException();
        }

       //private static Type PeekType(JObject jobject)
       //{
       //    string typeName;
       //    if (string.IsNullOrEmpty(typeName = jobject["$type"]?.ToObject<string>()))
       //        throw new TypeNameNotFoundException();
       //
       //    var type = Type.GetType(typeName);
       //    return type;
       //}

        protected abstract TTo Convert(TFrom value);

        public override bool CanWrite => false;
        public override bool CanRead => true;

        public override bool CanConvert(Type objectType)
        {
            return typeof(TDeclared) == objectType || 
                   typeof(TFrom) == objectType;
        }
    }
}