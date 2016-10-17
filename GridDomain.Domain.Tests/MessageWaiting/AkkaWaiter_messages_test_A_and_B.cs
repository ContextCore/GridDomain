using System;
using NUnit.Framework;

namespace GridDomain.Tests.MessageWaiting
{
    [TestFixture]

    public class AkkaWaiter_messages_test_A_and_B : AkkaWaiterTest
    {
        private string _messageA = "et";
        private char _messageB = 'a';

        [OneTimeSetUp]
        public void Init()
        {
            _messageA = "testMsg";

            Waiter.Expect<string>()
                .And<char>()
                .Within(TimeSpan.FromSeconds(1));

        }

        [Test]
        public void A_and_B_should_end_wait()
        {
            Publish(_messageA, _messageB);

            Expect(_messageA);
            Expect(_messageB);
        }
    }
}