using Akka;
using Akka.Actor;
using GridDomain.CQRS.Messaging;
using GridDomain.EventSourcing.Sagas;
using GridDomain.EventSourcing.Sagas.InstanceSagas;
using GridDomain.Node.Actors;
using GridDomain.Tests.Sagas.InstanceSagas;
using GridDomain.Tests.Sagas.SoftwareProgrammingDomain.Events;

namespace GridDomain.Tests.Aggregate_Sagas_actor_lifetime.Actors
{
    class TestInstanceSagaActor : SagaActor<ISagaInstance<SoftwareProgrammingSaga, SoftwareProgrammingSagaData>, 
                                          SagaDataAggregate<SoftwareProgrammingSagaData>>
    {
        protected override bool Receive(object message)
        {
            //echo for testing purpose
            message.Match().With<Ping>(m => Sender.Tell(new Pong(m.Payload)));
            return base.Receive(message);
        }

        public TestInstanceSagaActor(ISagaFactory<ISagaInstance<SoftwareProgrammingSaga, SoftwareProgrammingSagaData>, object> sagaStarter, 
                                     ISagaFactory<ISagaInstance<SoftwareProgrammingSaga, SoftwareProgrammingSagaData>, 
                                     SagaDataAggregate<SoftwareProgrammingSagaData>> sagaFactory,
                                     IPublisher publisher,
                                     ISagaDescriptor<ISagaInstance<SoftwareProgrammingSaga, SoftwareProgrammingSagaData>> descriptor) :
            base(sagaStarter, sagaFactory, publisher, descriptor)
        {
        }
    }
}