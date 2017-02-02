using System;
using System.Threading.Tasks;
using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.EventSourcing.Sagas;
using GridDomain.EventSourcing.Sagas.InstanceSagas;
using GridDomain.Node.AkkaMessaging.Waiting;
using GridDomain.Tests.Framework;
using GridDomain.Tests.XUnit.Sagas.SoftwareProgrammingDomain.Events;
using Xunit;

namespace GridDomain.Tests.XUnit.Sagas.InstanceSagas
{
   
    public class Given_instance_saga_When_exception_on_transit : SoftwareProgrammingInstanceSagaTest
    {

        [Fact]
        public async Task When_saga_receives_a_message_that_case_saga_exception()
        {
            var sagaId = Guid.NewGuid();
            //prepare initial saga state
            var sagaData = new SoftwareProgrammingSagaData(sagaId,nameof(SoftwareProgrammingSaga.MakingCoffee))
            {
                PersonId = Guid.NewGuid()
            };
            var sagaDataEvent = new SagaCreatedEvent<SoftwareProgrammingSagaData>(sagaData, sagaId);
            await SaveToJournal<SagaStateAggregate<SoftwareProgrammingSagaData>>(sagaId, sagaDataEvent);

            var results = await GridNode.NewDebugWaiter()
                                        .Expect<IFault<CoffeMakeFailedEvent>>()
                                        .Create()
                                        .SendToSagas(new CoffeMakeFailedEvent(Guid.Empty, sagaData.PersonId), sagaId);

            var fault = results.Message<IFault<CoffeMakeFailedEvent>>();
        // Fault_should_be_produced_and_published()
             Assert.NotNull(fault);
       //Fault_exception_should_contains_stack_trace()
            Assert.True(fault.Exception.UnwrapSingle().StackTrace.Contains(typeof(SoftwareProgrammingSaga).Name));
        // Fault_should_have_saga_as_producer()
            Assert.Equal(typeof(SoftwareProgrammingSaga), fault.Processor);
        //Fault_should_contains_exception_from_saga()
            Assert.IsAssignableFrom<UndefinedCoffeMachineException>(fault.Exception.UnwrapSingle());
        }
    }
}