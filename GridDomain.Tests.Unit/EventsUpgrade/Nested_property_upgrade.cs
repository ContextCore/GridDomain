using GridDomain.EventSourcing;
using GridDomain.EventSourcing.Adapters;
using GridDomain.Node.Serializers;
using Newtonsoft.Json;
using Xunit;

namespace GridDomain.Tests.Unit.EventsUpgrade
{
    public class Nested_property_upgrade
    {
        private class SubObjectConverter : ObjectAdapter<SubObject_V1, SubObject_V2>
        {
            public override SubObject_V2 Convert(SubObject_V1 value)
            {
                return new SubObject_V2 {number = int.Parse(value.Name), Value = value.Value};
            }
        }

        private interface ISubObject
        {
            string Value { get; set; }
        }

        private class SubObject_V1 : ISubObject
        {
            public string Name { get; set; }
            public string Value { get; set; }
        }

        private class SubObject_V2 : ISubObject
        {
            public int number { get; set; }
            public string Value { get; set; }
        }

        private class Payload
        {
            public ISubObject Property { get; set; }
        }

        private class Event
        {
            public Payload Payload { get; set; }
        }

        [Fact]
        public void All_old_objects_should_be_updated()
        {
            var initialEvent = new Event {Payload = new Payload {Property = new SubObject_V1 {Name = "10", Value = "123"}}};

            var settings = DomainSerializer.GetDefaultSettings();
            settings.Converters.Add(new SubObjectConverter());

            var serializedValue = JsonConvert.SerializeObject(initialEvent, settings);
            var restoredEvent = JsonConvert.DeserializeObject<Event>(serializedValue, settings);

            Assert.IsAssignableFrom<SubObject_V2>(restoredEvent.Payload.Property);
        }
    }
}