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
    public class AkkaWaiter_messages_test_A_or_B : AkkaWaiterTest
    {
        protected override Task<IWaitResults> ConfigureWaiter(AkkaMessageLocalWaiter waiter)
        {
           return waiter.Expect<string>()
                        .Or<char>()
                        .Create();
        }

        [Fact]
        public async Task When_publish_one_of_subscribed_message_Then_wait_is_over_And_message_received()
        {
            var msg = 'a';
            Publish(msg);
            await ExpectMsg(msg);
        }

        [Fact]
        public async Task When_publish_other_of_subscribed_message_Then_wait_is_over_And_message_received()
        {
            var msg = "testMsg";
            Publish(msg);
            await ExpectMsg(msg);
        }
    }
}