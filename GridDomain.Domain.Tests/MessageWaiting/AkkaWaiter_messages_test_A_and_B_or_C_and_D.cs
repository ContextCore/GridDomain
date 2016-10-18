using System;
using NUnit.Framework;

namespace GridDomain.Tests.MessageWaiting
{
    [TestFixture]
    public class AkkaWaiter_messages_test_A_and_B_or_C_and_D : AkkaWaiterTest
    {
        private readonly Message _messageA = new Message("A");
        private readonly Message _messageB = new Message("B");
        private readonly Message _messageC = new Message("C");
        private readonly Message _messageD = new Message("D");

        [SetUp]
        public void Init()
        {
            Waiter.Expect<Message>(m => m.Id == _messageA.Id)
                  .And<Message>(m => m.Id == _messageB.Id)
                  .Or<Message>(m => m.Id == _messageC.Id)
                  .And<Message>(m => m.Id == _messageD.Id)
                  .Start(TimeSpan.FromMilliseconds(100));
        }

        [Test]
        public void Should_end_on_A_and_B_and_D()
        {
            Publish(_messageA, _messageB, _messageD);
            ExpectMsg(_messageA, m => m.Id == _messageA.Id);
            ExpectMsg(_messageB, m => m.Id == _messageB.Id);
            ExpectMsg(_messageD, m => m.Id == _messageD.Id);
        }

        [Test]
        public void Condition_wait_end_should_be_true_on_A_and_B_and_D()
        {
            var sampleObjectsReceived = new object[] { _messageA, _messageB, _messageD };
            Assert.True(Waiter.ExpectBuilder.WaitIsOver.Compile()(sampleObjectsReceived));
        }

        [Test]
        public void Should_not_end_on_C_and_A()
        {
            Publish(_messageC, _messageA);

            ExpectNoMsg();
            ExpectNoMsg();
        }

        [Test]
        public void Condition_wait_end_should_be_false_on_A_and_C()
        {
            var sampleObjectsReceived = new object[] { _messageA, _messageC };
            Assert.False(Waiter.ExpectBuilder.WaitIsOver.Compile()(sampleObjectsReceived));
        }
   
        [Test]
        public void Should_not_end_on_D_and_B()
        {
            Publish(_messageB, _messageD);

            ExpectNoMsg();
            ExpectNoMsg();
        }

        [Test]
        public void Should_end_on_C_and_D()
        {
            Publish(_messageC, _messageD);

            ExpectMsg(_messageC,m => m.Id == _messageC.Id);
            ExpectMsg(_messageD, m => m.Id == _messageD.Id);
        }
    }
}