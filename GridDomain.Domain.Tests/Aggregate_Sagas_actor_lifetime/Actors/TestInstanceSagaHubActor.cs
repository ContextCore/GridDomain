using System;
using Akka.Actor;
using GridDomain.CQRS.Messaging;
using GridDomain.EventSourcing.Sagas;
using GridDomain.EventSourcing.Sagas.InstanceSagas;
using GridDomain.Node.Actors;
using GridDomain.Tests.Sagas.InstanceSagas;
using GridDomain.Tests.Sagas.SoftwareProgrammingDomain.Events;

namespace GridDomain.Tests.Aggregate_Sagas_actor_lifetime.Actors
{
    class TestInstanceSagaHubActor : SagaHubActor<ISagaInstance<SoftwareProgrammingSaga, SoftwareProgrammingSagaData>,
        SagaDataAggregate<SoftwareProgrammingSagaData>,
        GotTiredEvent>
    {
        private readonly IActorRef _observer;

        public TestInstanceSagaHubActor(IPublisher publisher, IPersistentChildsRecycleConfiguration conf, IActorRef observer) : base(publisher, conf)
        {
            _observer = observer;
        }

        protected override Type GetChildActorType(object message)
        {
            return typeof(TestInstanceSagaActor);
        }
        protected override TimeSpan ChildClearPeriod { get; } = PersistentHubTestsStatus.ChildClearTime;
        protected override TimeSpan ChildMaxInactiveTime { get; } = PersistentHubTestsStatus.ChildMaxLifetime;
    }
}