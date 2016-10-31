using GridDomain.Common;
using NUnit.Framework;

namespace GridDomain.Tests.EventsUpgrade
{
    [TestFixture]
    public class LegacyWireSerializerTests
    {



        [Test]
        public void LegacySerializer_can_be_created()
        {
           var ser = new LegacyWireSerializer();
        }

        [Test]
        public void LegacySerializer_should_serialize_deserialize_objects()
        {
            var evt = new SubObject_V1 {Name = "10", Value = "123"};

            var serializer = new LegacyWireSerializer();
            var bytes = serializer.Serialize(evt);
            var restored = (SubObject_V1)serializer.Deserialize(bytes, evt.GetType());

            Assert.AreEqual(evt.Name, restored.Name);
            Assert.AreEqual(evt.Value, restored.Value);
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
    }
}