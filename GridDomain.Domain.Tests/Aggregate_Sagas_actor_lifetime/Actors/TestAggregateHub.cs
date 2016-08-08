using System;
using Akka.Actor;
using GridDomain.CQRS.Messaging.MessageRouting;
using GridDomain.Node.Actors;
using GridDomain.Tests.SampleDomain;

namespace GridDomain.Tests.Aggregate_Sagas_actor_lifetime.Actors
{
    class TestAggregateHub : AggregateHubActor<SampleAggregate>
    {
        public TestAggregateHub(ICommandAggregateLocator<SampleAggregate> locator,
                                IPersistentChildsRecycleConfiguration conf) : base(locator,conf)
        {
        }

        protected override Type GetChildActorType(object message)
        {
            return typeof(TestAggregateActor);
        }
    
        protected override TimeSpan ChildClearPeriod { get; } = PersistentHubTestsStatus.ChildClearTime;
        protected override TimeSpan ChildMaxInactiveTime { get; } = PersistentHubTestsStatus.ChildMaxLifetime;
    }
}