using System;
using GridDomain.EventSourcing.Sagas;

namespace GridDomain.Tests.Sagas.SagaRecycling.Saga
{
    public class SagaForRecyclingFactory :
        ISagaFactory<SagaForRecycling, StartEvent>,
        ISagaFactory<SagaForRecycling, State>,
        IEmptySagaFactory<SagaForRecycling>
    {
        public SagaForRecycling Create(StartEvent message)
        {
            return new SagaForRecycling(new State(message.SagaId, States.Created));
        }

        public SagaForRecycling Create(State state)
        {
            return new SagaForRecycling(state);
        }

        public SagaForRecycling Create()
        {
            return new SagaForRecycling(new State(Guid.Empty, States.Created));
        }
    }
}