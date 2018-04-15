using System;
using System.Threading.Tasks;
using Automatonymous;
using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.Node.AkkaMessaging.Waiting;
using GridDomain.ProcessManagers;
using GridDomain.ProcessManagers.State;
using GridDomain.Tests.Common;
using GridDomain.Tests.Unit.ProcessManagers.SoftwareProgrammingDomain;
using GridDomain.Tests.Unit.ProcessManagers.SoftwareProgrammingDomain.Events;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.Unit.ProcessManagers
{
    public class GivenInstanceProcessWhenExceptionOnTransit : NodeTestKit
    {
        public GivenInstanceProcessWhenExceptionOnTransit(ITestOutputHelper helper) : this(new SoftwareProgrammingProcessManagerFixture(helper)) {}
        protected GivenInstanceProcessWhenExceptionOnTransit(NodeTestFixture fixture) : base(fixture) {}

        [Fact]
        public async Task When_process_receives_a_message_that_case_process_exception()
        {
            var processId = Guid.NewGuid().ToString();
            //prepare initial process state
            var processState = new SoftwareProgrammingState(processId, nameof(SoftwareProgrammingProcess.MakingCoffee))
                           {
                               PersonId = Guid.NewGuid().ToString()
                           };

            var procesStateEvent = new ProcessManagerCreated<SoftwareProgrammingState>(processState, processId);
            await Node.SaveToJournal<ProcessStateAggregate<SoftwareProgrammingState>>(processId, procesStateEvent);

            var results = await Node.NewLocalDebugWaiter()
                                    .Expect<Fault<CoffeMakeFailedEvent>>()
                                    .Create()
                                    .SendToProcessManagers(new CoffeMakeFailedEvent(null, processState.PersonId), processId);

            var fault = results.Message<IFault<CoffeMakeFailedEvent>>();
            //Fault_should_be_produced_and_published()
            Assert.NotNull(fault);
            //Fault_exception_should_contains_stack_trace()
            var exception = fault.Exception.UnwrapSingle();
            Assert.IsAssignableFrom<ProcessTransitionException>(exception);
            //Fault_should_have_process_as_producer()
            Assert.Equal(typeof(SoftwareProgrammingProcess), fault.Processor);

            Assert.True(exception.StackTrace == null || exception.StackTrace.Contains("Process"));

            Assert.IsAssignableFrom<UndefinedCoffeMachineException>(exception.InnerException);
        }
    }
}