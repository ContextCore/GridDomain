using System;
using System.Threading;
using NUnit.Framework;

namespace GridDomain.Tests.Aggregate_Sagas_actor_lifetime
{
    [TestFixture(PersistentHubTestsStatus.PersistenceCase.Aggregate)]
    class PersistenceHubOnChildActivity : PersistentHub_children_lifetime_test
    {
        private DateTime _initialTimeToLife;

        [SetUp]
        public void When_child_has_activity_while_living()
        {
            When_hub_creates_a_child();
            _initialTimeToLife = PersistentHubTestsStatus.ChildTerminationTimes[Infrastructure.ChildId];

            //TODO: replace sleep with dateTime manipulations via DateTimeFacade
            Thread.Sleep(500);

            And_command_for_child_is_sent();
        }

        [Then]
        public void LifeTime_should_be_prolongated()
        {
            var prolongatedTimeToLife = PersistentHubTestsStatus.ChildTerminationTimes[Infrastructure.ChildId];
            Assert.GreaterOrEqual(prolongatedTimeToLife, _initialTimeToLife);
        }

        [Then]
        public void It_should_exist()
        {
            Assert.True(PersistentHubTestsStatus.ChildExistence.Contains(Infrastructure.ChildId));
        }

        public PersistenceHubOnChildActivity(PersistentHubTestsStatus.PersistenceCase @case) : base(@case)
        {
        }
    }
}