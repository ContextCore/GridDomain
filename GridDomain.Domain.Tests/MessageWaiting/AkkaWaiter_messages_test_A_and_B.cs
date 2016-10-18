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
                  .Start(TimeSpan.FromSeconds(100));

            Publish(_messageA);
            Publish(_messageB);
        }

        [Test]
        public void Condition_wait_end_should_be_true_on_A_and_B()
        {
            var sampleObjectsReceived = new object[] { _messageA, _messageB };
            Assert.True(Waiter.ExpectBuilder.WaitIsOver.Compile()(sampleObjectsReceived));
        }

        [Test]
        public void Condition_wait_end_should_be_false_on_A()
        {
            var sampleObjectsReceived = new object[] { _messageA };
            Assert.False(Waiter.ExpectBuilder.WaitIsOver.Compile()(sampleObjectsReceived));
        }

        [Test]
        public void Condition_wait_end_should_be_false_on_B()
        {
            var sampleObjectsReceived = new object[] { _messageB };
            Assert.False(Waiter.ExpectBuilder.WaitIsOver.Compile()(sampleObjectsReceived));
        }

        [Test]
        public void A_and_B_should_be_received()
        {
            ExpectMsg(_messageB);
            ExpectMsg(_messageA);
        }

  
    }
}