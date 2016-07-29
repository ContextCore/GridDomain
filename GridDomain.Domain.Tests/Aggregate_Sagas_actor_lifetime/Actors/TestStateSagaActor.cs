using GridDomain.CQRS.Messaging;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.Sagas;
using GridDomain.Node.Actors;
using GridDomain.Tests.Sagas.SoftwareProgrammingDomain.Events;
using GridDomain.Tests.Sagas.StateSagas.SampleSaga;

namespace GridDomain.Tests.Aggregate_Sagas_actor_lifetime
{
    class TestStateSagaActor : SagaActor<SoftwareProgrammingSaga, SoftwareProgrammingSagaState, GotTiredEvent>
    {
        protected override void PreStart()
        {
            base.PreStart();
            PersistentHubTestsStatus.ChildExistence.Add(Id);
        }

        protected override void Shutdown()
        {
            PersistentHubTestsStatus.ChildExistence.Remove(Id);
            base.Shutdown();
        }

        public TestStateSagaActor(ISagaFactory<SoftwareProgrammingSaga, GotTiredEvent> sagaStarter, ISagaFactory<SoftwareProgrammingSaga, SoftwareProgrammingSagaState> sagaFactory, AggregateFactory aggregateFactory, IPublisher publisher) : base(sagaStarter, sagaFactory, aggregateFactory, publisher)
        {
        }
    }
}