using System;
using System.Threading.Tasks;
using Automatonymous;
using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.EventSourcing.Sagas;
using GridDomain.EventSourcing.Sagas.InstanceSagas;
using GridDomain.Node.AkkaMessaging.Waiting;
using GridDomain.Tests.Framework;
using GridDomain.Tests.XUnit.Sagas.SoftwareProgrammingDomain;
using GridDomain.Tests.XUnit.Sagas.SoftwareProgrammingDomain.Events;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.XUnit.Sagas
{
    public class Given_instance_saga_When_exception_on_transit : SoftwareProgrammingInstanceSagaTest
    {
        public Given_instance_saga_When_exception_on_transit(ITestOutputHelper helper) : base(helper) {}

        [Fact]
        public async Task When_saga_receives_a_message_that_case_saga_exception()
        {
            var sagaId = Guid.NewGuid();
            //prepare initial saga state
            var sagaData = new SoftwareProgrammingSagaData(sagaId, nameof(SoftwareProgrammingSaga.MakingCoffee))
                           {
                               PersonId
                                   =
                                   Guid
                                   .NewGuid
                                   ()
                           };

            var sagaDataEvent = new SagaCreatedEvent<SoftwareProgrammingSagaData>(sagaData, sagaId);
            await Node.SaveToJournal<SagaStateAggregate<SoftwareProgrammingSagaData>>(sagaId, sagaDataEvent);

            var results =
                await
                    Node.NewDebugWaiter(TimeSpan.FromDays(1))
                        .Expect<IFault<CoffeMakeFailedEvent>>()
                        .Create()
                        .SendToSagas(new CoffeMakeFailedEvent(Guid.Empty, sagaData.PersonId), sagaId);

            var fault = results.Message<IFault<CoffeMakeFailedEvent>>();
            //Fault_should_be_produced_and_published()
            Assert.NotNull(fault);
            //Fault_exception_should_contains_stack_trace()
            var exception = fault.Exception.UnwrapSingle();
            Assert.IsAssignableFrom<SagaTransitionException>(exception);
            //Fault_should_have_saga_as_producer()
            Assert.Equal(typeof(SoftwareProgrammingSaga), fault.Processor);
            Assert.True(exception.StackTrace.Contains("Saga"));
            //Fault_should_contains_exception_from_saga()
            Assert.IsAssignableFrom<EventExecutionException>(exception.InnerException);
            Assert.IsAssignableFrom<UndefinedCoffeMachineException>(exception.InnerException.InnerException);
        }
    }
}