using System;
using GridDomain.CQRS.Messaging;
using GridDomain.EventSourcing.Sagas;
using GridDomain.EventSourcing.Sagas.InstanceSagas;
using GridDomain.Node.Actors;
using GridDomain.Tests.Sagas.InstanceSagas;
using GridDomain.Tests.Sagas.SoftwareProgrammingDomain.Events;

namespace GridDomain.Tests.Aggregate_Sagas_actor_lifetime
{
    class TestInstanceSagaHubActor : SagaHubActor<ISagaInstance<SoftwareProgrammingSaga, SoftwareProgrammingSagaData>,
        SagaDataAggregate<SoftwareProgrammingSagaData>,
        GotTiredEvent>
    {
        public TestInstanceSagaHubActor(IPublisher publisher) : base(publisher)
        {
        }
        protected override Type GetChildActorType(object message)
        {
            return typeof(TestInstanceSagaActor);
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