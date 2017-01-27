using System;
using GridDomain.EventSourcing;
using GridDomain.Tests.Framework;
using GridDomain.Tests.XUnit.SampleDomain;
using Newtonsoft.Json;
using Xunit;

namespace GridDomain.Tests.XUnit.Serialization
{
  
    public class Aggregate_Should_serializer_and_deserialize_by_json
    {
        private SampleAggregate _aggregate;
        private SampleAggregate _restoredAggregate;

        [Fact]
        public void Test()
        {
            _aggregate = new SampleAggregate(Guid.NewGuid(), "test");
            _aggregate.ChangeState(10);
            _aggregate.ClearEvents();

            var jsonString = JsonConvert.SerializeObject(_aggregate, DomainSerializer.GetDefaultSettings());
            _restoredAggregate = JsonConvert.DeserializeObject<SampleAggregate>(jsonString,DomainSerializer.GetDefaultSettings());
        // Values_should_be_equal()
            Assert.Equal(_aggregate.Value, _restoredAggregate.Value);
      //Ids_should_be_equal()
            Assert.Equal(_aggregate.Id, _restoredAggregate.Id);
        }
    }
}