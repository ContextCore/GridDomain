using System;
using GridDomain.CQRS;
using GridDomain.EventSourcing.Sagas;
using SchedulerDemo.PhoneCallDomain.Commands;
using SchedulerDemo.PhoneCallDomain.Events;

namespace SchedulerDemo.PhoneCallDomain.Sagas.PhoneCall
{
    public class PhoneCallSaga : StateSaga<PhoneCallSaga.States, PhoneCallSaga.Transitions, PhoneCallSagaStateAggregate, CallInitiatedEvent>,
        IHandler<AbonentAnsweredEvent>,
        IHandler<AbonentHungUpEvent>,
        IHandler<AbonentIsBusyEvent>,
        IHandler<AbonentReceivingIncomingCallEvent>
    {
        public enum States
        {
            CallInitiated,
            Dialing,
            InConversation,
            CallBeingTerminated
        }

        public enum Transitions
        {
            InitiateCall,
            WaitForAnswer,
            AbonentResponded,
            AbonentisBusy,
            TerminateCall
        }

        public PhoneCallSaga(PhoneCallSagaStateAggregate stateAggregateData) : base(stateAggregateData)
        {
            var phoneInitTransition = RegisterEvent<CallInitiatedEvent>(Transitions.InitiateCall);
            Machine.Configure(States.CallInitiated)
                .Permit(Transitions.WaitForAnswer, States.Dialing)
                .Permit(Transitions.AbonentisBusy, States.CallBeingTerminated);

            Machine.Configure(States.Dialing)
                .Permit(Transitions.AbonentResponded, States.InConversation)
                .Permit(Transitions.TerminateCall, States.CallBeingTerminated);

            Machine.Configure(States.InConversation)
                .Permit(Transitions.TerminateCall, States.CallBeingTerminated);

            Machine.Configure(States.CallBeingTerminated)
                .OnEntry(e=> Dispatch(new HangupCommand(State.LastActiveMember.Value)));

        }

        public static ISagaDescriptor SagaDescriptor => new PhoneCallSaga(new PhoneCallSagaStateAggregate(Guid.Empty, States.CallBeingTerminated));

        public void Handle(AbonentAnsweredEvent msg)
        {
            Transit(msg);
        }

        public void Handle(AbonentHungUpEvent msg)
        {
            Transit(msg);
        }

        public void Handle(AbonentIsBusyEvent msg)
        {
            Transit(msg);
        }

        public void Handle(AbonentReceivingIncomingCallEvent msg)
        {
            Transit(msg);
        }
    }
}
