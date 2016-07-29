using System;
using NUnit.Framework;

namespace GridDomain.Tests.Aggregate_Sagas_actor_lifetime
{

    [TestFixture(PersistentHubTestsStatus.PersistenceCase.Saga)]
    [TestFixture(PersistentHubTestsStatus.PersistenceCase.Aggregate)]
    class PersistenceHub_on_child_creation : PersistentHub_childs_lifetime_test
    {
        private static DateTime _childTimeToLife;

        [SetUp]
        public void When_Hub_creates_a_child()
        {
            When_hub_creates_a_child();
            _childTimeToLife = PersistentHubTestsStatus.ChildTerminationTimes[_aggregateId];
        }

        [Then]
        public void Time_to_life_can_differs_by_check_time()
        {
            var approximateTimeToLife = PersistentHubTestsStatus.ChildMaxLifetime - PersistentHubTestsStatus.ChildClearTime;

            Assert.GreaterOrEqual(_childTimeToLife - DateTime.UtcNow,approximateTimeToLife);
        }

        [Then]
        public void Time_to_life_is_limited()
        {
            Assert.LessOrEqual(_childTimeToLife - DateTime.UtcNow, PersistentHubTestsStatus.ChildMaxLifetime);
        }

        [Then]
        public void It_should_be_written_in_static_result_holders()
        {
            Assert.True(PersistentHubTestsStatus.ChildTerminationTimes.ContainsKey(_aggregateId));
        }

        public PersistenceHub_on_child_creation(PersistentHubTestsStatus.PersistenceCase @case) : base(@case)
        {
        }
    }
}