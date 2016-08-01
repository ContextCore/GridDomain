using System;
using GridDomain.CQRS.Messaging;
using GridDomain.Node.Actors;
using GridDomain.Tests.Sagas.SoftwareProgrammingDomain.Events;
using GridDomain.Tests.Sagas.StateSagas.SampleSaga;

namespace GridDomain.Tests.Aggregate_Sagas_actor_lifetime.Actors
{
    class TestStateSagaHubActor : SagaHubActor<SoftwareProgrammingSaga, SoftwareProgrammingSagaState,GotTiredEvent>
    {
        public TestStateSagaHubActor(IPublisher publisher, IPersistentChildsRecycleConfiguration conf) : base(publisher, conf)
        {
        }
        protected override Type GetChildActorType(object message)
        {
            return typeof(TestStateSagaActor);
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