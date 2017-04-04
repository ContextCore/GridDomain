using System;
using GridDomain.EventSourcing.Sagas;
using GridDomain.EventSourcing.Sagas.InstanceSagas;
using GridDomain.Tests.XUnit.SampleDomain.Events;
using Serilog;

namespace GridDomain.Tests.XUnit.Sagas.SagaActorTests
{
    public class AsyncLongRunningSagaFactory : ISagaCreator<TestState>,
                                               ISagaCreator<TestState, SampleAggregateCreatedEvent>
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

        public ISaga<TestState> CreateNew(SampleAggregateCreatedEvent message, Guid? id = null)
        {
            return Create(new TestState(id ?? message.SagaId, nameof(AsyncLongRunningSaga.Initial)));
        }
    }
}