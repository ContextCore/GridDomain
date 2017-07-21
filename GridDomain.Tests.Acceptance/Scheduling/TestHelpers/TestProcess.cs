using Automatonymous;
using GridDomain.ProcessManagers;
using GridDomain.ProcessManagers.DomainBind;

namespace GridDomain.Tests.Acceptance.Scheduling.TestHelpers
{
    public class TestProcess : Process<TestProcessState>
    {
        static TestProcess()
        {
            var descriptor = ProcessManagerDescriptor.CreateDescriptor<TestProcess, TestProcessState>();
            descriptor.AddStartMessage<TestProcessStartMessage>();
            descriptor.AddAcceptedMessage(typeof(TestEvent));
            Descriptor = descriptor;
        }

        public TestProcess()
        {
            During(Initial, When(Start).TransitionTo(Started));

            During(Started, When(Process).Then(ctx => ResultHolder.Add("123", "123")).Finalize());
        }

        public Event<TestProcessStartMessage> Start { get; private set; }
        public Event<TestEvent> Process { get; private set; }

        public State Started { get; set; }
        public static IProcessManagerDescriptor Descriptor { get; }
    }
}