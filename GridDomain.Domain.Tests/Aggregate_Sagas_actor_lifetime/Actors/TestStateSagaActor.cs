using Akka;
using Akka.Actor;
using GridDomain.CQRS.Messaging;
using GridDomain.EventSourcing.Sagas;
using GridDomain.Node.Actors;
using GridDomain.Tests.Sagas.SoftwareProgrammingDomain.Events;
using GridDomain.Tests.Sagas.StateSagas.SampleSaga;

namespace GridDomain.Tests.Aggregate_Sagas_actor_lifetime.Actors
{
    class TestStateSagaActor : SagaActor<SoftwareProgrammingSaga, SoftwareProgrammingSagaState>
    {

        protected override bool Receive(object message)
        {
            //echo for testing purpose
            message.Match().With<Ping>(m => Sender.Tell(new Pong(m.Payload)));
            return base.Receive(message);
        }

        public TestStateSagaActor(ISagaFactory<SoftwareProgrammingSaga, object> sagaStarter, 
            ISagaFactory<SoftwareProgrammingSaga, SoftwareProgrammingSagaState> sagaFactory, IPublisher publisher, 
            ISagaDescriptor<SoftwareProgrammingSaga> descriptor): base(sagaStarter, sagaFactory, publisher, descriptor)
        {
        }
    }
}