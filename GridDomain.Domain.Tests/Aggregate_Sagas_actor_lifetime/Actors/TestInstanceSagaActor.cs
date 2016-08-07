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

        public TestInstanceSagaActor(ISagaFactory<ISagaInstance<SoftwareProgrammingSaga, SoftwareProgrammingSagaData>, GotTiredEvent> sagaStarter, ISagaFactory<ISagaInstance<SoftwareProgrammingSaga, SoftwareProgrammingSagaData>, SagaDataAggregate<SoftwareProgrammingSagaData>> sagaFactory, AggregateFactory aggregateFactory,
            IPublisher publisher,
            IActorRef observer) : base(sagaStarter, sagaFactory, aggregateFactory, publisher)
        {
            _observer = observer;
        }
    }
}