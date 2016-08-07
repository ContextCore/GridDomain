using Akka.Actor;
using GridDomain.CQRS.Messaging;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.Sagas;
using GridDomain.Node.Actors;
using GridDomain.Tests.Sagas.SoftwareProgrammingDomain.Events;
using GridDomain.Tests.Sagas.StateSagas.SampleSaga;

namespace GridDomain.Tests.Aggregate_Sagas_actor_lifetime.Actors
{

    abstract class TestHubActor : PersistentHubActor
    {
        public TestHubActor(IPersistentChildsRecycleConfiguration recycleConfiguration, 
                            string counterName,
                            IActorRef observer) : base(recycleConfiguration, counterName)
        {
        }
    }

    class TestStateSagaActor : SagaActor<SoftwareProgrammingSaga, SoftwareProgrammingSagaState, GotTiredEvent>
    {
        private readonly IActorRef _observer;

        protected override void PreStart()
        {
            base.PreStart();
            _observer.Tell(new ChildCreated(Id));
        }

        protected override void Shutdown()
        {
            _observer.Tell(new ChildTerminated(Id));
            base.Shutdown();
        }

        public TestStateSagaActor(ISagaFactory<SoftwareProgrammingSaga, GotTiredEvent> sagaStarter, 
            ISagaFactory<SoftwareProgrammingSaga, SoftwareProgrammingSagaState> sagaFactory, 
            AggregateFactory aggregateFactory, IPublisher publisher,
            IActorRef observer) : base(sagaStarter, sagaFactory, aggregateFactory, publisher)
        {
            _observer = observer;
        }
    }
}