using System;
using GridDomain.EventSourcing.Sagas.InstanceSagas;
using GridDomain.Tests.Framework;
using GridDomain.Tests.Unit.Sagas.InstanceSagas;
using Newtonsoft.Json;
using NUnit.Framework;

namespace GridDomain.Tests.Unit.Serialization
{
    [TestFixture]
    class Instance_sagas_state_aggregates_should_be_serialize_and_deserialize : SoftwareProgrammingInstanceSagaTest
    {
        private SagaStateAggregate<SoftwareProgrammingSagaData> _sagaState;
        private SagaStateAggregate<SoftwareProgrammingSagaData> _restoredState;
        public Instance_sagas_state_aggregates_should_be_serialize_and_deserialize() : base(true) { }

        [OneTimeSetUp]
        public void Test()
        {
            var saga  = new SoftwareProgrammingSaga();
            var state = new SoftwareProgrammingSagaData(Guid.NewGuid(),"123", Guid.NewGuid(), Guid.NewGuid());

            _sagaState = new SagaStateAggregate<SoftwareProgrammingSagaData>(state);
            _sagaState.RememberEvent(saga.CoffeReady, state, new object());
            _sagaState.ClearEvents();

            var json = JsonConvert.SerializeObject(_sagaState);
            _restoredState = JsonConvert.DeserializeObject<SagaStateAggregate<SoftwareProgrammingSagaData>>(json);
        }

        [Test]
        public void CoffeMachineId_should_be_equal()
        {
            Assert.AreEqual(_sagaState.Data.CoffeeMachineId, _restoredState.Data.CoffeeMachineId);
        }

        [Test]
        public void Id_should_be_equal()
        {
            Assert.AreEqual(_sagaState.Id, _restoredState.Id);
        }

        [Test]
        public void State_should_be_equal()
        {
            Assert.AreEqual(_sagaState.Data.CurrentStateName, _restoredState.Data.CurrentStateName);
        }
    }
}