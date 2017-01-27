using System;
using Newtonsoft.Json;
using Xunit;

namespace GridDomain.Tests.XUnit.Serialization
{

    public class Private_setters_without_parameter_in_constructor_deserialization
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

        [Fact]
        public void Should_serialize_deserialize()
        {
            var testClass = new TestClass("123");
            var data = JsonConvert.SerializeObject(testClass);
            var restoredData = JsonConvert.DeserializeObject<TestClass>(data);
            Assert.Equal(testClass.Value,restoredData.Value);
        }
    }
}