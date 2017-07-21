using Automatonymous;
using GridDomain.Processes;
using GridDomain.Processes.DomainBind;

namespace GridDomain.Tests.Acceptance.Scheduling.TestHelpers
{
    public class TestSaga : Process<TestProcessState>
    {
        static TestSaga()
        {
            var descriptor = ProcessManagerDescriptor.CreateDescriptor<TestSaga, TestProcessState>();
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
        public static IProcessManagerDescriptor Descriptor { get; }
    }
}