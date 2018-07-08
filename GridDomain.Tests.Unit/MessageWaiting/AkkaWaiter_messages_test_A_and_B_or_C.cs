using System.Threading.Tasks;
using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.Node.AkkaMessaging.Waiting;
using Xunit;

namespace GridDomain.Tests.Unit.MessageWaiting
{
    public class AkkaWaiter_messages_test_A_and_B_or_C : AkkaWaiterTest
    {
        private string _messageA;
        private readonly B _messageB = new B();
        private C _messageC;
        class B { }
        class C { }
        protected override Task<IWaitResult> ConfigureWaiter(MessagesWaiter waiter)
        {
            _messageA = "testMsg";
            _messageC = new C();

            return waiter.Expect<string>().And<B>().Or<C>().Create();
        }

        [Fact]
        public async Task A_and_B_message_should_end_wait()
        {
            Publish(_messageA, _messageB);

            await ExpectMsg(_messageA);
            await ExpectMsg(_messageB);
        }

        [Fact]
        public void A_message_should_not_end_wait()
        {
            Publish(_messageA);
            ExpectNoMsg();
        }

        [Fact]
        public async Task C_message_should_end_wait()
        {
            Publish(_messageC);

            await ExpectMsg(_messageC);
        }
    }
}