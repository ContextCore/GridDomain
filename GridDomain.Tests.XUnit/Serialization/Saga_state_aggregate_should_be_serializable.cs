using System;
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
            var state = new SoftwareProgrammingSagaData(Guid.NewGuid(), "123", Guid.NewGuid(), Guid.NewGuid());

            var sagaState = new SagaStateAggregate<SoftwareProgrammingSagaData>(state);
            sagaState.RememberEvent(saga.CoffeReady, state, new object());
            sagaState.ClearEvents();

            var json = JsonConvert.SerializeObject(sagaState);
            var restoredState = JsonConvert.DeserializeObject<SagaStateAggregate<SoftwareProgrammingSagaData>>(json);

            //CoffeMachineId_should_be_equal()
            Assert.Equal(sagaState.Data.CoffeeMachineId, restoredState.Data.CoffeeMachineId);
            // Id_should_be_equal()
            Assert.Equal(sagaState.Id, restoredState.Id);
            //State_should_be_equal()
            Assert.Equal(sagaState.Data.CurrentStateName, restoredState.Data.CurrentStateName);
        }
    }
}