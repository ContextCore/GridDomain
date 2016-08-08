using System;
using NUnit.Framework;

namespace GridDomain.Tests.Aggregate_Sagas_actor_lifetime
{

    [TestFixture(PersistentHubTestsStatus.PersistenceCase.IstanceSaga)]
    [TestFixture(PersistentHubTestsStatus.PersistenceCase.Aggregate)]
    [TestFixture(PersistentHubTestsStatus.PersistenceCase.StateSaga)]
    class PersistenceHubOnChildInactive : PersistentHub_children_lifetime_test
    {
        [SetUp]
        public void When_child_became_inactive_too_long()
        {
            When_hub_creates_a_child();
            And_it_is_not_active_until_lifetime_period_is_expired();
        }

        [Then]
        public void It_should_be_removed_from_hub()
        {
           // Assert.False(PersistentHubTestsStatus.ChildTerminationTimes.ContainsKey(Infrastructure.ChildId));
        }

        [Then]
        public void It_should_be_terminated()
        {
          //  Assert.False(PersistentHubTestsStatus.ChildExistence.Contains(Infrastructure.ChildId));
        }

        public PersistenceHubOnChildInactive(PersistentHubTestsStatus.PersistenceCase @case) : base(@case)
        {
        }
    }
}