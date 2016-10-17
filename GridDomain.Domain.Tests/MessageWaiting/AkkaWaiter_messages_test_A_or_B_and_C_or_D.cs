using System;
using NUnit.Framework;

namespace GridDomain.Tests.MessageWaiting
{
    [TestFixture]
    public class AkkaWaiter_messages_test_A_or_B_and_C_or_D : AkkaWaiterTest
    {
        private readonly Message _messageA = new Message("A");
        private readonly Message _messageB = new Message("B");
        private readonly Message _messageC = new Message("C");
        private readonly Message _messageD = new Message("D");

        [SetUp]
        public void Init()
        {
            Waiter.Expect<Message>(m => m.Id == _messageA.Id)
                .Or<Message>(m => m.Id == _messageB.Id)
                .And<Message>(m => m.Id == _messageC.Id)
                .Or<Message>(m => m.Id == _messageD.Id)
                .Within(TimeSpan.FromMilliseconds(100));
        }

        [Test]
        public void Should_end_on_A()
        {
            Publish(_messageA);
            ExpectMsg(_messageA);
        }

        [Test]
        public void Should_end_on_D()
        {
            Publish(_messageD);
            ExpectMsg(_messageD);
        }

        [Test]
        public void Should_end_on_B_and_C()
        {
            Publish(_messageC,_messageB);
            ExpectMsg(_messageC);
            ExpectMsg(_messageB);
        }

        [Test]
        public void Should_not_end_on_B()
        {
            Publish(_messageB);
            ExpectNoMsg();
        }

        [Test]
        public void Should_not_end_on_C()
        {
            Publish(_messageC);
            ExpectNoMsg();
        }
    }
}