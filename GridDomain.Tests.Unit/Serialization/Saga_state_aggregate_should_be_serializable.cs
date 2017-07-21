using System;
using GridDomain.EventSourcing;
using GridDomain.Processes.State;
using GridDomain.Tests.Unit.Sagas.SoftwareProgrammingDomain;
using Newtonsoft.Json;
using Xunit;

namespace GridDomain.Tests.Unit.Serialization
{
    public class Saga_state_aggregate_should_be_serializable
    {
        [Fact]
        public void Test()
        {
            var state = new SoftwareProgrammingState(Guid.NewGuid(), "123", Guid.NewGuid(), Guid.NewGuid());

            var sagaState = new ProcessStateAggregate<SoftwareProgrammingState>(state);
            sagaState.ReceiveMessage(state, typeof(Object));
            sagaState.PersistAll();

            var json = JsonConvert.SerializeObject(sagaState);
            var restoredState = JsonConvert.DeserializeObject<ProcessStateAggregate<SoftwareProgrammingState>>(json);
            restoredState.PersistAll();

            //CoffeMachineId_should_be_equal()
            Assert.Equal(sagaState.State.CoffeeMachineId, restoredState.State.CoffeeMachineId);
            // Id_should_be_equal()
            Assert.Equal(sagaState.Id, restoredState.Id);
            //State_should_be_equal()
            Assert.Equal(sagaState.State.CurrentStateName, restoredState.State.CurrentStateName);
        }
    }
}