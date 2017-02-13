using System;
using GridDomain.EventSourcing.Sagas.InstanceSagas;
using GridDomain.Tests.Framework;
using GridDomain.Tests.XUnit.Sagas.SoftwareProgrammingDomain;
using GridDomain.Tests.XUnit.Sagas.SoftwareProgrammingDomain.Events;
using Newtonsoft.Json;
using Ploeh.AutoFixture;
using Xunit;

namespace GridDomain.Tests.XUnit.Serialization
{
    public class Saga_state_aggregate_should_be_serializable
    {
        private SagaStateAggregate<SoftwareProgrammingSagaData> _sagaState;
        private SagaStateAggregate<SoftwareProgrammingSagaData> _restoredState;

        [Fact]
        public void Test()
        {
            var saga  = new SoftwareProgrammingSaga();
            var state = new SoftwareProgrammingSagaData(Guid.NewGuid(),"123", Guid.NewGuid(), Guid.NewGuid());

            _sagaState = new SagaStateAggregate<SoftwareProgrammingSagaData>(state);
            _sagaState.RememberEvent(saga.CoffeReady, state, new object());
            _sagaState.ClearEvents();

            //var fixture = new Fixture();
            //var gotTired = fixture.Create<GotTiredEvent>();

            //var factory = new SoftwareProgrammingSagaFactory();
            //var instance = factory.Create(gotTired); 

            var json = JsonConvert.SerializeObject(_sagaState);
            _restoredState = JsonConvert.DeserializeObject<SagaStateAggregate<SoftwareProgrammingSagaData>>(json);

        //CoffeMachineId_should_be_equal()
            Assert.Equal(_sagaState.Data.CoffeeMachineId, _restoredState.Data.CoffeeMachineId);
      // Id_should_be_equal()
            Assert.Equal(_sagaState.Id, _restoredState.Id);
       //State_should_be_equal()
            Assert.Equal(_sagaState.Data.CurrentStateName, _restoredState.Data.CurrentStateName);
        }
    }
}