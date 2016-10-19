using System;
using System.Threading.Tasks;
using GridDomain.Node.AkkaMessaging.Waiting;
using NUnit.Framework;

namespace GridDomain.Tests.MessageWaiting
{
    [TestFixture]
    public class AkkaWaiter_messages_test_A_or_B_and_C : AkkaWaiterTest
    {
        private readonly Message _messageA = new Message("A");
        private readonly Message _messageB = new Message("B");
        private readonly Message _messageC = new Message("C");
        private readonly Message _messageD = new Message("D");


        protected override Task<IWaitResults> ConfigureWaiter(AkkaMessageLocalWaiter waiter)
        {
            return waiter.Expect<Message>(m => m.Id == _messageA.Id)
                        .Or<Message>(m => m.Id == _messageB.Id)
                        .And<Message>(m => m.Id == _messageC.Id)
                        .Start(TimeSpan.FromMilliseconds(200));
        }

        [Test]
        public void Should_end_on_A_and_C()
        {
            Publish(_messageA);
            Publish(_messageC);
            ExpectMsg(_messageA,m => m.Id == _messageA.Id);
            ExpectMsg(_messageC,m => m.Id == _messageC.Id);
        }

        [Test]
        public void Should_end_on_B_and_C()
        {
            Publish(_messageB);
            Publish(_messageC);
            ExpectMsg(_messageB, m => m.Id == _messageB.Id);
            ExpectMsg(_messageC, m => m.Id == _messageC.Id);
        }

        [Test]
        public void Condition_wait_end_should_be_true_on_B_and_C()
        {
            var sampleObjectsReceived = new object[] { _messageB, _messageC };
            Assert.True(Waiter.ExpectBuilder.WaitIsOver.Compile()(sampleObjectsReceived));
        }

        [Test]
        public void Should_not_end_on_C()
        {
            Publish(_messageC);
            ExpectNoMsg();
        }

        [Test]
        public void Should_not_end_on_B()
        {
            Publish(_messageB);
            ExpectNoMsg();
        }
    }
}