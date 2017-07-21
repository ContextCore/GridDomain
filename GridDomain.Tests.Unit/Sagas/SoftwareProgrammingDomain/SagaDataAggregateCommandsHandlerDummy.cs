using GridDomain.EventSourcing;
using GridDomain.Processes;
using GridDomain.Processes.State;

namespace GridDomain.Tests.Unit.Sagas.SoftwareProgrammingDomain
{
    public class SagaDataAggregateCommandsHandlerDummy<T> : AggregateCommandsHandler<ProcessStateAggregate<T>>
        where T : IProcessState {}
}