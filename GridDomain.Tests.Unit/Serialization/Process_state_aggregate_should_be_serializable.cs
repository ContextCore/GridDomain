using System;
using System.Reflection;
using GridDomain.EventSourcing;
using GridDomain.ProcessManagers.State;
using GridDomain.Tests.Unit.ProcessManagers.SoftwareProgrammingDomain;
using GridDomain.Tools;
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
            processStateAggregate.MarkAllPesisted();

            var json = JsonConvert.SerializeObject(processStateAggregate);
            var restoredState = JsonConvert.DeserializeObject<ProcessStateAggregate<SoftwareProgrammingState>>(json);
            restoredState.MarkAllPesisted();

            //CoffeMachineId_should_be_equal()
            Assert.Equal(processStateAggregate.State.CoffeeMachineId, restoredState.State.CoffeeMachineId);
            // Id_should_be_equal()
            Assert.Equal(processStateAggregate.Id, restoredState.Id);
            //State_should_be_equal()
            Assert.Equal(processStateAggregate.State.CurrentStateName, restoredState.State.CurrentStateName);
        }
    }
}