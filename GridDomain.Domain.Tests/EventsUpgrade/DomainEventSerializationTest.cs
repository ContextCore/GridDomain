using System;
using System.IO;
using System.Xml.Serialization;
using Akka.Actor;
using Akka.Serialization;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.VersionedTypeSerialization;
using GridDomain.Tests.Framework.Configuration;
using Newtonsoft.Json;
using NUnit.Framework;

namespace GridDomain.Tests.EventsUpgrade
{
    [TestFixture]
    public class DomainEventSerializationTest
    {
        //latest version of event, has version 2
        //will be resialized as TestEvent_V2
        public class TestEvent : DomainEvent
        {
            public TestEvent(Guid sourceId, DateTime? createdTime = null, Guid sagaId = new Guid()) : base(sourceId, createdTime, sagaId)
            {
            }

            public TestEvent():this(Guid.Empty)
            {
                
            }
            public override int Version => 2;
        }

        public class TestEvent_V1 : DomainEvent
        {
            public TestEvent_V1(Guid sourceId, DateTime? createdTime = null, Guid sagaId = new Guid()) : base(sourceId, createdTime, sagaId)
            {
            }
            public TestEvent_V1():this(Guid.Empty)
            {

            }
            public override int Version => 1;
        }


        [Test]
        public void Given_original_event_it_writes_its_version_in_type_name()
        {
            var evt = new TestEvent(Guid.NewGuid());
            var serializedString = SerializedToString(evt);

            var expectedTypeName = VersionedTypeName.Parse(typeof(TestEvent), evt.Version).ToString();
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
            Serialization serialization = system.Serialization;

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

            var expectedTypeName = VersionedTypeName.Parse(typeof(TestEvent_V1), evt.Version).ToString();
            Assert.True(serializedString.Contains(expectedTypeName));
        }
    }
}