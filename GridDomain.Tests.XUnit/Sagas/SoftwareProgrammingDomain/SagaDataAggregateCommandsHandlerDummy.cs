using GridDomain.CQRS.Messaging.MessageRouting;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.Sagas.InstanceSagas;

namespace GridDomain.Tests.XUnit.Sagas.SoftwareProgrammingDomain
{
    public class SagaDataAggregateCommandsHandlerDummy<T> : AggregateCommandsHandler<SagaStateAggregate<T>>
        where T : ISagaState {}
}