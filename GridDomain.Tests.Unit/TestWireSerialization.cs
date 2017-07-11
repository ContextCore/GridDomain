using System;
using Akka.Actor;
using Akka.Serialization;
using Akka.TestKit.Xunit2;
using Automatonymous;
using GridDomain.EventSourcing.Sagas.InstanceSagas;
using GridDomain.Tests.Unit.Sagas.SoftwareProgrammingDomain;
using GridDomain.Tests.Unit.Sagas.SoftwareProgrammingDomain.Events;
using Xunit;

namespace GridDomain.Tests.Unit {
    public class TestWireSerialization : TestKit
    {
        [Fact]
        public void Test_deserialization()
        {
            //CoffeMakeFailedEvent
            var msg = new Akka.Actor.Status.Failure(new UndefinedCoffeMachineException());
            //var msg = 
            var serializer = new WireSerializer((ExtendedActorSystem) Sys);
            var bytes = serializer.ToBinary(msg);
            var restored = serializer.FromBinary(bytes, msg.GetType());
            Assert.NotNull(restored);
        }
        
        [Fact(Skip = "all exception should properly support ISerialization interface, will be done later")]
        public void Saga_Faults_Should_be_deserializable()
        {
            //CoffeMakeFailedEvent
            var coffeMakeFailedEvent = new CoffeMakeFailedEvent(Guid.NewGuid(), Guid.NewGuid());
            CheckDeserialize(coffeMakeFailedEvent, nameof(coffeMakeFailedEvent));

            var undefinedCoffeMachineException = new UndefinedCoffeMachineException();
            CheckDeserialize(undefinedCoffeMachineException, nameof(undefinedCoffeMachineException));

            var sagaTransitionException = new SagaTransitionException(coffeMakeFailedEvent, undefinedCoffeMachineException);

            CheckDeserialize(sagaTransitionException, nameof(sagaTransitionException));

            var eventExecutionException = new EventExecutionException("test",sagaTransitionException);
            CheckDeserialize(eventExecutionException, nameof(eventExecutionException));

            var msg = new Akka.Actor.Status.Failure(new AggregateException(eventExecutionException));
            CheckDeserialize(msg,nameof(msg));
        }

        private void CheckDeserialize(object msg, string message)
        {
            Log.Info("testing " + message);
            var serializer = new WireSerializer((ExtendedActorSystem) Sys);
            var bytes = serializer.ToBinary(msg);
            var restored = serializer.FromBinary(bytes, msg.GetType());
            Assert.NotNull(restored);
        }
    }
}