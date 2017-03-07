using System.Threading.Tasks;
using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.Node.AkkaMessaging.Waiting;
using Xunit;

namespace GridDomain.Tests.XUnit.MessageWaiting
{
    public class AkkaWaiter_messages_test_A_or_B_and_C : AkkaWaiterTest
    {
        private readonly Message _messageA = new Message("A");
        private readonly Message _messageB = new Message("B");
        private readonly Message _messageC = new Message("C");
        private readonly Message _messageD = new Message("D");

        protected override Task<IWaitResults> ConfigureWaiter(LocalMessagesWaiter waiter)
        {
            return
                waiter.Expect<Message>(m => m.Id == _messageA.Id)
                      .Or<Message>(m => m.Id == _messageB.Id)
                      .And<Message>(m => m.Id == _messageC.Id)
                      .Create();
        }

        [Fact]
        public void Condition_wait_end_should_be_true_on_B_and_C()
        {
            var sampleObjectsReceived = new object[]
                                        {
                                            MessageMetadataEnvelop.New(_messageB),
                                            MessageMetadataEnvelop.New(_messageC)
                                        };
            Assert.True(Waiter.ConditionBuilder.StopCondition(sampleObjectsReceived));
        }

        [Fact]
        public async Task Should_end_on_A_and_C()
        {
            Publish(_messageA);
            Publish(_messageC);
            await ExpectMsg(_messageA, m => m.Id == _messageA.Id);
            await ExpectMsg(_messageC, m => m.Id == _messageC.Id);
        }

        [Fact]
        public async Task Should_end_on_B_and_C()
        {
            Publish(_messageB);
            Publish(_messageC);
            await ExpectMsg(_messageB, m => m.Id == _messageB.Id);
            await ExpectMsg(_messageC, m => m.Id == _messageC.Id);
        }

        [Fact]
        public void Should_not_end_on_B()
        {
            Publish(_messageB);
            ExpectNoMsg();
        }

        [Fact]
        public void Should_not_end_on_C()
        {
            Publish(_messageC);
            ExpectNoMsg();
        }
    }
}