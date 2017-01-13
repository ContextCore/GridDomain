using System.Threading.Tasks;
using GridDomain.CQRS;
using GridDomain.Node.AkkaMessaging.Waiting;
using NUnit.Framework;

namespace GridDomain.Tests.Unit.MessageWaiting
{
    [TestFixture]
    public class AkkaWaiter_messages_test_A_or_B_or_C : AkkaWaiterTest
    {
        private readonly Message _messageA = new Message("A");
        private readonly Message _messageB = new Message("B");
        private readonly Message _messageC = new Message("C");
        private readonly Message _messageD = new Message("D");

        protected override Task<IWaitResults> ConfigureWaiter(AkkaMessageLocalWaiter waiter)
        {
            return Waiter.Expect<Message>(m => m.Id == _messageA.Id)
                      .Or<Message>(m => m.Id == _messageB.Id)
                      .Or<Message>(m => m.Id == _messageC.Id)
                  .Create();
        }

        [Test]
        public void Should_end_on_A()
        {
            Publish(_messageA);
            ExpectMsg(_messageA);
        }


        [Test]
        public void Condition_wait_end_should_be_true_on_A()
        {
            var sampleObjectsReceived = new object[] { _messageA };
            Assert.True(Waiter.ExpectBuilder.WaitIsOver.Compile()(sampleObjectsReceived));
        }

        [Test]
        public void Should_end_on_B()
        {
            Publish(_messageB);
            ExpectMsg(_messageB);
        }

        [Test]
        public void Condition_wait_end_should_be_true_on_B()
        {
            var sampleObjectsReceived = new object[] { _messageA };
            Assert.True(Waiter.ExpectBuilder.WaitIsOver.Compile()(sampleObjectsReceived));
        }

        [Test]
        public void Should_end_on_C()
        {
            Publish(_messageC);
            ExpectMsg(_messageC);
        }

        [Test]
        public void Should_not_end_on_D()
        {
            Publish(_messageD);
            ExpectNoMsg();
        }
    }
}