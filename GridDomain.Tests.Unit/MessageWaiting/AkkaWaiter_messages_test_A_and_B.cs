using System.Threading.Tasks;
using GridDomain.CQRS;
using GridDomain.Node.AkkaMessaging.Waiting;
using NUnit.Framework;

namespace GridDomain.Tests.Unit.MessageWaiting
{
    [TestFixture]

    public class AkkaWaiter_messages_test_A_and_B : AkkaWaiterTest
    {
        private string _messageA = "et";
        private char _messageB = 'a';

        protected override Task<IWaitResults> ConfigureWaiter(AkkaMessageLocalWaiter waiter)
        {
            var task = Waiter.Expect<string>()
                             .And<char>()
                             .Create();

            Publish(_messageA);
            Publish(_messageB);

            return task;
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