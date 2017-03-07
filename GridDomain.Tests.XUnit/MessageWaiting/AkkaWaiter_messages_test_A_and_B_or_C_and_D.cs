using System.Threading.Tasks;
using GridDomain.CQRS;
using GridDomain.Node.AkkaMessaging.Waiting;
using Xunit;

namespace GridDomain.Tests.XUnit.MessageWaiting
{
    public class AkkaWaiter_messages_test_A_and_B_or_C_and_D : AkkaWaiterTest
    {
        private readonly Message _messageA = new Message("A");
        private readonly Message _messageB = new Message("B");
        private readonly Message _messageC = new Message("C");
        private readonly Message _messageD = new Message("D");

        protected override Task<IWaitResults> ConfigureWaiter(LocalExplicitMessagesWaiter waiter)
        {
            return
                waiter.Expect<Message>(m => m.Id == _messageA.Id)
                      .And<Message>(m => m.Id == _messageB.Id)
                      .Or<Message>(m => m.Id == _messageC.Id)
                      .And<Message>(m => m.Id == _messageD.Id)
                      .Create();
        }

        [Fact]
        public void Condition_wait_end_should_be_false_on_A_and_C()
        {
            var sampleObjectsReceived = new object[] {_messageA, _messageC};
            Assert.False(Waiter.ConditionBuilder.StopCondition(sampleObjectsReceived));
        }

        [Fact]
        public void Condition_wait_end_should_be_true_on_A_and_B_and_D()
        {
            var sampleObjectsReceived = new object[] {_messageA, _messageB, _messageD};
            Assert.True(Waiter.ConditionBuilder.StopCondition(sampleObjectsReceived));
        }

        [Fact]
        public async Task Should_end_on_A_and_B_and_D()
        {
            Publish(_messageA, _messageB, _messageD);
            await ExpectMsg(_messageA, m => m.Id == _messageA.Id);
            await ExpectMsg(_messageB, m => m.Id == _messageB.Id);
            await ExpectMsg(_messageD, m => m.Id == _messageD.Id);
        }

        [Fact]
        public async Task Should_end_on_C_and_D()
        {
            Publish(_messageC, _messageD);

            await ExpectMsg(_messageC, m => m.Id == _messageC.Id);
            await ExpectMsg(_messageD, m => m.Id == _messageD.Id);
        }

        [Fact]
        public void Should_not_end_on_C_and_A()
        {
            Publish(_messageC, _messageA);

            ExpectNoMsg();
            ExpectNoMsg();
        }

        [Fact]
        public void Should_not_end_on_D_and_B()
        {
            Publish(_messageB, _messageD);

            ExpectNoMsg();
        }
    }
}