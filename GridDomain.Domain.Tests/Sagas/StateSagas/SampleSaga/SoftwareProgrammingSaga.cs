using System;
using GridDomain.CQRS;
using GridDomain.EventSourcing.Sagas;
using GridDomain.Tests.Sagas.SoftwareProgrammingDomain.Commands;
using GridDomain.Tests.Sagas.SoftwareProgrammingDomain.Events;

namespace GridDomain.Tests.Sagas.StateSagas.SampleSaga
{
    public class SoftwareProgrammingSaga :
        StateSaga<SoftwareProgrammingSaga.States, SoftwareProgrammingSaga.Triggers, SoftwareProgrammingSagaState, GotTiredEvent>,
        IHandler<SleptWellEvent>,
        IHandler<CoffeMakeFailedEvent>,
        IHandler<CoffeMadeEvent>
    {

        public static ISagaDescriptor Descriptor = new SoftwareProgrammingSaga(new SoftwareProgrammingSagaState(Guid.Empty,States.Working));
        public SoftwareProgrammingSaga(SoftwareProgrammingSagaState state) : base(state)
        {
            var gotTiredTriggerTrigger = RegisterEvent<GotTiredEvent>(Triggers.GoForCoffe);
            var coffeMadeTrigger = RegisterEvent<CoffeMadeEvent>(Triggers.FeelWell);
            var sleptWellTrigger = RegisterEvent<SleptWellEvent>(Triggers.SleepAnough);
            var coffeMakeFailedTrigger = RegisterEvent<CoffeMakeFailedEvent>(Triggers.GoToSleep);

            //TODO: refactor this ugly hack! 
            RegisterEvent<BadCoffeMachineRememberedEvent>(Triggers.DummyForSagaStateChange);

            Machine.Configure(States.Working)
                   .Permit(Triggers.GoForCoffe, States.MakingCoffe);

            Machine.Configure(States.MakingCoffe)
                .OnEntryFrom(gotTiredTriggerTrigger, e =>
                {
                    State.RememberPerson(e.PersonId);
                    Dispatch(new MakeCoffeCommand(e.PersonId, State.CoffeMachineId));
                })
                .Permit(Triggers.FeelWell, States.Working)
                .Permit(Triggers.GoToSleep, States.Sleeping);

            Machine.Configure(States.Sleeping)
                .OnEntryFrom(coffeMakeFailedTrigger,
                    e => {
                        State.RememberBadCoffeMachine(e.CoffeMachineId);
                        Dispatch(new GoToWorkCommand(e.ForPersonId));
                    })
                .Permit(Triggers.SleepAnough, States.Working);
        }

        public void Handle(GotTiredEvent e)
        {
            TransitState(e);
        }

        public void Handle(CoffeMakeFailedEvent msg)
        {
            TransitState(msg);
        }

        public void Handle(SleptWellEvent msg)
        {
            TransitState(msg);
        }

        public void Handle(CoffeMadeEvent msg)
        {
            TransitState(msg);
        }

        public enum Triggers
        {
            GoForCoffe,
            FeelWell,
            SleepAnough,
            GoToSleep,
            DummyForSagaStateChange
        }

        public enum States
        {
            Working,
            MakingCoffe,
            Sleeping
        }
    }
}