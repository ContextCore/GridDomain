using System;
using System.Threading;
using NUnit.Framework;

namespace GridDomain.Tests.Aggregate_Sagas_actor_lifetime
{
    [TestFixture(PersistentHubTestsStatus.PersistenceCase.Saga)]
    [TestFixture(PersistentHubTestsStatus.PersistenceCase.Aggregate)]
    class PersistenceHub_on_child_activity : PersistentHub_childs_lifetime_test
    {
        private DateTime _initialTimeToLife;

        [SetUp]
        public void When_child_has_activity_while_living()
        {
            When_hub_creates_a_child();
            _initialTimeToLife = PersistentHubTestsStatus.ChildTerminationTimes[_aggregateId];
            Thread.Sleep(500);
            And_command_for_child_is_sent();
        }

        [Then]
        public void LifeTime_should_be_prolongated()
        {
            var prolongatedTimeToLife = PersistentHubTestsStatus.ChildTerminationTimes[_aggregateId];
            Assert.GreaterOrEqual(prolongatedTimeToLife, _initialTimeToLife);
        }

        [Then]
        public void It_should_exist()
        {
            Assert.True(PersistentHubTestsStatus.ChildExistence.Contains(_aggregateId));
        }

        public PersistenceHub_on_child_activity(PersistentHubTestsStatus.PersistenceCase @case) : base(@case)
        {
        }
    }
}