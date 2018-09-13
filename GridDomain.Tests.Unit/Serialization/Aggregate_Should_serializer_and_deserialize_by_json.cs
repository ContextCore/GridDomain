using System;
using GridDomain.EventSourcing;
using GridDomain.Node.Serializers;
using GridDomain.Tests.Common;
using GridDomain.Tests.Unit.BalloonDomain;
using GridDomain.Tools;
using Newtonsoft.Json;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.Unit.Serialization
{
    public class Aggregate_Should_serializer_and_deserialize_by_json
    {
        private Balloon _aggregate;
        private Balloon _restoredAggregate;
        private readonly ITestOutputHelper _testOutputHelper;

        public Aggregate_Should_serializer_and_deserialize_by_json(ITestOutputHelper output)
        {
            _testOutputHelper = output;
        }
        [Fact]
        public void Test()
        {
            _aggregate = new Balloon(Guid.NewGuid().ToString(), "test");
            _aggregate.WriteNewTitle(10);
            _aggregate.Clear();

            var jsonSerializerSettings = DomainSerializer.GetDefaultSettings();
            jsonSerializerSettings.TraceWriter = new XUnitTraceWriter(_testOutputHelper);

            var jsonString = JsonConvert.SerializeObject(_aggregate, jsonSerializerSettings);
            _restoredAggregate = JsonConvert.DeserializeObject<Balloon>(jsonString, jsonSerializerSettings);
            _restoredAggregate.Clear();
            // Values_should_be_equal()
            Assert.Equal(_aggregate.Title, _restoredAggregate.Title);
            //Ids_should_be_equal()
            Assert.Equal(_aggregate.Id, _restoredAggregate.Id);
        }
    }
}