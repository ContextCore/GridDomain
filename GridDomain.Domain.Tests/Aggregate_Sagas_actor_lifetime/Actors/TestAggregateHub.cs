using System;
using Akka.Actor;
using GridDomain.CQRS.Messaging.MessageRouting;
using GridDomain.Node.Actors;
using GridDomain.Tests.SampleDomain;

namespace GridDomain.Tests.Aggregate_Sagas_actor_lifetime.Actors
{
    class TestAggregateHub : AggregateHubActor<SampleAggregate>
    {
        public TestAggregateHub(ICommandAggregateLocator<SampleAggregate> locator) : base(locator,new TestPersistentChildsRecycleConfiguration())
        {
        }

        protected override Type GetChildActorType(object message)
        {
            return typeof(TestAggregateActor);
        }
    }
}