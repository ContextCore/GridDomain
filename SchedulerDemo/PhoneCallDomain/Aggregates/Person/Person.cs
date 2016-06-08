using System;
using CommonDomain.Core;

namespace SchedulerDemo.PhoneCallDomain.Aggregates.Person
{
    public class Person : AggregateBase
    {
        public enum State
        {
            Available,
            OutgoingCallInitiated,
            ReceivingIncomingCall,
            WaitingForAnswer,
            InConversation,
            WaitingForCallTermination
        }
        public State CurrentState { get; }

        public Person(Guid id, State currentState)
        {
            Id = id;
            CurrentState = currentState;
        }
    }
}