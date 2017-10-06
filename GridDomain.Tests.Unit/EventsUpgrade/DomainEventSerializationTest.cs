using System;
using Akka.Actor;
using GridDomain.EventSourcing.VersionedTypeSerialization;
using GridDomain.Node.Configuration;
using GridDomain.Tests.Common.Configuration;
using GridDomain.Tests.Unit.EventsUpgrade.Events;
using Xunit;

namespace GridDomain.Tests.Unit.EventsUpgrade
{
    public class DomainEventSerializationTest
    {
        private static string SerializedToString<T>(T evt)
        {
            var system = ActorSystem.Create("example", new AutoTestAkkaConfiguration().ToDebugStandAloneInMemorySystemConfig());

            // Get the Serialization Extension
            var serialization = system.Serialization;

            // Find the Serializer for it
            var serializer = serialization.FindSerializerFor(evt);

            return serializer.ToBinary(evt).ToString();
        }

        [Fact(Skip = "Disabled due to backward versioning is not ready")]
        public void Given_historical_event_it_writes_version_from_type_name()
        {
            var evt = new TestEvent_V1(Guid.NewGuid());
            var serializedString = SerializedToString(evt);

            var expectedTypeName = VersionedTypeName.Parse(typeof(TestEvent_V1)).ToString();
            Assert.Contains(expectedTypeName, serializedString);
        }

        //latest version of event, has version 2
        //will be serialized as TestEvent_V2

        [Fact(Skip = "Disabled due to backward versioning is not ready")]
        public void Given_original_event_it_writes_its_version_in_type_name()
        {
            var evt = new TestEvent(Guid.NewGuid());
            var serializedString = SerializedToString(evt);

            var expectedTypeName = VersionedTypeName.Parse(typeof(TestEvent), 2).ToString();
            Assert.Contains(expectedTypeName, serializedString);
        }
    }
}