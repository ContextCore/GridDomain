using GridDomain.EventSourcing.Sagas;
using GridDomain.EventSourcing.Sagas.InstanceSagas;
using GridDomain.Tests.XUnit.SampleDomain.Events;
using Serilog;

namespace GridDomain.Tests.XUnit.Sagas.SagaActorTests
{
    public class AsyncLongRunningSagaFactory :
        ISagaFactory<ISagaInstance<AsyncLongRunningSaga, TestState>, SagaStateAggregate<TestState>>,
        ISagaFactory<ISagaInstance<AsyncLongRunningSaga, TestState>, SampleAggregateCreatedEvent>
    {
        private readonly ILogger _log;

        public AsyncLongRunningSagaFactory(ILogger log)
        {
            _log = log;
        }

        public ISagaInstance<AsyncLongRunningSaga, TestState> Create(SagaStateAggregate<TestState> message)
        {
            return SagaInstance.New(new AsyncLongRunningSaga(), message, _log);
        }

        public ISagaInstance<AsyncLongRunningSaga, TestState> Create(SampleAggregateCreatedEvent message)
        {
            return SagaInstance.New(new AsyncLongRunningSaga(),
                                    new SagaStateAggregate<TestState>(new TestState(message.SagaId, nameof(AsyncLongRunningSaga.Initial))),
                                    _log);
        }
    }
}