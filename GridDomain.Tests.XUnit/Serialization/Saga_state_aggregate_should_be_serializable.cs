using System;
using Automatonymous;
using GridDomain.EventSourcing.Sagas.InstanceSagas;
using GridDomain.Tests.Framework;
using GridDomain.Tests.XUnit.Sagas.SoftwareProgrammingDomain;
using Newtonsoft.Json;
using Xunit;

namespace GridDomain.Tests.XUnit.Serialization
{
    public class Saga_state_aggregate_should_be_serializable
    {
        [Fact]
        public void Test()
        {
            var saga = new SoftwareProgrammingSaga();
            var state = new SoftwareProgrammingSagaState(Guid.NewGuid(), "123", Guid.NewGuid(), Guid.NewGuid());

            var sagaState = new SagaStateAggregate<SoftwareProgrammingSagaState>(state);
            Event @event = saga.CoffeReady;
            sagaState.RememberEvent(state, typeof(Object), @event.Name);
            sagaState.ClearEvents();

            var json = JsonConvert.SerializeObject(sagaState);
            var restoredState = JsonConvert.DeserializeObject<SagaStateAggregate<SoftwareProgrammingSagaState>>(json);

            //CoffeMachineId_should_be_equal()
            Assert.Equal(sagaState.Data.CoffeeMachineId, restoredState.Data.CoffeeMachineId);
            // Id_should_be_equal()
            Assert.Equal(sagaState.Id, restoredState.Id);
            //State_should_be_equal()
            Assert.Equal(sagaState.Data.CurrentStateName, restoredState.Data.CurrentStateName);
        }
    }
}