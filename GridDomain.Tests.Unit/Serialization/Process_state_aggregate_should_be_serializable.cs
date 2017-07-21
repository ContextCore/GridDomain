using System;
using GridDomain.EventSourcing;
using GridDomain.Processes.State;
using GridDomain.Tests.Unit.ProcessManagers.SoftwareProgrammingDomain;
using Newtonsoft.Json;
using Xunit;

namespace GridDomain.Tests.Unit.Serialization
{
    public class Process_state_aggregate_should_be_serializable
    {
        [Fact]
        public void Test()
        {
            var state = new SoftwareProgrammingState(Guid.NewGuid(), "123", Guid.NewGuid(), Guid.NewGuid());

            var processStateAggregate = new ProcessStateAggregate<SoftwareProgrammingState>(state);
            processStateAggregate.ReceiveMessage(state, typeof(Object));
            processStateAggregate.PersistAll();

            var json = JsonConvert.SerializeObject(processStateAggregate);
            var restoredState = JsonConvert.DeserializeObject<ProcessStateAggregate<SoftwareProgrammingState>>(json);
            restoredState.PersistAll();

            //CoffeMachineId_should_be_equal()
            Assert.Equal(processStateAggregate.State.CoffeeMachineId, restoredState.State.CoffeeMachineId);
            // Id_should_be_equal()
            Assert.Equal(processStateAggregate.Id, restoredState.Id);
            //State_should_be_equal()
            Assert.Equal(processStateAggregate.State.CurrentStateName, restoredState.State.CurrentStateName);
        }
    }
}