using System;
using System.Linq;
using System.Threading.Tasks;
using Akka.TestKit.Xunit;
using GridDomain.CQRS;
using GridDomain.CQRS.Messaging.Akka;
using GridDomain.Node.AkkaMessaging.Waiting;
using Xunit;

namespace GridDomain.Tests.XUnit.MessageWaiting
{
   

    public class AkkaWaiter_messages_test_A_or_B : TestKit
    {
        private AkkaMessageLocalWaiter _waiter;
        private string _testmsgString = "testMsg";
        private bool _testmsgBool = true;

        private LocalAkkaEventBusTransport _transport;
        private Task<IWaitResults> _received;

        //Given_waiter_subscribed_for_one_of_two_messages
        public AkkaWaiter_messages_test_A_or_B()
        {
            
            _transport = new LocalAkkaEventBusTransport(Sys);
            _waiter = new AkkaMessageLocalWaiter(Sys, _transport, TimeSpan.FromSeconds(10));
            _waiter.Expect<string>()
                   .Or<bool?>();
            _received = _waiter.Start(TimeSpan.FromSeconds(1));
        }

        [Fact]
        public void When_publish_one_of_subscribed_message_Then_wait_is_over_And_message_received()
        {
            _transport.Publish(_testmsgBool);
            Assert.Equal(_testmsgBool, _received.Result.All.OfType<bool>().First());
        }

        [Fact]
        public void When_publish_other_of_subscribed_message_Then_wait_is_over_And_message_received()
        {
            _transport.Publish(_testmsgString);
            Assert.Equal(_testmsgString, _received.Result.Message<string>());
        }
    }
}