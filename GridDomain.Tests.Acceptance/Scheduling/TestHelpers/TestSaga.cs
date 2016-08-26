using System;
using GridDomain.EventSourcing.Sagas;
using GridDomain.EventSourcing.Sagas.StateSagas;

namespace GridDomain.Tests.Acceptance.Scheduling.TestHelpers
{
    public class TestSaga : StateSaga<TestSaga.TestStates, TestSaga.Transitions, TestSagaState, TestSagaStartMessage>
    {
        public enum TestStates
        {
            Created,
            GotStartEvent,
            GotSecondEvent
        }

        public enum Transitions
        {
            StartEventArrived,
            SecondEventArrived
        }


        public TestSaga(TestSagaState state) : base(state)
        {
            RegisterEvent<TestSagaStartMessage>(Transitions.StartEventArrived);
            RegisterEvent<TestEvent>(Transitions.SecondEventArrived);

            Machine
                .Configure(TestStates.Created)
                .Permit(Transitions.StartEventArrived, TestStates.GotStartEvent);

            Machine.Configure(TestStates.GotStartEvent)
                .OnExit(() =>
                {
                    ResultHolder.Add("123","123");    
                })
                .Permit(Transitions.SecondEventArrived, TestStates.GotSecondEvent);
        }

        public static readonly ISagaDescriptor SagaDescriptor = new TestSaga(new TestSagaState(Guid.Empty, TestStates.Created));
    }
}