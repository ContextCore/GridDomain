using System;
using GridDomain.EventSourcing.Adapters;
using GridDomain.Node;
using GridDomain.Tests.Framework;
using GridDomain.Tests.SampleDomain;
using GridDomain.Tests.SampleDomain.Events;
using GridDomain.Tools.Repositories;
using Newtonsoft.Json;
using NUnit.Framework;

namespace GridDomain.Tests.Serialization
{
    [TestFixture]
    class Aggregate_Should_serializer_and_deserialize_by_json
    {
        private SampleAggregate _aggregate;
        private SampleAggregate _restoredAggregate;

        [OneTimeSetUp]
        public void Test()
        {
            _aggregate = new SampleAggregate(Guid.NewGuid(), "test");
            _aggregate.ChangeState(10);
            _aggregate.ClearEvents();

            var jsonString = JsonConvert.SerializeObject(_aggregate, DomainSerializer.GetDefaultSettings());
            _restoredAggregate = JsonConvert.DeserializeObject<SampleAggregate>(jsonString,DomainSerializer.GetDefaultSettings());
        }

        [Test]
        public void Values_should_be_equal()
        {
            Assert.AreEqual(_aggregate.Value, _restoredAggregate.Value);
        }

        [Test]
        public void Ids_should_be_equal()
        {
            Assert.AreEqual(_aggregate.Id, _restoredAggregate.Id);
        }
    }
}