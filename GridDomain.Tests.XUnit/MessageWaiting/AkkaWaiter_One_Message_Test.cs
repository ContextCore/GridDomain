using System;
using System.Linq;
using System.Threading.Tasks;
using Akka.Actor;
using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.CQRS.Messaging.Akka;
using GridDomain.Node.AkkaMessaging.Waiting;
using Xunit;

namespace GridDomain.Tests.XUnit.MessageWaiting
{
    public class AkkaWaiter_One_Message_Test
    {
        //Given_waiter_subscribed_for_message_When_publishing_message()
        public AkkaWaiter_One_Message_Test()
        {
            var actorSystem = ActorSystem.Create("test");
            var transport = new LocalAkkaEventBusTransport(actorSystem);
            var waiter = new LocalExplicitMessagesWaiter(actorSystem, transport, TimeSpan.FromSeconds(10));
            waiter.Expect<string>();

            _results = waiter.Start(TimeSpan.FromSeconds(1));
            _testmsg = "TestMsg";
            transport.Publish(_testmsg);
        }

        private readonly string _testmsg;
        private readonly Task<IWaitResults> _results;

        [Fact]
        public void Message_is_included_in_all_results()
        {
            Assert.Contains(_testmsg, _results.Result.All.OfType<IMessageMetadataEnvelop>().Select(m => m.Message));
        }

        [Fact]
        public void Message_is_included_in_results_with_metadata()
        {
            Assert.Contains(_testmsg, _results.Result.MessageWithMetadata<string>().Message);
        }

        [Fact]
        public void Message_is_included_in_typed_results()
        {
            Assert.Equal(_testmsg, _results.Result.Message<string>());
        }

        [Fact]
        public async Task Message_is_waitable()
        {
            await _results;
        }

        [Fact]
        public async Task Multiply_waites_completes_after_message_was_received()
        {
            await _results;
            await _results;
            await _results;
            await _results;
        }
    }
}