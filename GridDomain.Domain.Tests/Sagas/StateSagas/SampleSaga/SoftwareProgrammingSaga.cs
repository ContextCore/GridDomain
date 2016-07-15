using System;
using GridDomain.CQRS;
using GridDomain.EventSourcing.Sagas;
using GridDomain.EventSourcing.Sagas.StateSagas;
using GridDomain.Tests.Sagas.StateSagas.SampleSaga.Commands;
using GridDomain.Tests.Sagas.StateSagas.SampleSaga.Events;

namespace GridDomain.Tests.Sagas.StateSagas.SampleSaga
{
    public class SoftwareProgrammingSaga :
        StateSaga<SoftwareProgrammingSaga.States, SoftwareProgrammingSaga.Triggers, SoftwareProgrammingSagaState, GotTiredEvent>,
        IHandler<SleptWellEvent>,
        IHandler<GotMoreTiredEvent>,
        IHandler<FeltGoodEvent>
    {

        public static ISagaDescriptor Descriptor = new SoftwareProgrammingSaga(new SoftwareProgrammingSagaState(Guid.Empty,States.Working));
        public SoftwareProgrammingSaga(SoftwareProgrammingSagaState state) : base(state)
        {
            var parForSubscriptionTrigger = RegisterEvent<GotTiredEvent>(Triggers.GoForCoffe);
            var remainSubscriptionTrigger = RegisterEvent<FeltGoodEvent>(Triggers.FeelWell);
            var changeSubscriptionTrigger = RegisterEvent<SleptWellEvent>(Triggers.SleepAnough);
            var revokeSubscriptionTrigger = RegisterEvent<GotMoreTiredEvent>(Triggers.GoToSleep);

            Machine.Configure(States.Working)
                .Permit(Triggers.GoForCoffe, States.DrinkingCoffe);

            Machine.Configure(States.DrinkingCoffe)
                .OnEntryFrom(parForSubscriptionTrigger, e => Dispatch(new DrinkCupOfCoffeCommand(e)))
                .Permit(Triggers.FeelWell, States.Working)
                .Permit(Triggers.GoToSleep, States.Sleeping);

            Machine.Configure(States.Sleeping)
                .OnEntryFrom(revokeSubscriptionTrigger, e => Dispatch(new SleepWellCommand(e)))
                .Permit(Triggers.SleepAnough, States.Working);
        }

        public void Handle(GotTiredEvent e)
        {
            TransitState(e);
        }

        public void Handle(GotMoreTiredEvent msg)
        {
            TransitState(msg);
        }

        public void Handle(SleptWellEvent msg)
        {
            TransitState(msg);
        }

        public void Handle(FeltGoodEvent msg)
        {
            TransitState(msg);
        }

        public enum Triggers
        {
            GoForCoffe,
            FeelWell,
            SleepAnough,
            GoToSleep
        }

        public enum States
        {
            Working,
            DrinkingCoffe,
            Sleeping
        }
    }
}