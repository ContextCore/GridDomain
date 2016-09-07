using System;
using GridDomain.EventSourcing.Sagas;
using GridDomain.EventSourcing.Sagas.StateSagas;

namespace GridDomain.Tests.Sagas.SagaRecycling.Saga
{
    public class SagaForRecycling : StateSaga<States,Triggers, State, StartEvent>
    {
        public SagaForRecycling(State state) : base(state)
        {
            var finished = RegisterEvent<FinishedEvent>(Triggers.Finish);
            Machine
                .Configure(States.Created)
                .OnEntryFrom(finished, e =>
                {
                    int a = 1;
                })
                .Permit(Triggers.Finish, States.Finished);

            var started = RegisterEvent<StartEvent>(Triggers.Start);

            Machine
                .Configure(States.Finished)
                .OnEntryFrom(started, e =>
                {
                    State.Finish();
                }).Permit(Triggers.Start, States.Created);

        }

        public static ISagaDescriptor Descriptor = new SagaForRecycling(new State(Guid.Empty, States.Created));
    }
}
