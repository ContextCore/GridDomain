using Automatonymous;
using GridDomain.CQRS.Messaging;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.Sagas;
using GridDomain.EventSourcing.Sagas.InstanceSagas;

namespace GridDomain.Node.Actors
{
    public class InstanceSagaActor<TSaga, TData, TStartMessage>:
        SagaActor<ISagaInstance, SagaDataAggregate<TData>, TStartMessage> 
        where TStartMessage : DomainEvent 
        where TData : ISagaState<State>
    {
        public InstanceSagaActor(ISagaFactory<ISagaInstance, TStartMessage> sagaStarter, ISagaFactory<ISagaInstance, SagaDataAggregate<TData>> sagaFactory, IEmptySagaFactory<ISagaInstance> emptySagaFactory, IPublisher publisher) : base(sagaStarter, sagaFactory, emptySagaFactory, publisher)
        {
        }
    }
}