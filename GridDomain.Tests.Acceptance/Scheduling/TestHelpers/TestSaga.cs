using System;
using Automatonymous;
using GridDomain.EventSourcing.Sagas;
using GridDomain.EventSourcing.Sagas.InstanceSagas;

namespace GridDomain.Tests.Acceptance.Scheduling.TestHelpers
{
    public class TestSaga : Saga<TestSagaState>
    {
        public TestSaga()
        {
            During(Initial,
                When(Start)
                    .TransitionTo(Started));

            During(Started,
                When(Process)
                    .Then(ctx => ResultHolder.Add("123", "123"))
                    .Finalize());
        }

        public Event<TestSagaStartMessage> Start { get; private set; }
        public Event<TestEvent> Process { get; private set; }

        public State Started { get; set; }

        public static ISagaDescriptor Descriptor
        {
            get
            {
                var descriptor = SagaDescriptor.CreateDescriptor<TestSaga, TestSagaState>();
                descriptor.AddStartMessage<TestSagaState>();
                return descriptor;
            }
        }
    }
}