using GridDomain.CQRS.Messaging.MessageRouting;
using GridDomain.EventSourcing.Sagas.InstanceSagas;

namespace GridDomain.Tests.XUnit.Sagas.InstanceSagas
{
    public class SagaDataAggregateCommandsHandlerDummy<T> : 
        AggregateCommandsHandler<SagaStateAggregate<T>> where T : ISagaState
    {
      
    }
}