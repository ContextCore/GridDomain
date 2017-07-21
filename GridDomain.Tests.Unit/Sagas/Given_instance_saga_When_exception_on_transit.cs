using System;
using System.Threading.Tasks;
using Automatonymous;
using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.Node.AkkaMessaging.Waiting;
using GridDomain.Processes;
using GridDomain.Processes.State;
using GridDomain.Tests.Common;
using GridDomain.Tests.Unit.Sagas.SoftwareProgrammingDomain;
using GridDomain.Tests.Unit.Sagas.SoftwareProgrammingDomain.Events;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.Unit.Sagas
{
    public class Given_instance_saga_When_exception_on_transit : SoftwareProgrammingSagaTest
    {
        public Given_instance_saga_When_exception_on_transit(ITestOutputHelper helper) : base(helper) {}

        [Fact]
        public async Task When_saga_receives_a_message_that_case_saga_exception()
        {
            var sagaId = Guid.NewGuid();
            //prepare initial saga state
            var sagaData = new SoftwareProgrammingState(sagaId, nameof(SoftwareProgrammingProcess.MakingCoffee))
                           {
                               PersonId = Guid.NewGuid()
                           };

            var sagaDataEvent = new SagaCreated<SoftwareProgrammingState>(sagaData, sagaId);
            await Node.SaveToJournal<ProcessStateAggregate<SoftwareProgrammingState>>(sagaId, sagaDataEvent);

            var results = await Node.NewDebugWaiter()
                                    .Expect<Fault<CoffeMakeFailedEvent>>()
                                    .Create()
                                    .SendToSagas(new CoffeMakeFailedEvent(Guid.Empty, sagaData.PersonId), sagaId);

            var fault = results.Message<IFault<CoffeMakeFailedEvent>>();
            //Fault_should_be_produced_and_published()
            Assert.NotNull(fault);
            //Fault_exception_should_contains_stack_trace()
            var exception = fault.Exception.UnwrapSingle();
            Assert.IsAssignableFrom<ProcessTransitionException>(exception);
            //Fault_should_have_saga_as_producer()
            Assert.Equal(typeof(ProcessManager<SoftwareProgrammingState>), fault.Processor);
            Assert.True(exception.StackTrace.Contains("Saga"));
            //Fault_should_contains_exception_from_saga()
            var innerException = exception.InnerException.UnwrapSingle();
            Assert.IsAssignableFrom<EventExecutionException>(innerException);

            var innerInnerException = innerException.InnerException.UnwrapSingle();
            Assert.IsAssignableFrom<UndefinedCoffeMachineException>(innerInnerException);
        }
    }
}