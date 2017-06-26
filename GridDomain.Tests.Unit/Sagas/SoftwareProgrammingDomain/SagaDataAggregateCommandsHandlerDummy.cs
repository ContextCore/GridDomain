using GridDomain.EventSourcing;
using GridDomain.EventSourcing.Sagas.InstanceSagas;

namespace GridDomain.Tests.Unit.Sagas.SoftwareProgrammingDomain
{
    public class SagaDataAggregateCommandsHandlerDummy<T> : AggregateCommandsHandler<SagaStateAggregate<T>>
        where T : ISagaState {}
}