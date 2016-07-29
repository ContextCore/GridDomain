using System;
using NUnit.Framework;

namespace GridDomain.Tests.Aggregate_Sagas_actor_lifetime
{

    [TestFixture(PersistentHubTestsStatus.PersistenceCase.IstanceSaga)]
    [TestFixture(PersistentHubTestsStatus.PersistenceCase.Aggregate)]
    [TestFixture(PersistentHubTestsStatus.PersistenceCase.StateSaga)]
    class PersistenceHubOnChildCreation : PersistentHub_children_lifetime_test
    {
        [SetUp]
        public void When_Hub_creates_a_child()
        {
            When_hub_creates_a_child();
        }

        [Then]
        public void Time_to_life_can_differs_by_check_time()
        {
            var approximateTimeToLife = PersistentHubTestsStatus.ChildMaxLifetime - PersistentHubTestsStatus.ChildClearTime;
            var childTimeToLife = PersistentHubTestsStatus.ChildTerminationTimes[Infrastructure.ChildId];

            Assert.GreaterOrEqual(childTimeToLife - DateTime.UtcNow,approximateTimeToLife);
        }

        [Then]
        public void Time_to_life_is_limited()
        {
           var  childTimeToLife = PersistentHubTestsStatus.ChildTerminationTimes[Infrastructure.ChildId];
            Assert.LessOrEqual(childTimeToLife - DateTime.UtcNow, PersistentHubTestsStatus.ChildMaxLifetime);
        }

        [Then]
        public void It_should_be_written_in_static_result_holders()
        {
            Assert.True(PersistentHubTestsStatus.ChildTerminationTimes.ContainsKey(Infrastructure.ChildId));
        }

        public PersistenceHubOnChildCreation(PersistentHubTestsStatus.PersistenceCase @case) : base(@case)
        {
        }
    }
}