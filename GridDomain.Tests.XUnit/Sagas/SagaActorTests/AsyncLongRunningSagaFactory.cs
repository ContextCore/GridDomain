using GridDomain.EventSourcing.Sagas;
using GridDomain.EventSourcing.Sagas.InstanceSagas;
using GridDomain.Tests.XUnit.SampleDomain.Events;

namespace GridDomain.Tests.XUnit.Sagas.SagaActorTests
{
    public class AsyncLongRunningSagaFactory : ISagaFactory<ISagaInstance<AsyncLongRunningSaga, TestState>, SagaStateAggregate<TestState>>,
        ISagaFactory<ISagaInstance<AsyncLongRunningSaga, TestState>, SampleAggregateCreatedEvent>
    {
        public ISagaInstance<AsyncLongRunningSaga, TestState> Create(SagaStateAggregate<TestState> message)
        {
            return SagaInstance.New(new AsyncLongRunningSaga(), message);
        }

        public ISagaInstance<AsyncLongRunningSaga, TestState> Create(SampleAggregateCreatedEvent message)
        {
            return SagaInstance.New(new AsyncLongRunningSaga(), new SagaStateAggregate<TestState>(new TestState(message.SagaId,nameof(AsyncLongRunningSaga.Initial))));
        }
    }
}