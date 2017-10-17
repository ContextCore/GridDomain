using System;
using System.Reflection;
using Common.Logging;
using GridDomain.EventSourcing;
using GridDomain.Node.Serializers;
using GridDomain.ProcessManagers;
using GridDomain.ProcessManagers.State;
using GridDomain.Tests.Unit.ProcessManagers.SoftwareProgrammingDomain;
using GridDomain.Tools;
using Newtonsoft.Json;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.Unit.Serialization
{
    public class Process_state_aggregate_should_be_serializable
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public Process_state_aggregate_should_be_serializable(ITestOutputHelper helper)
        {
            _testOutputHelper = helper;
        }

        [Fact]
        public void Test()
        {
            var state = new SoftwareProgrammingState(Guid.NewGuid(), "123", Guid.NewGuid(), Guid.NewGuid());

            var processStateAggregate = new ProcessStateAggregate<SoftwareProgrammingState>(state);
            processStateAggregate.ReceiveMessage(state, typeof(Object));
            processStateAggregate.CommitAll();


            var jsonSerializerSettings = DomainSerializer.GetDefaultSettings();
            jsonSerializerSettings.TraceWriter = new XUnitTraceWriter(_testOutputHelper);

            var json = JsonConvert.SerializeObject(processStateAggregate, jsonSerializerSettings);

            var restoredState = JsonConvert.DeserializeObject<ProcessStateAggregate<SoftwareProgrammingState>>(json,jsonSerializerSettings);

            restoredState.CommitAll();

            //CoffeMachineId_should_be_equal()
            Assert.Equal(processStateAggregate.State.CoffeeMachineId, restoredState.State.CoffeeMachineId);
            // Id_should_be_equal()
            Assert.Equal(processStateAggregate.Id, restoredState.Id);
            //State_should_be_equal()
            Assert.Equal(processStateAggregate.State.CurrentStateName, restoredState.State.CurrentStateName);
        }
    }
}