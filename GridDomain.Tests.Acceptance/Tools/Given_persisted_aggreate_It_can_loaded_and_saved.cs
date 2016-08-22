using System;
using GridDomain.Tests.SampleDomain;
using GridDomain.Tests.Tools;
using NUnit.Framework;

namespace GridDomain.Tests.Acceptance.Tools
{
    [TestFixture]
    class Given_persisted_aggreate_It_can_loaded_and_saved
    {
        private SampleAggregate _aggregate;
        private Guid _aggregateId;
        private string _agregateValue;

        [TestFixtureSetUp]
        public void Given_persisted_aggreate()
        {
            _aggregateId = Guid.NewGuid();
            _agregateValue = "initial";
            var aggregate = new SampleAggregate(_aggregateId, _agregateValue);

            using (var repo = TestRepository.NewPersistent())
            {
                repo.Save(aggregate);
            }

            using (var repo = TestRepository.NewPersistent())
            {
                _aggregate = repo.LoadAggregate<SampleAggregate>(aggregate.Id);
            }
        }

        [Then]
        public void Aggregate_has_correct_id()
        {
            Assert.AreEqual(_aggregateId, _aggregate.Id);
        }


        [Then]
        public void Aggregate_has_state_from_changed_event()
        {
            Assert.AreEqual(_agregateValue, _aggregate.Value);
        }

    }
}