using GridDomain.CQRS.Messaging;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.Sagas;
using GridDomain.EventSourcing.Sagas.InstanceSagas;
using GridDomain.Node.Actors;
using GridDomain.Tests.Sagas.InstanceSagas;
using GridDomain.Tests.Sagas.SoftwareProgrammingDomain.Events;

namespace GridDomain.Tests.Aggregate_Sagas_actor_lifetime
{
    class TestInstanceSagaActor : SagaActor<ISagaInstance<SoftwareProgrammingSaga, SoftwareProgrammingSagaData>, 
        SagaDataAggregate<SoftwareProgrammingSagaData>,
        GotTiredEvent>
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

        public TestInstanceSagaActor(ISagaFactory<ISagaInstance<SoftwareProgrammingSaga, SoftwareProgrammingSagaData>, GotTiredEvent> sagaStarter, ISagaFactory<ISagaInstance<SoftwareProgrammingSaga, SoftwareProgrammingSagaData>, SagaDataAggregate<SoftwareProgrammingSagaData>> sagaFactory, AggregateFactory aggregateFactory, IPublisher publisher) : base(sagaStarter, sagaFactory, aggregateFactory, publisher)
        {
        }
    }
}