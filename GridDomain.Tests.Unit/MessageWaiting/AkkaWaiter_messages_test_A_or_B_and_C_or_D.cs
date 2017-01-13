using System.Threading.Tasks;
using GridDomain.CQRS;
using GridDomain.Node.AkkaMessaging.Waiting;
using NUnit.Framework;

namespace GridDomain.Tests.Unit.MessageWaiting
{
    [TestFixture]
    public class AkkaWaiter_messages_test_A_or_B_and_C_or_D : AkkaWaiterTest
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
                          .Or<Message>(m => m.Id == _messageD.Id)
                         .Create();
        }

        [Test]
        public void Should_not_end_on_A()
        {
            Publish(_messageA);
            ExpectNoMsg();
        }

        [Test]
        public void Should_end_on_A_and_C()
        {
            Publish(_messageC, _messageA);
            ExpectMsg(_messageC, m => m.Id == _messageC.Id);
            ExpectMsg(_messageA, m => m.Id == _messageA.Id);
        }

        [Test]
        public void Condition_wait_end_should_be_false_on_A()
        {
            var sampleObjectsReceived = new object[] { _messageA };
            Assert.False(Waiter.ExpectBuilder.WaitIsOver.Compile()(sampleObjectsReceived));
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
            ExpectMsg(_messageC, m => m.Id == _messageC.Id);
            ExpectMsg(_messageB, m => m.Id == _messageB.Id);
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