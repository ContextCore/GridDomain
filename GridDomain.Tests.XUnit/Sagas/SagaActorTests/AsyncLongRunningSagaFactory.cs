using GridDomain.EventSourcing.Sagas;
using GridDomain.EventSourcing.Sagas.InstanceSagas;
using GridDomain.Tests.XUnit.SampleDomain.Events;
using Serilog;

namespace GridDomain.Tests.XUnit.Sagas.SagaActorTests
{
    public class AsyncLongRunningSagaFactory :
        ISagaFactory<ISaga<TestState>, TestState>,
        ISagaFactory<ISaga<TestState>, SampleAggregateCreatedEvent>
    {
        private readonly ILogger _log;

        public AsyncLongRunningSagaFactory(ILogger log)
        {
            _log = log;
        }

        public ISaga<TestState> Create(TestState message)
        {
            return new Saga<TestState>(new AsyncLongRunningSaga(), message, _log);
        }

        public ISaga<TestState> Create(SampleAggregateCreatedEvent message)
        {
            return Create(new TestState(message.SagaId, nameof(AsyncLongRunningSaga.Initial)));
        }
    }
}