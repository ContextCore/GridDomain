using System;
using System.Threading;
using Akka.Actor;
using GridDomain.Tests.Aggregate_Sagas_actor_lifetime.Actors;
using NUnit.Framework;

namespace GridDomain.Tests.Aggregate_Sagas_actor_lifetime
{
    [TestFixture(PersistentHubTestsStatus.PersistenceCase.Aggregate)]
    [TestFixture(PersistentHubTestsStatus.PersistenceCase.IstanceSaga)]
    [TestFixture(PersistentHubTestsStatus.PersistenceCase.StateSaga)]
    class PersistenceHubOnChildActivity : PersistentHub_children_lifetime_test
    {
        private DateTime _initialTimeToLife;

        [SetUp]
        public void When_child_has_activity_while_living()
        {
            When_hub_creates_a_child();
            _initialTimeToLife = Child.ExpiresAt;

            //TODO: replace sleep with dateTime manipulations via DateTimeFacade
            Thread.Sleep(200);

            And_command_for_child_is_sent();
        }

        [Then]
        public void Hub_should_contains_child()
        {
            Assert.True(Hub.Children.ContainsKey(Infrastructure.ChildId));
        }

        [Then]
        [Ignore("Until pull request merge for faked akka.remote")]
        public void Child_lifeTime_should_be_prolongated()
        {
            Assert.GreaterOrEqual(Child.ExpiresAt, _initialTimeToLife);
        }

        [Then]
        public void Child_should_exist()
        {
            var payload = "child ping";
            Assert.AreEqual(payload, PingChild(payload).Payload);
        }

      

        public PersistenceHubOnChildActivity(PersistentHubTestsStatus.PersistenceCase @case) : base(@case)
        {
        }
    }
}