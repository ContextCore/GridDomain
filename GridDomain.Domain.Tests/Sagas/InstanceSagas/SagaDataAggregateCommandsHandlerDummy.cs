using GridDomain.CQRS.Messaging.MessageRouting;
using GridDomain.EventSourcing.Sagas.InstanceSagas;

namespace GridDomain.Tests.Sagas.InstanceSagas
{
    public class SagaDataAggregateCommandsHandlerDummy<T> : 
        AggregateCommandsHandler<SagaDataAggregate<T>> where T : ISagaState
    {
      
    }
}