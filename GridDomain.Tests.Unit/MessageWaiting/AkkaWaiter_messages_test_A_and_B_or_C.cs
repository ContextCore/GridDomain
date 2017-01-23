using System.Threading.Tasks;
using GridDomain.CQRS;
using GridDomain.Node.AkkaMessaging.Waiting;
using NUnit.Framework;

namespace GridDomain.Tests.Unit.MessageWaiting
{
    [TestFixture]
    public class AkkaWaiter_messages_test_A_and_B_or_C : AkkaWaiterTest
    {
        private string _messageA;
        private char _messageB;
        private int _messageC;

        protected override Task<IWaitResults> ConfigureWaiter(AkkaMessageLocalWaiter waiter)
        {
            _messageA = "testMsg";
            _messageC = 'a';

            return waiter.Expect<string>()
                         .And<char>()
                         .Or<int>()
                         .Create();
        }

        [Test]
        public async Task C_message_should_end_wait()
        {
            Publish(_messageC);

            await ExpectMsg(_messageC);
        }

        [Test]
        public void A_message_should_not_end_wait()
        {
            Publish(_messageA);
            ExpectNoMsg();
        }


        [Test]
        public async Task A_and_B_message_should_end_wait()
        {
            Publish(_messageA,_messageB);

            await ExpectMsg(_messageA);
            await ExpectMsg(_messageB);
        }
    }
}