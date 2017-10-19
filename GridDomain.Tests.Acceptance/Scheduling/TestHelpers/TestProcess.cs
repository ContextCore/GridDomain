using System.Collections.Generic;
using System.Threading.Tasks;
using Automatonymous;
using GridDomain.CQRS;
using GridDomain.ProcessManagers;
using GridDomain.ProcessManagers.DomainBind;

namespace GridDomain.Tests.Acceptance.Scheduling.TestHelpers
{
    public class TestProcess : Process<TestProcessState>
    {
        static TestProcess()
        {
            var descriptor = ProcessDescriptor.CreateDescriptor<TestProcess, TestProcessState>();
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
        public static IProcessDescriptor Descriptor { get; }
        public override Task<IReadOnlyCollection<ICommand>> Transit(TestProcessState state, object message)
        {
            switch (message)
            {
                case TestProcessStartMessage e: return TransitMessage(Start, e, state);   
                case TestEvent e: return TransitMessage(Process, e, state);   
            }
            throw new UnbindedMessageReceivedException(message);
        }
    }
}