using System;
using GridDomain.CQRS.Messaging.MessageRouting;
using GridDomain.Node.Actors;
using GridDomain.Tests.SampleDomain;

namespace GridDomain.Tests.Aggregate_Sagas_actor_lifetime
{
    class TestAggregateHub : AggregateHubActor<SampleAggregate>
    {
        public TestAggregateHub(ICommandAggregateLocator<SampleAggregate> locator) : base(locator)
        {
        }

        protected override Type GetChildActorType(object message)
        {
            return typeof(TestAggregateActor);
        }

        protected override void Clear()
        {
            base.Clear();
            PersistentHubTestsStatus.ChildTerminationTimes.Clear();
            foreach (var child in Children)
                SetChildLifetimeInformation(child.Key);
        }

        protected override void OnReceive(object message)
        {
            base.OnReceive(message);
            SetChildLifetimeInformation(GetChildActorId(message));
        }

        private void SetChildLifetimeInformation(Guid id)
        {
            ChildInfo childInfo;
            if (!Children.TryGetValue(id, out childInfo)) return;
            PersistentHubTestsStatus.ChildTerminationTimes[id] = childInfo.LastTimeOfAccess + ChildMaxInactiveTime;
        }

        protected override TimeSpan ChildClearPeriod { get; } = PersistentHubTestsStatus.ChildClearTime;
        protected override TimeSpan ChildMaxInactiveTime { get; } = PersistentHubTestsStatus.ChildMaxLifetime;
    }
}