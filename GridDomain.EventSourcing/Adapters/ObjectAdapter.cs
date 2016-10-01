using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace GridDomain.EventSourcing.Adapters
{
    /// <summary>
    /// Used to adapt objects from old versions to new one. 
    /// Support sub-property & collection items adaptation. 
    /// </summary>
    /// <typeparam name="TFrom"></typeparam>
    /// <typeparam name="TTo"></typeparam>
    public abstract class ObjectAdapter<TFrom, TTo> : JsonConverter,
                                                      IObjectAdapter<TFrom,TTo>
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.StartObject && existingValue == null)
            {
                object value;
                //prevent infinite recursion
                var removed = serializer.Converters.Remove(this);
                try { value = serializer.Deserialize(reader); }
                finally { if (removed) serializer.Converters.Add(this);}

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

        public abstract TTo Convert(TFrom value);

        public override bool CanWrite => false;
        public override bool CanRead => true;

        public override bool CanConvert(Type objectType)
        {
            return objectType.IsAssignableFrom(typeof(TFrom));
        }

        public object Convert(object evt)
        {
            throw new NotImplementedException();
        }
    }
}