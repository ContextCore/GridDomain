using System;
using Akka.Actor;
using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.Node;
using GridDomain.Tests.Unit.BalloonDomain.Events;
using GridDomain.Tests.Unit.BalloonDomain.ProjectionBuilders;
using GridDomain.Tests.Unit.MessageWaiting;
using Xunit;

namespace GridDomain.Tests.Unit.Serialization {
    public class Message_serialization
    {
        [Fact]
        public void Message_Envelop_should_be_serializable()
        {
            var evt = new BalloonTitleChanged("123",Guid.NewGuid().ToString());
            var fault = new Fault<BalloonTitleChanged>(evt, new Exception(), typeof(object), Guid.NewGuid().ToString(), DateTime.Now);
            //Fault
            //ballonTitleChanged
            var msg = new MessageMetadataEnvelop<Fault<BalloonTitleChanged>>(fault,MessageMetadata.Empty);
            var serializer = new DebugHyperionSerializer((ExtendedActorSystem)TestActorSystem.Create());

            var bytes = serializer.ToBinary(msg);
            var restored = serializer.FromBinary(bytes, typeof(MessageMetadataEnvelop<Fault<BalloonTitleChanged>>));  
            Assert.NotNull(restored);
        }
        [Fact]
        public void MessageHandleException_should_be_serializable()
        {
            var ex = new MessageHandleException(new BalloonTitleChanged("123", Guid.NewGuid().ToString()));
           
            var serializer = new DebugHyperionSerializer((ExtendedActorSystem)TestActorSystem.Create());
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
            var ex = new AggregateException(new MessageHandleException(new BalloonTitleChanged("123", Guid.NewGuid().ToString())));

            var serializer = new DebugHyperionSerializer((ExtendedActorSystem)TestActorSystem.Create());
            var bytes = serializer.ToBinary(ex);

            var restored = (AggregateException)serializer.FromBinary(bytes, typeof(AggregateException));
            Assert.IsType<MessageHandleException>(restored.UnwrapSingle());
        }
    }
}