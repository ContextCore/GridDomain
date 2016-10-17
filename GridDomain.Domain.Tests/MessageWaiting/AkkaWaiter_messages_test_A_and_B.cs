using System;
using NUnit.Framework;

namespace GridDomain.Tests.MessageWaiting
{
    [TestFixture]

    public class AkkaWaiter_messages_test_A_and_B : AkkaWaiterTest
    {
        private string _messageA = "et";
        private char _messageB = 'a';

        [SetUp]
        public void Init()
        {
            Waiter.Expect<string>()
                  .And<char>()
                  .Within(TimeSpan.FromSeconds(1000));

            Publish(_messageA);
            Publish(_messageB);
        }

        [Test]
        public void A_and_B_should_be_received()
        {
            Expect(_messageB);
            Expect(_messageA);
        }

  
    }
}