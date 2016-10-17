using System;
using Akka.TestKit.NUnit3;
using GridDomain.CQRS.Messaging.Akka;
using GridDomain.Node.AkkaMessaging.Waiting;
using NUnit.Framework;

namespace GridDomain.Tests.MessageWaiting
{
    [TestFixture]
    public class AkkaWaiter_One_Message_Test : TestKit
    {
        private AkkaMessageLocalWaiter _waiter;
        private string _testmsg;

        [SetUp]
        public void Given_waiter_subscribed_for_message_When_publishing_message()
        {
            var transport = new AkkaEventBusTransport(Sys);
            _waiter = new AkkaMessageLocalWaiter(Sys,transport);
            _waiter.Expect<string>();
            _waiter.Start(TimeSpan.FromSeconds(1));

            _testmsg = "TestMsg";
            transport.Publish(_testmsg);
        }

        [Test]
        public void Message_is_waitable()
        {
            Assert.True(_waiter.WhenReceiveAll.Wait(TimeSpan.FromMilliseconds(50)));
        }

        [Test]
        public void Multiply_waites_completes_after_message_was_received()
        {
            Assert.True(_waiter.WhenReceiveAll.Wait(TimeSpan.FromMilliseconds(50))
                        && _waiter.WhenReceiveAll.Wait(TimeSpan.FromMilliseconds(50))
                        && _waiter.WhenReceiveAll.Wait(TimeSpan.FromMilliseconds(50))
                        && _waiter.WhenReceiveAll.Wait(TimeSpan.FromMilliseconds(50)));
        }

        [Test]
        public void Message_is_included_in_typed_results()
        {
            var results = _waiter.WhenReceiveAll.Result;

            Assert.AreEqual(_testmsg, results.Message<string>());
        }

        [Test]
        public void Message_is_included_in_all_results()
        {
            var results = _waiter.WhenReceiveAll.Result;

            CollectionAssert.Contains(results.All, _testmsg);
        }
    }
}