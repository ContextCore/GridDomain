using System;
using System.Linq;
using System.Threading.Tasks;
using Akka.TestKit.Xunit2;
using GridDomain.CQRS;
using GridDomain.CQRS.Messaging.Akka;
using GridDomain.Node.AkkaMessaging.Waiting;
using Xunit;

namespace GridDomain.Tests.XUnit.MessageWaiting
{
    public class AkkaWaiter_messages_test_A_or_B : TestKit
    {
        private string _testmsgString = "testMsg";
        private bool _testmsgBool = true;

        private readonly LocalAkkaEventBusTransport _transport;
        private readonly Task<IWaitResults> _received;

        //Given_waiter_subscribed_for_one_of_two_messages
        public AkkaWaiter_messages_test_A_or_B()
        {
            _transport = new LocalAkkaEventBusTransport(Sys);

            _received = new AkkaMessageLocalWaiter(Sys, _transport, TimeSpan.FromSeconds(10))
                                    .Expect<string>()
                                    .Or<bool?>()
                                    .Create();
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