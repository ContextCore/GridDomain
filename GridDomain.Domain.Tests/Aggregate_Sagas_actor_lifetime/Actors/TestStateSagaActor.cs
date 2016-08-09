using Akka.Actor;
using GridDomain.CQRS.Messaging;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.Sagas;
using GridDomain.Node.Actors;
using GridDomain.Tests.Sagas.SoftwareProgrammingDomain.Events;
using GridDomain.Tests.Sagas.StateSagas.SampleSaga;

namespace GridDomain.Tests.Aggregate_Sagas_actor_lifetime.Actors
{
    class TestStateSagaActor : SagaActor<SoftwareProgrammingSaga, SoftwareProgrammingSagaState, GotTiredEvent>
    {
        private readonly IActorRef _observer;

        protected override bool Receive(object message)
        {
            //echo for testing purpose
            _observer.Tell(message);
            Sender.Tell(message);
            return base.Receive(message);
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