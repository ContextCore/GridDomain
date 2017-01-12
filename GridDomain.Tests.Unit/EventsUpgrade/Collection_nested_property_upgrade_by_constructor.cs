using System;
using System.Linq;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.Adapters;
using Newtonsoft.Json;
using NUnit.Framework;

namespace GridDomain.Tests.Unit.EventsUpgrade
{
    [TestFixture]
    class Collection_nested_property_upgrade_by_constructor
    {

        [Test]
        public void All_occurance_should_be_upgraded()
        {
            var initialEvent = new Event(new []{ new Payload(new ISubObject[]{new SubObject_V1("10", "123")})});

            var settings = DomainSerializer.GetDefaultSettings();
            settings.Converters.Add(new SubObjectConverter());

            var serializedValue = JsonConvert.SerializeObject(initialEvent, settings);
            Console.WriteLine(serializedValue);

            var restoredEvent = JsonConvert.DeserializeObject<Event>(serializedValue, settings);

            Assert.IsInstanceOf<SubObject_V2>(restoredEvent.Payload?.FirstOrDefault()?.Property?.FirstOrDefault());
        }

        [Test]
       
        public void All_occurance_should_be_upgraded_with_implicit_collection_set()
        {
            //Should get an exception due to different serialized value
            var initialEvent = new Event(new[] { new Payload(new [] { new SubObject_V1("10", "123") }) });

            var settings = DomainSerializer.GetDefaultSettings();
            settings.Converters.Add(new SubObjectConverter());

            var serializedValue = JsonConvert.SerializeObject(initialEvent, settings);
            Console.WriteLine(serializedValue);
            Assert.Throws<ArgumentException>(() =>
                JsonConvert.DeserializeObject<Event>(serializedValue, settings));

        }


        [Test]
        public void Collections_should_be_deserialized()
        {
            var initialEvent = new Event(new[] { new Payload(new[] { new SubObject_V1("10", "123") }) });

            var settings = DomainSerializer.GetDefaultSettings();

            var serializedValue = JsonConvert.SerializeObject(initialEvent, settings);
            var restoredEvent = JsonConvert.DeserializeObject<Event>(serializedValue, settings);

            Assert.IsInstanceOf<SubObject_V1>(restoredEvent.Payload?.FirstOrDefault()?.Property?.FirstOrDefault());
        }



        class SubObjectConverter : ObjectAdapter<SubObject_V1, SubObject_V2>
        {
            public override SubObject_V2 Convert(SubObject_V1 value)
            {
                return new SubObject_V2(int.Parse(value.Name), value.Value);
            }
        }

        private interface ISubObject
        {
            string Value { get; }
        }

        class SubObject_V1 : ISubObject
        {
            public SubObject_V1(string name, string value)
            {
                Name = name;
                Value = value;
            }
            public string Name { get; }
            public string Value { get; }
        }

        class SubObject_V2 : ISubObject
        {

            public SubObject_V2(int number, string value)
            {
                Number = number;
                Value = value;
            }

            public int Number { get; }
            public string Value { get; }
        }

        class Payload
        {
            public Payload(ISubObject[] property)
            {
                Property = property;
            }
            public ISubObject[] Property { get; }
        }
        class Event
        {
            public Event(Payload[] payload)
            {
                Payload = payload;
            }
            public Payload[] Payload { get; }
        }
    }
}