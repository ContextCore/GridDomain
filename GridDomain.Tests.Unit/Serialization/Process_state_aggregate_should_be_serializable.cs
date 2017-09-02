using System;
using System.Reflection;
using Akka.Actor;
using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.EventSourcing;
using GridDomain.ProcessManagers.State;
using GridDomain.Tests.Unit.BalloonDomain.Events;
using GridDomain.Tests.Unit.ProcessManagers.SoftwareProgrammingDomain;
using Newtonsoft.Json;
using Xunit;
using GridDomain.Node;
using GridDomain.Tests.Unit.BalloonDomain.ProjectionBuilders;

namespace GridDomain.Tests.Unit.Serialization
{
    public class Message_serialization
    {
        [Fact]
        public void Message_Envelop_should_be_serializable()
        {
            var evt = new BalloonTitleChanged("123",Guid.NewGuid());
            var fault = new Fault<BalloonTitleChanged>(evt, new Exception(), typeof(object), Guid.NewGuid(), DateTime.Now);
            //Fault
            //ballonTitleChanged
            var msg = new MessageMetadataEnvelop<Fault<BalloonTitleChanged>>(fault,MessageMetadata.Empty);
            var serializer = new DebugHyperionSerializer((ExtendedActorSystem)ActorSystem.Create("test"));

            var bytes = serializer.ToBinary(msg);
            var restored = serializer.FromBinary(bytes, typeof(MessageMetadataEnvelop<Fault<BalloonTitleChanged>>));  
            Assert.NotNull(restored);
        }
        [Fact]
        public void MessageHandleException_should_be_serializable()
        {
            var ex = new MessageHandleException(new BalloonTitleChanged("123", Guid.NewGuid()));
           
            var serializer = new DebugHyperionSerializer((ExtendedActorSystem)ActorSystem.Create("test"));
            var bytes = serializer.ToBinary(ex);

            var restored = serializer.FromBinary(bytes, typeof(MessageHandleException));  
            Assert.NotNull(restored);
        }

       //[Fact]
       //public void TargetnvocationException_should_be_serializable()
       //{
       //    var ex = new TargetInvocationException(new Exception());
       //   
       //    var serializer = new DebugHyperionSerializer((ExtendedActorSystem)ActorSystem.Create("test"));
       //    var bytes = serializer.ToBinary(ex);
       //
       //    var restored = serializer.FromBinary(bytes, typeof(TargetInvocationException));  
       //    Assert.NotNull(restored);
       //}
        [Fact]
        public void AggregateException_should_be_serializable()
        {
            var ex = new AggregateException(new MessageHandleException(new BalloonTitleChanged("123", Guid.NewGuid())));

            var serializer = new DebugHyperionSerializer((ExtendedActorSystem)ActorSystem.Create("test"));
            var bytes = serializer.ToBinary(ex);

            var restored = (AggregateException)serializer.FromBinary(bytes, typeof(AggregateException));
            Assert.IsType<MessageHandleException>(restored.UnwrapSingle());
        }
    }
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