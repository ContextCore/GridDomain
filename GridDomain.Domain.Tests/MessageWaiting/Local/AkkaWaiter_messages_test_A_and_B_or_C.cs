using System;
using System.Threading.Tasks;
using GridDomain.Node.AkkaMessaging.Waiting;
using NUnit.Framework;

namespace GridDomain.Tests.MessageWaiting.Local
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
                         .Create(TimeSpan.FromMilliseconds(50));
        }

        [Test]
        public void C_message_should_end_wait()
        {
            Publish(_messageC);

            ExpectMsg(_messageC);
        }

        [Test]
        public void A_message_should_not_end_wait()
        {
            Publish(_messageA);
            ExpectNoMsg();
        }


        [Test]
        public void A_and_B_message_should_end_wait()
        {
            Publish(_messageA,_messageB);

            ExpectMsg(_messageA);
            ExpectMsg(_messageB);
        }
    }
}