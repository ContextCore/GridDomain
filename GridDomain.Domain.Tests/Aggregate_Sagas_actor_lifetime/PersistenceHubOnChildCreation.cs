using System;
using GridDomain.Common;
using GridDomain.Node.Actors;
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
        public void Time_to_life_is_set_and_can_differs_by_check_time()
        {
            Assert.GreaterOrEqual(Child.ExpiresAt - BusinessDateTime.UtcNow, Hub.ChildMaxInactiveTime - Hub.ChildClearPeriod);
        }

        private ChildInfo Child => Hub.Children[Infrastructure.ChildId];

        [Then]
        public void Time_to_life_is_limited()
        {
            Assert.LessOrEqual(Child.ExpiresAt - BusinessDateTime.UtcNow, Hub.ChildMaxInactiveTime);
        }

        public PersistenceHubOnChildCreation(PersistentHubTestsStatus.PersistenceCase @case) : base(@case)
        {
        }
    }
}