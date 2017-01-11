using GridDomain.EventSourcing;
using GridDomain.EventSourcing.Sagas;
using GridDomain.EventSourcing.Sagas.InstanceSagas;
using GridDomain.Tests.Unit.SampleDomain.Events;

namespace GridDomain.Tests.Unit.Sagas.InstanceSagas
{
    public class AsyncLongRunningSagaFactory : ISagaFactory<ISagaInstance<AsyncLongRunningSaga, TestState>, SagaDataAggregate<TestState>>,
        ISagaFactory<ISagaInstance<AsyncLongRunningSaga, TestState>, SampleAggregateCreatedEvent>
    {
        public ISagaInstance<AsyncLongRunningSaga, TestState> Create(SagaDataAggregate<TestState> message)
        {
            return SagaInstance.New(new AsyncLongRunningSaga(), message);
        }

        public ISagaInstance<AsyncLongRunningSaga, TestState> Create(SampleAggregateCreatedEvent message)
        {
            return SagaInstance.New(new AsyncLongRunningSaga(), new SagaDataAggregate<TestState>(message.SagaId, new TestState(nameof(AsyncLongRunningSaga.Initial))));
        }
    }
}