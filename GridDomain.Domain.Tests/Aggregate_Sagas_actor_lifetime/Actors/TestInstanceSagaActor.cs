using Akka;
using Akka.Actor;
using GridDomain.CQRS.Messaging;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.Sagas;
using GridDomain.EventSourcing.Sagas.InstanceSagas;
using GridDomain.Node.Actors;
using GridDomain.Tests.Sagas.InstanceSagas;
using GridDomain.Tests.Sagas.SoftwareProgrammingDomain.Events;

namespace GridDomain.Tests.Aggregate_Sagas_actor_lifetime.Actors
{
    class TestInstanceSagaActor : SagaActor<ISagaInstance<SoftwareProgrammingSaga, SoftwareProgrammingSagaData>, 
        SagaDataAggregate<SoftwareProgrammingSagaData>,
        GotTiredEvent>
    {
        protected override bool Receive(object message)
        {
            //echo for testing purpose
            message.Match().With<Ping>(m => Sender.Tell(new Pong(m.Payload)));
            return base.Receive(message);
        }

        public TestInstanceSagaActor(ISagaFactory<ISagaInstance<SoftwareProgrammingSaga, SoftwareProgrammingSagaData>, GotTiredEvent> sagaStarter, ISagaFactory<ISagaInstance<SoftwareProgrammingSaga, SoftwareProgrammingSagaData>, SagaDataAggregate<SoftwareProgrammingSagaData>> sagaFactory, AggregateFactory aggregateFactory,
            IPublisher publisher) : base(sagaStarter, sagaFactory, aggregateFactory, publisher)
        {
        }
    }
}