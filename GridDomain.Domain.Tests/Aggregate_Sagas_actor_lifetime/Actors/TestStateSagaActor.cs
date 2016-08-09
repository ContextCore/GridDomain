using Akka;
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

        protected override bool Receive(object message)
        {
            //echo for testing purpose
            message.Match().With<Ping>(m => Sender.Tell(new Pong(m.Payload)));
            return base.Receive(message);
        }

        public TestStateSagaActor(ISagaFactory<SoftwareProgrammingSaga, GotTiredEvent> sagaStarter, 
            ISagaFactory<SoftwareProgrammingSaga, SoftwareProgrammingSagaState> sagaFactory, 
            AggregateFactory aggregateFactory, IPublisher publisher) : base(sagaStarter, sagaFactory, aggregateFactory, publisher)
        {
        }
    }
}