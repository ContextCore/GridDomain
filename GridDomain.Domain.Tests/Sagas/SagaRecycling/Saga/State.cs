using System;
using GridDomain.EventSourcing.Sagas;
using GridDomain.EventSourcing.Sagas.StateSagas;

namespace GridDomain.Tests.Sagas.SagaRecycling.Saga
{
    public class State : SagaStateAggregate<States, States>
    {
        private State(Guid id) : base(id)
        {
        }

        public State(Guid id, States state) : base(id, state)
        {
        }

        public void Finish()
        {
            RaiseEvent(new FinishedEvent(Id));
        }

        public void Apply(FinishedEvent evt)
        {

        }
    }
}