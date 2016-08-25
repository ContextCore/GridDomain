using System;
using GridDomain.CQRS.Messaging;
using GridDomain.EventSourcing.Sagas;
using GridDomain.Node.Actors;
using GridDomain.Tests.Sagas.SoftwareProgrammingDomain.Events;
using GridDomain.Tests.Sagas.StateSagas.SampleSaga;

namespace GridDomain.Tests.Aggregate_Sagas_actor_lifetime.Actors
{
    class TestStateSagaHubActor : SagaHubActor<SoftwareProgrammingSaga, SoftwareProgrammingSagaState>
    {
        public TestStateSagaHubActor(IPublisher publisher, ISagaProducer<SoftwareProgrammingSaga, SoftwareProgrammingSagaState> producer):
            base(publisher, new TestPersistentChildsRecycleConfiguration(), producer)
        {
        }
        protected override Type GetChildActorType(object message)
        {
            return typeof(TestStateSagaActor);
        }
    }
}