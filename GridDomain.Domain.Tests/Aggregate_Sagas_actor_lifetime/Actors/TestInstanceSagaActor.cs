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
        private readonly IActorRef _observer;

        protected override bool Receive(object message)
        {
            _observer.Tell(message); //echo for testing purpose
            return base.Receive(message);
        }

        public TestInstanceSagaActor(ISagaFactory<ISagaInstance<SoftwareProgrammingSaga, SoftwareProgrammingSagaData>, GotTiredEvent> sagaStarter, ISagaFactory<ISagaInstance<SoftwareProgrammingSaga, SoftwareProgrammingSagaData>, SagaDataAggregate<SoftwareProgrammingSagaData>> sagaFactory, AggregateFactory aggregateFactory,
            IPublisher publisher,
            IActorRef observer) : base(sagaStarter, sagaFactory, aggregateFactory, publisher)
        {
            _observer = observer;
        }
    }
}