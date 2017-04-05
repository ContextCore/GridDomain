using System;
using GridDomain.EventSourcing;
using GridDomain.Tests.Framework;
using GridDomain.Tests.XUnit.BalloonDomain;
using Newtonsoft.Json;
using Xunit;

namespace GridDomain.Tests.XUnit.Serialization
{
    public class Aggregate_Should_serializer_and_deserialize_by_json
    {
        private Balloon _aggregate;
        private Balloon _restoredAggregate;

        [Fact]
        public void Test()
        {
            _aggregate = new Balloon(Guid.NewGuid(), "test");
            _aggregate.WriteNewTitle(10);
            _aggregate.ClearEvents();

            var jsonString = JsonConvert.SerializeObject(_aggregate, DomainSerializer.GetDefaultSettings());
            _restoredAggregate = JsonConvert.DeserializeObject<Balloon>(jsonString,
                                                                                DomainSerializer.GetDefaultSettings());
            // Values_should_be_equal()
            Assert.Equal(_aggregate.Title, _restoredAggregate.Title);
            //Ids_should_be_equal()
            Assert.Equal(_aggregate.Id, _restoredAggregate.Id);
        }
    }
}