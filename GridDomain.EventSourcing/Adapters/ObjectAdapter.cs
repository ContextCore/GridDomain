using System;
using Newtonsoft.Json;

namespace GridDomain.EventSourcing.Adapters
{
    /// <summary>
    ///     Used to adapt objects from old versions to new one.
    ///     Support sub-property & collection items adaptation.
    /// </summary>
    /// <typeparam name="TFrom"></typeparam>
    /// <typeparam name="TTo"></typeparam>
    public abstract class ObjectAdapter<TFrom, TTo> : JsonConverter,
                                                      IObjectAdapter<TFrom, TTo>
    {
        public override bool CanWrite => false;
        public override bool CanRead => true;

        public abstract TTo Convert(TFrom value);

        public object ConvertAny(object evt)
        {
            return Convert((TFrom) evt);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            object value;
            //prevent infinite recursion
            var removed = serializer.Converters.Remove(this);
            try { value = serializer.Deserialize(reader); }
            finally
            {
                if (removed) serializer.Converters.Add(this);
            }

            if (value is TFrom) return ConvertAny(value);

            return value;
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType.IsAssignableFrom(typeof(TFrom));
        }
    }
}