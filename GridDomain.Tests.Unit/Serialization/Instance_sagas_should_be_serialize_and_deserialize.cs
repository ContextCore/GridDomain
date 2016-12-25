using System;
using GridDomain.EventSourcing.Sagas.InstanceSagas;
using GridDomain.Tests.Framework;
using GridDomain.Tests.Unit.Sagas.InstanceSagas;
using GridDomain.Tests.Unit.Sagas.SoftwareProgrammingDomain.Events;
using Newtonsoft.Json;
using NUnit.Framework;
using Ploeh.AutoFixture;

namespace GridDomain.Tests.Unit.Serialization
{
    [TestFixture]
    class Instance_sagas_should_be_serialize_and_deserialize : SoftwareProgrammingInstanceSagaTest
    {
        private SagaDataAggregate<SoftwareProgrammingSagaData> _sagaState;
        private SagaDataAggregate<SoftwareProgrammingSagaData> _restoredState;
        public Instance_sagas_should_be_serialize_and_deserialize() : base(true) { }

        [OneTimeSetUp]
        public void Test()
        {
            var saga  = new SoftwareProgrammingSaga();
            var state = new SoftwareProgrammingSagaData("123", Guid.NewGuid(), Guid.NewGuid());

            _sagaState = new SagaDataAggregate<SoftwareProgrammingSagaData>(Guid.NewGuid(), state);
            _sagaState.RememberEvent(saga.CoffeReady, state, new object());
            _sagaState.ClearEvents();

            var fixture = new Fixture();
            var gotTired = fixture.Create<GotTiredEvent>();

            var factory = new SoftwareProgrammingSagaFactory();
            var instance = factory.Create(gotTired); 


            var json = JsonConvert.SerializeObject(_sagaState);
            _restoredState = JsonConvert.DeserializeObject<SagaDataAggregate<SoftwareProgrammingSagaData>>(json);
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