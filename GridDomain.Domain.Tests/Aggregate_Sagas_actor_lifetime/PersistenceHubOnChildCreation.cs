using System;
using GridDomain.Common;
using GridDomain.Node.Actors;
using NUnit.Framework;

namespace GridDomain.Tests.Aggregate_Sagas_actor_lifetime
{
  //  [TestFixture(PersistentHubTestsStatus.PersistenceCase.IstanceSaga)]
  //  [TestFixture(PersistentHubTestsStatus.PersistenceCase.Aggregate)]
    [TestFixture(PersistentHubTestsStatus.PersistenceCase.StateSaga)]
    class PersistenceHubOnChildCreation : PersistentHub_children_lifetime_test
    {

        public PersistenceHubOnChildCreation(PersistentHubTestsStatus.PersistenceCase @case) : base(@case)
        {
        }

        private DateTime _expiresAt;


        [SetUp]
        public void When_Hub_creates_a_child()
        {
            When_hub_creates_a_child();
            _expiresAt = Child.ExpiresAt;
        }

        [Then]
        public void Time_to_life_is_set_and_can_differs_by_check_time()
        {
            Assert.GreaterOrEqual(_expiresAt - BusinessDateTime.UtcNow, Hub.ChildMaxInactiveTime - Hub.ChildClearPeriod);
        }


        [Then]
        public void Time_to_life_is_limited()
        {
            Assert.LessOrEqual(_expiresAt - BusinessDateTime.UtcNow, Hub.ChildMaxInactiveTime);
        }

       
    }
}