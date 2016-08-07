using System;
using Akka.Actor;
using GridDomain.CQRS.Messaging.MessageRouting;
using GridDomain.Node.Actors;
using GridDomain.Tests.SampleDomain;

namespace GridDomain.Tests.Aggregate_Sagas_actor_lifetime.Actors
{

    internal class ClearComplete
    {
    }

    internal class ChildLifeTimeChanged
    {
        public Guid Id { get;  }
        public DateTime DateTime { get;  }

        public ChildLifeTimeChanged(Guid id, DateTime dateTime)
        {
            Id = id;
            DateTime = dateTime;
        }
    }


    class TestAggregateHub : AggregateHubActor<SampleAggregate>
    {
        private readonly IActorRef _observer;

        public TestAggregateHub(ICommandAggregateLocator<SampleAggregate> locator,
                                IPersistentChildsRecycleConfiguration conf,
                                IActorRef observer) : base(locator,conf)
        {
            _observer = observer;
        }

        protected override Type GetChildActorType(object message)
        {
            return typeof(TestAggregateActor);
        }

        protected override void Clear()
        {
            base.Clear();
            foreach (var child in Children)
                SetChildLifetimeInformation(child.Key);
            _observer.Tell(new ClearComplete());
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
            //PersistentHubTestsStatus.ChildTerminationTimes[id] = childInfo.LastTimeOfAccess + ChildMaxInactiveTime;
            _observer.Tell(new ChildLifeTimeChanged(id, childInfo.LastTimeOfAccess + ChildMaxInactiveTime));
        }

    
        protected override TimeSpan ChildClearPeriod { get; } = PersistentHubTestsStatus.ChildClearTime;
        protected override TimeSpan ChildMaxInactiveTime { get; } = PersistentHubTestsStatus.ChildMaxLifetime;
    }
}