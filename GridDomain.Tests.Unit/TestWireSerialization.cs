using System;
using Akka.Actor;
using Akka.Serialization;
using Akka.TestKit.Xunit2;
using Automatonymous;
using GridDomain.ProcessManagers;
using GridDomain.Tests.Unit.ProcessManagers.SoftwareProgrammingDomain;
using GridDomain.Tests.Unit.ProcessManagers.SoftwareProgrammingDomain.Events;
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
            var serializer = new HyperionSerializer((ExtendedActorSystem) Sys);
            var bytes = serializer.ToBinary(msg);
            var restored = serializer.FromBinary(bytes, msg.GetType());
            Assert.NotNull(restored);
        }
        
        [Fact(Skip = "all exception should properly support ISerialization interface, will be done later")]
        public void Process_Faults_Should_be_deserializable()
        {
            //CoffeMakeFailedEvent
            var coffeMakeFailedEvent = new CoffeMakeFailedEvent(Guid.NewGuid(), Guid.NewGuid());
            CheckDeserialize(coffeMakeFailedEvent, nameof(coffeMakeFailedEvent));

            var undefinedCoffeMachineException = new UndefinedCoffeMachineException();
            CheckDeserialize(undefinedCoffeMachineException, nameof(undefinedCoffeMachineException));

            var processTransitionException = new ProcessTransitionException(coffeMakeFailedEvent, undefinedCoffeMachineException);

            CheckDeserialize(processTransitionException, nameof(processTransitionException));

            var eventExecutionException = new EventExecutionException("test",processTransitionException);
            CheckDeserialize(eventExecutionException, nameof(eventExecutionException));

            var msg = new Akka.Actor.Status.Failure(new AggregateException(eventExecutionException));
            CheckDeserialize(msg,nameof(msg));
        }

        private void CheckDeserialize(object msg, string message)
        {
            Log.Info("testing " + message);
            var serializer = new HyperionSerializer((ExtendedActorSystem) Sys);
            var bytes = serializer.ToBinary(msg);
            var restored = serializer.FromBinary(bytes, msg.GetType());
            Assert.NotNull(restored);
        }
    }
}