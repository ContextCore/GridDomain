using Automatonymous;
using GridDomain.EventSourcing.Sagas;
using GridDomain.EventSourcing.Sagas.InstanceSagas;

namespace GridDomain.Tests.Acceptance.XUnit.Scheduling.TestHelpers
{
    public class TestSaga : Saga<TestSagaState>
    {
        static TestSaga()
        {
            var descriptor = SagaDescriptor.CreateDescriptor<TestSaga, TestSagaState>();
            descriptor.AddStartMessage<TestSagaStartMessage>();
            descriptor.AddAcceptedMessage(typeof(TestEvent));
            Descriptor = descriptor;
        }

        public TestSaga()
        {
            During(Initial, When(Start).TransitionTo(Started));

            During(Started, When(Process).Then(ctx => ResultHolder.Add("123", "123")).Finalize());
        }

        public Event<TestSagaStartMessage> Start { get; private set; }
        public Event<TestEvent> Process { get; private set; }

        public State Started { get; set; }
        public static ISagaDescriptor Descriptor { get; }
    }
}