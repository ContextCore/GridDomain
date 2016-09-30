using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters;
using System.Text;
using System.Threading.Tasks;
using GridDomain.EventSourcing.DomainEventAdapters;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace GridDomain.Tests.EventsUpgrade
{
    abstract class ObjectConverter<TDeclared, TFrom, TTo> : JsonConverter
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
                var jObject = JObject.Load(reader);

                var type = PeekType(jObject);
                if (type != typeof(TFrom))
                    return serializer.ContractResolver.ResolveContract(objectType).DefaultCreator();

                existingValue = serializer.ContractResolver.ResolveContract(type).DefaultCreator();
                serializer.Populate(jObject.CreateReader(), existingValue);

                return Convert((TFrom)existingValue);
            }
            if (reader.TokenType == JsonToken.Null)
            {
                return null;
            }
            throw new JsonSerializationException();
        }

        private static Type PeekType(JObject jobject)
        {
            string typeName;
            if (string.IsNullOrEmpty(typeName = jobject["$type"]?.ToObject<string>()))
                throw new TypeNameNotFoundException();

            var type = Type.GetType(typeName);
            return type;
        }

        protected abstract TTo Convert(TFrom value);

        public override bool CanWrite => false;
        public override bool CanRead => true;

        public override bool CanConvert(Type objectType)
        {
            return typeof(TDeclared) == objectType;
        }
    }

    [TestFixture]
    class SubObject_version_upgrade
    {
        private interface ISubObject
        {
            string Value { get; set; }
        }

        class SubObject_V1 : ISubObject
        {
            public string Name { get; set; }
            public string Value { get; set; }
        }

        class SubObject_V2 : ISubObject
        {
            public int number { get; set; }
            public string Value { get; set; }
        }

        class OldEvent
        {
            public ISubObject Payload { get; set; }
        }

        class SubObjectCoverter : ObjectConverter<SubObject_V2, SubObject_V1, SubObject_V2>
        {
            protected override SubObject_V2 Convert(SubObject_V1 value)
            {
                return new SubObject_V2() { number = int.Parse(value.Name), Value = value.Value };
            }
        }



        [Test]
        public void Given_persisted_event_with_old_subobject_It_can_be_updated_to_new_subobject()
        {
            var settings = DomainEventSerialization.GetDefault();
            settings.Converters.Add(new SubObjectCoverter());

            var serializedValue = @"{
                              ""$id"": ""1"",
                              ""Payload"": {
                                ""$id"": ""2"",
                                ""$type"": ""GridDomain.Tests.EventsUpgrade.SubObject_version_upgrade+SubObject_V1, GridDomain.Tests"",
                                ""Name"": ""10"",
                                ""Value"": ""123""
                              }
                            }";

            var restoredEvent = JsonConvert.DeserializeObject<OldEvent>(serializedValue, settings);
            Assert.IsInstanceOf<SubObject_V2>(restoredEvent.Payload);
        }


        [Test]
        public void Given_event_with_old_subobject_it_Should_be_updated_to_new_subobject_by_subobject_adapter()
        {
            var initialEvent = new OldEvent() {Payload = new SubObject_V1() {Name = "10",Value = "123"} };

            var settings = DomainEventSerialization.GetDefault();
            settings.Converters.Add(new SubObjectCoverter());
     
            var serializedValue = JsonConvert.SerializeObject(initialEvent, settings);
            var restoredEvent = JsonConvert.DeserializeObject<OldEvent>(serializedValue,settings);

            Assert.IsInstanceOf<SubObject_V2>(restoredEvent.Payload);
        }
    }

    internal class TypeNameNotFoundException : Exception
    {
    }
}
