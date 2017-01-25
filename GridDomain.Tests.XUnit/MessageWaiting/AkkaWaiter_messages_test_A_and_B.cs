using System.Threading.Tasks;
using GridDomain.CQRS;
using GridDomain.Node.AkkaMessaging.Waiting;
using Xunit;

namespace GridDomain.Tests.XUnit.MessageWaiting
{
   
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

        [Fact]
        public void Condition_wait_end_should_be_true_on_A_and_B()
        {
            var sampleObjectsReceived = new object[] { _messageA, _messageB };
            Assert.True(Waiter.ExpectBuilder.WaitIsOver.Compile()(sampleObjectsReceived));
        }

        [Fact]
        public void Condition_wait_end_should_be_false_on_A()
        {
            var sampleObjectsReceived = new object[] { _messageA };
            Assert.False(Waiter.ExpectBuilder.WaitIsOver.Compile()(sampleObjectsReceived));
        }

        [Fact]
        public void Condition_wait_end_should_be_false_on_B()
        {
            var sampleObjectsReceived = new object[] { _messageB };
            Assert.False(Waiter.ExpectBuilder.WaitIsOver.Compile()(sampleObjectsReceived));
        }

        [Fact]
        public async Task A_and_B_should_be_received()
        {
            await ExpectMsg(_messageB);
            await ExpectMsg(_messageA);
        }

  
    }
}