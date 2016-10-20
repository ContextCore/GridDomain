using System;
using System.Linq;
using System.Threading.Tasks;
using Akka.TestKit.NUnit3;
using Akka.Util;
using GridDomain.CQRS.Messaging.Akka;
using GridDomain.Node.AkkaMessaging.Waiting;
using NUnit.Framework;

namespace GridDomain.Tests.MessageWaiting.Local
{
    [TestFixture]
    public class AkkaWaiter_message_test_filtered_message : TestKit
    {
        private AkkaMessageLocalWaiter _waiter;
        private AkkaEventBusTransport _transport;
        private Task<IWaitResults> results;

        [SetUp]
        public void Given_waiter_subscribed_for_message_When_publishing_message()
        {
            _transport = new AkkaEventBusTransport(Sys);
            _waiter = new AkkaMessageLocalWaiter(Sys, _transport);
            _waiter.Expect<string>(m => m.Like("Msg"));
            results = _waiter.Start(TimeSpan.FromMilliseconds(50));
        }

        [Test]
        public void Timeout_should_be_fired_on_wait_and_no_message_publish()
        {
            var e = Assert.Throws<AggregateException>(() =>
            {
                var a = results.Result;
            });
            Assert.IsInstanceOf<TimeoutException>(e.InnerExceptions.FirstOrDefault());
        }

        [Test]
        public void Timeout_should_be_fired_on_wait_without_messages()
        {
            var e = Assert.Throws<AggregateException>(() => results.Wait());
            Assert.IsInstanceOf<TimeoutException>(e.InnerExceptions.FirstOrDefault());
        }

        [Test]
        public void Message_satisfying_filter_should_be_received()
        {
            _transport.Publish("TestMsg");
            Assert.AreEqual("TestMsg", results.Result.Message<string>());
        }

        [Test]
        public void Message_not_satisfying_filter_should_not_be_received()
        {
            _transport.Publish("Test");

            var e = Assert.Throws<AggregateException>(() => results.Result.Message<string>());
            Assert.IsInstanceOf<TimeoutException>(e.InnerException);
        }
    }
}