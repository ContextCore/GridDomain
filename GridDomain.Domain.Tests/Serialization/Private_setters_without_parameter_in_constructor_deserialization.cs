using System;
using Akka.DI.Core;
using GridDomain.EventSourcing.Adapters;
using GridDomain.Tests.Sagas.StateSagas.SampleSaga;
using Newtonsoft.Json;
using NUnit.Framework;

namespace GridDomain.Tests.Acceptance.Snapshots
{
    [TestFixture]
    class Private_setters_without_parameter_in_constructor_deserialization
    {
        class TestClass
        {
            public Guid Id { get; }
            public string Name { get;  }
            public string Value { get; private set; }

            public TestClass(string name)
            {
                Name = name;
                Value = "1";
            }


            private TestClass()
            {
                
            }

            public void ChangeValue(string value)
            {
                Value = value;
            }
        }

        [Test]
        public void Should_serialize_deserialize()
        {
            var testClass = new TestClass("123");
            var data = JsonConvert.SerializeObject(testClass);
            var restoredData = JsonConvert.DeserializeObject<TestClass>(data);
            Assert.AreEqual(testClass.Value,restoredData.Value);
        }
    }
}