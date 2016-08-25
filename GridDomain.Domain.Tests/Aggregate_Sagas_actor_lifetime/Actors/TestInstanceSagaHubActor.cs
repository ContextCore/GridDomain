using System;
using Akka.Actor;
using GridDomain.CQRS.Messaging;
using GridDomain.EventSourcing.Sagas;
using GridDomain.EventSourcing.Sagas.InstanceSagas;
using GridDomain.Node.Actors;
using GridDomain.Tests.Sagas.InstanceSagas;
using GridDomain.Tests.Sagas.SoftwareProgrammingDomain.Events;

namespace GridDomain.Tests.Aggregate_Sagas_actor_lifetime.Actors
{
    class TestInstanceSagaHubActor : SagaHubActor<ISagaInstance<SoftwareProgrammingSaga, SoftwareProgrammingSagaData>,
                                                   SagaDataAggregate<SoftwareProgrammingSagaData>>
    {
        public TestInstanceSagaHubActor(IPublisher publisher, 
            ISagaProducer<ISagaInstance<SoftwareProgrammingSaga, SoftwareProgrammingSagaData>,
                          SagaDataAggregate<SoftwareProgrammingSagaData>> producer)
                    : base(publisher, new TestPersistentChildsRecycleConfiguration(), producer)
        {
        }

        protected override Type GetChildActorType(object message)
        {
            return typeof(TestInstanceSagaActor);
        }
    }
}