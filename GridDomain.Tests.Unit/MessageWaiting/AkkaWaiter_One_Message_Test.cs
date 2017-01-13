using System;
using System.Threading.Tasks;
using Akka.Actor;
using GridDomain.CQRS;
using GridDomain.CQRS.Messaging.Akka;
using GridDomain.Node.AkkaMessaging.Waiting;
using NUnit.Framework;

namespace GridDomain.Tests.Unit.MessageWaiting
{
    [TestFixture]
    public class AkkaWaiter_One_Message_Test
    {
        private AkkaMessageLocalWaiter _waiter;
        private string _testmsg;
        private Task<IWaitResults> _results;

        [SetUp]
        public void Given_waiter_subscribed_for_message_When_publishing_message()
        {
            var actorSystem = ActorSystem.Create("test");
            var transport = new LocalAkkaEventBusTransport(actorSystem);
            _waiter = new AkkaMessageLocalWaiter(actorSystem, transport, TimeSpan.FromSeconds(10));
            _waiter.Expect<string>();
            _results = _waiter.Start(TimeSpan.FromSeconds(1));

            _testmsg = "TestMsg";
            transport.Publish(_testmsg);
        }

        [Test]
        public async Task Message_is_waitable()
        {
            await _results;
        }

        [Test]
        public async Task Multiply_waites_completes_after_message_was_received()
        {
            await _results;
            await _results;
            await _results;
            await _results;
        }

        [Test]
        public void Message_is_included_in_typed_results()
        {
            var results = _results.Result;

            Assert.AreEqual(_testmsg, results.Message<string>());
        }

        [Test]
        public void Message_is_included_in_all_results()
        {
            var results = _results.Result;

            CollectionAssert.Contains(results.All, _testmsg);
        }
    }
}