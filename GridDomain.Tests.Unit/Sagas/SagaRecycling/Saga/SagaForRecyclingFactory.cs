using System;
using GridDomain.EventSourcing.Sagas;

namespace GridDomain.Tests.Unit.Sagas.SagaRecycling.Saga
{
    public class SagaForRecyclingFactory :
        ISagaFactory<SagaForRecycling, StartEvent>,
        ISagaFactory<SagaForRecycling, State>,
        ISagaFactory<SagaForRecycling, Guid>
    {
        public SagaForRecycling Create(StartEvent message)
        {
            return new SagaForRecycling(new State(message.SagaId, States.Created));
        }

        public SagaForRecycling Create(State state)
        {
            return new SagaForRecycling(state);
        }

        public SagaForRecycling Create(Guid id)
        {
            return new SagaForRecycling(new State(id, States.Created));
        }
    }
}