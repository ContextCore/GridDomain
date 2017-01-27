using GridDomain.EventSourcing;
using GridDomain.EventSourcing.Adapters;
using Newtonsoft.Json;
using Xunit;

namespace GridDomain.Tests.XUnit.EventsUpgrade
{
    
    public class Nested_property_upgrade_with_custom_constructor
    {

        [Fact]
        public void All_occurance_should_be_upgraded()
        {
            var initialEvent = new Event(new Payload(new SubObject_V1("10", "123")));

            var settings = DomainSerializer.GetDefaultSettings();
            settings.Converters.Add(new SubObjectConverter());

            var serializedValue = JsonConvert.SerializeObject(initialEvent, settings);
            var restoredEvent = JsonConvert.DeserializeObject<Event>(serializedValue, settings);

            Assert.IsAssignableFrom<SubObject_V2>(restoredEvent.Payload.Property);
        }

        [Fact]
        public void Objects_with_custom_constructor_are_deserialized()
        {
            var initialEvent = new Event(new Payload(new SubObject_V1("10", "123")));

            var settings = DomainSerializer.GetDefaultSettings();

            var serializedValue = JsonConvert.SerializeObject(initialEvent, settings);
            var restoredEvent = JsonConvert.DeserializeObject<Event>(serializedValue, settings);

            Assert.IsAssignableFrom<SubObject_V1>(restoredEvent?.Payload?.Property);
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