using GridDomain.EventSourcing.Adapters;
using Newtonsoft.Json;
using NUnit.Framework;

namespace GridDomain.Tests.EventsUpgrade
{
    [TestFixture]
    class Nested_property_upgrade_with_custom_constructor
    {

        [Test]
        public void All_occurance_should_be_upgraded()
        {
            var initialEvent = new Event(new Payload(new SubObject_V1("10", "123")));

            var settings = DomainEventSerialization.GetDefault();
            settings.Converters.Add(new SubObjectConverter());

            var serializedValue = JsonConvert.SerializeObject(initialEvent, settings);
            var restoredEvent = JsonConvert.DeserializeObject<Event>(serializedValue, settings);

            Assert.IsInstanceOf<SubObject_V2>(restoredEvent.Payload.Property);
        }

        [Test]
        public void Objects_with_custom_constructor_are_deserialized()
        {
            var initialEvent = new Event(new Payload(new SubObject_V1("10", "123")));

            var settings = DomainEventSerialization.GetDefault();

            var serializedValue = JsonConvert.SerializeObject(initialEvent, settings);
            var restoredEvent = JsonConvert.DeserializeObject<Event>(serializedValue, settings);

            Assert.IsInstanceOf<SubObject_V1>(restoredEvent?.Payload?.Property);
        }

        class SubObjectConverter : ObjectAdapter<SubObject_V1, SubObject_V2>
        {
            public override SubObject_V2 Convert(SubObject_V1 value)
            {
                return new SubObject_V2( int.Parse(value.Name), value.Value);
            }
        }

        private interface ISubObject
        {
            string Value { get;}
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
            public string Value { get;}
        }

        class Payload
        {
            public Payload(ISubObject property)
            {
                Property = property;
            }
            public ISubObject Property { get; }
        }
        class Event
        {
            public Event(Payload payload)
            {
                Payload = payload;
            }
            public Payload Payload { get;}
        }
    }
}