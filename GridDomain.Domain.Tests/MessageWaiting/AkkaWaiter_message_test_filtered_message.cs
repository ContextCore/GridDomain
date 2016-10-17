using System;
using Akka.TestKit.NUnit3;
using Akka.Util;
using GridDomain.CQRS.Messaging.Akka;
using GridDomain.Node.AkkaMessaging.Waiting;
using NUnit.Framework;

namespace GridDomain.Tests.MessageWaiting
{
    [TestFixture]
    public class AkkaWaiter_message_test_filtered_message : TestKit
    {
        private AkkaMessageLocalWaiter _waiter;
        private AkkaEventBusTransport _transport;

        [SetUp]
        public void Given_waiter_subscribed_for_message_When_publishing_message()
        {
            _transport = new AkkaEventBusTransport(Sys);
            _waiter = new AkkaMessageLocalWaiter(Sys, _transport);
            _waiter.Expect<string>(m => m.Like("Msg"));
            _waiter.Start(TimeSpan.FromMilliseconds(50));
        }


        [Test]
        public void Timeout_should_be_fired_on_wait_and_no_message_publish()
        {
            Assert.Throws<TimeoutException>(() =>
            {
                var a = _waiter.WhenReceiveAll().Result;
            });
        }

        [Test]
        public void Timeout_should_be_fired_on_wait_without_messages()
        {
            Assert.Throws<TimeoutException>(() =>
            {
                _waiter.WhenReceiveAll().Wait();
            });
        }

        [Test]
        public void Message_satisfying_filter_should_be_received()
        {
            var testmsg = "TestMsg";
            _transport.Publish(testmsg);
            Assert.AreEqual(testmsg,_waiter.WhenReceiveAll().Result.Message<string>());
        }

        [Test]
        public void Message_not_satisfying_filter_should_not_be_received()
        {
            var testmsg = "Test";
            _transport.Publish(testmsg);

            Assert.Throws<TimeoutException>(() => _waiter.WhenReceiveAll().Result.Message<string>());
        }
    }
}