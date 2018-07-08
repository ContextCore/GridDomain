using System.Threading.Tasks;
using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.Node.AkkaMessaging.Waiting;
using Xunit;

namespace GridDomain.Tests.Unit.MessageWaiting
{
    public class AkkaWaiter_messages_test_A_or_B_or_C : AkkaWaiterTest
    {
        private readonly Message _messageA = new Message("A");
        private readonly Message _messageB = new Message("B");
        private readonly Message _messageC = new Message("C");
        private readonly Message _messageD = new Message("D");
        private IMessagesExpectation _messageConditionFactory;

        protected override Task<IWaitResult> ConfigureWaiter(MessagesWaiter waiter)
        {
            var messageConditionFactory = Waiter.Expect<Message>(m => m.Id == _messageA.Id);
            var result = 
                messageConditionFactory
                    .Or<Message>(m => m.Id == _messageB.Id)
                    .Or<Message>(m => m.Id == _messageC.Id)
                    .Create();
            _messageConditionFactory = Waiter.CreateMessagesExpectation();
            return result;
        }

        [Fact]
        public void Condition_wait_end_should_be_true_on_A()
        {
            var sampleObjectsReceived = new object[] {MessageMetadataEnvelop.New(_messageA)};
            Assert.True(_messageConditionFactory.IsExpectationFulfilled(sampleObjectsReceived));
        }

        [Fact]
        public void Condition_wait_end_should_be_true_on_B()
        {
            var sampleObjectsReceived = new object[] {MessageMetadataEnvelop.New(_messageA)};
            Assert.True(_messageConditionFactory.IsExpectationFulfilled(sampleObjectsReceived));
        }

        [Fact]
        public async Task Should_end_on_A()
        {
            Publish(_messageA);
            await ExpectMsg(_messageA);
        }

        [Fact]
        public async Task Should_end_on_B()
        {
            Publish(_messageB);
            await ExpectMsg(_messageB);
        }

        [Fact]
        public async Task Should_end_on_C()
        {
            Publish(_messageC);
            await ExpectMsg(_messageC);
        }

        [Fact]
        public void Should_not_end_on_D()
        {
            Publish(_messageD);
            ExpectNoMsg();
        }
    }
}