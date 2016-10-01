using GridDomain.EventSourcing.Adapters;
using Newtonsoft.Json;
using NUnit.Framework;

namespace GridDomain.Tests.EventsUpgrade
{
    [TestFixture]
    class Nested_property_upgrade
    {
        [Test]
        public void All_old_objects_should_be_updated()
        {
            var initialEvent = new Event() { Payload = new Payload() {Property = new SubObject_V1() { Name = "10", Value = "123" } }};

            var settings = DomainEventSerialization.GetDefault();
            settings.Converters.Add(new SubObjectConverter());

            var serializedValue = JsonConvert.SerializeObject(initialEvent, settings);
            var restoredEvent = JsonConvert.DeserializeObject<Event>(serializedValue, settings);

            Assert.IsInstanceOf<SubObject_V2>(restoredEvent.Payload.Property);
        }

        class SubObjectConverter : ObjectAdapter<ISubObject, SubObject_V1, SubObject_V2>
        {
            protected override SubObject_V2 Convert(SubObject_V1 value)
            {
                return new SubObject_V2() { number = int.Parse(value.Name), Value = value.Value };
            }
        }

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

        class Payload
        {
            public ISubObject Property { get; set; }
        }
        class Event
        {
            public Payload Payload { get; set; }
        }
    }
}