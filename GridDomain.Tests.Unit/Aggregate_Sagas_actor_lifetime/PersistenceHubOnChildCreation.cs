using GridDomain.Common;
using NUnit.Framework;

namespace GridDomain.Tests.Unit.Aggregate_Sagas_actor_lifetime
{
    [TestFixture(PersistentHubTestsStatus.PersistenceCase.IstanceSaga)]
    [TestFixture(PersistentHubTestsStatus.PersistenceCase.Aggregate)]
    class PersistenceHubOnChildCreation : PersistentHub_children_lifetime_test
    {

        public PersistenceHubOnChildCreation(PersistentHubTestsStatus.PersistenceCase @case) : base(@case)
        {
        }


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

        [Then]
        public void Hub_should_contains_child()
        {
            Assert.True(Hub.Children.ContainsKey(Infrastructure.ChildId));
        }

        [Then]
        public void Time_to_life_is_limited()
        {
            Assert.LessOrEqual(Child.ExpiresAt - BusinessDateTime.UtcNow, Hub.ChildMaxInactiveTime);
        }
       
    }
}