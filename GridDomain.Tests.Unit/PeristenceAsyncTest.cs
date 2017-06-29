using System.Threading.Tasks;
using Akka.Actor;
using Akka.TestKit.Xunit2;
using Xunit;

namespace GridDomain.Tests.Unit {
    public class PeristenceAsyncTest : TestKit
    {
        class TestingActor : ReceiveActor
        {
            public TestingActor()
            {
                ReceiveAsync<int>(i => Task.Run(() => State = i));
            }
            public int State { get; set; }
        }

        [Fact]
        public void Test()
        {
            var actor = ActorOfAsTestActorRef(() => new TestingActor());
            actor.Tell(10);
            Assert.Equal(10,actor.UnderlyingActor.State);
        }
    }
}