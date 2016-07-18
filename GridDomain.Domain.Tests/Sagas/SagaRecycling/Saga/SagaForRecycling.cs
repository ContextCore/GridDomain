using System;
using GridDomain.EventSourcing.Sagas;

namespace GridDomain.Tests.Sagas.SagaRecycling.Saga
{
    public class SagaForRecycling : StateSaga<States, States, State, StartEvent>
    {
        public SagaForRecycling(State state) : base(state)
        {
            var finished = RegisterEvent<FinishedEvent>(States.Created);
            Machine
                .Configure(States.Created)
                .OnEntryFrom(finished, e =>
                {
                    int a = 1;
                })
                .Permit(States.Finished, States.Finished);

            var started = RegisterEvent<StartEvent>(States.Finished);

            Machine
                .Configure(States.Finished)
                .OnEntryFrom(started, e =>
                {
                    State.Finish();
                }).Permit(States.Created, States.Created);

        }

        public static ISagaDescriptor Descriptor = new SagaForRecycling(new State(Guid.Empty, States.Created));
    }
}
