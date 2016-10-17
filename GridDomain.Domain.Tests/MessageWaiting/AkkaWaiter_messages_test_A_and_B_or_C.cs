using System;
using NUnit.Framework;

namespace GridDomain.Tests.MessageWaiting
{
    public class AkkaWaiter_messages_test_A_and_B_or_C : AkkaWaiterTest
    {
        private string _messageA;
        private char _messageB;
        private int _messageC;

        [OneTimeSetUp]
        public void Init()
        {
            _messageA = "testMsg";
            _messageC = 'a';

            Waiter.Expect<string>()
                .And<char>()
                .Or<int>()
                .Within(TimeSpan.FromMilliseconds(50));
        }

        [Test]
        public void C_message_should_end_wait()
        {
            Publish(_messageC);

            Expect(_messageC);
        }

        [Test]
        public void A_message_should_not_end_wait()
        {
            Publish(_messageA);
            Assert.Throws<TimeoutException>(() => Expect(_messageA));
        }


        [Test]
        public void A_and_B_message_should_end_wait()
        {
            Publish(_messageA,_messageB);

            Expect(_messageA);
            Expect(_messageB);
        }
    }
}