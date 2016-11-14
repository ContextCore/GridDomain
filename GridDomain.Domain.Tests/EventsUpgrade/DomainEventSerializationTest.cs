using System;
using Akka.Actor;
using Akka.Serialization;
using GridDomain.EventSourcing.VersionedTypeSerialization;
using GridDomain.Tests.EventsUpgrade.Events;
using GridDomain.Tests.Framework.Configuration;
using NUnit.Framework;

namespace GridDomain.Tests.EventsUpgrade
{
    [TestFixture]
    [Ignore("Disabled due to backward versioning is not ready")]
    public class DomainEventSerializationTest
    {
        //latest version of event, has version 2
        //will be serialized as TestEvent_V2

        [Test]
        public void Given_original_event_it_writes_its_version_in_type_name()
        {
            var evt = new TestEvent(Guid.NewGuid());
            var serializedString = SerializedToString(evt);

            var expectedTypeName = VersionedTypeName.Parse(typeof(TestEvent), 2).ToString();
            Assert.True(serializedString.Contains(expectedTypeName));
        }

        private static string SerializedToString<T>(T evt)
        {
            //XmlSerializer xmlSerializer = new XmlSerializer(evt.GetType());
            //using (StringWriter textWriter = new StringWriter())
            //{
            //    xmlSerializer.Serialize(textWriter, evt);
            //    return textWriter.ToString();
            //}
            ActorSystem system = ActorSystem.Create("example",new AutoTestAkkaConfiguration().ToStandAloneInMemorySystemConfig());

            // Get the Serialization Extension
            var serialization = system.Serialization;

            // Find the Serializer for it
            Serializer serializer = serialization.FindSerializerFor(evt);

            //var serializedString = JsonConvert.SerializeObject(evt, new JsonSerializerSettings
            //{
            //    TypeNameHandling = TypeNameHandling.All
            //});

            return serializer.ToBinary(evt).ToString();
        }

        [Test]
        public void Given_historical_event_it_writes_version_from_type_name()
        {
            var evt = new TestEvent_V1(Guid.NewGuid());
            var serializedString = SerializedToString(evt);

            var expectedTypeName = VersionedTypeName.Parse(typeof(TestEvent_V1)).ToString();
            Assert.True(serializedString.Contains(expectedTypeName));
        }
    }
}