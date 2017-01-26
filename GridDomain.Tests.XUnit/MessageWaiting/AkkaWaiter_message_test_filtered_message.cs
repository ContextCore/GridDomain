using System;
using System.Linq;
using System.Threading.Tasks;

using Akka.TestKit.Xunit2;
using Akka.Util;
using GridDomain.CQRS;
using GridDomain.CQRS.Messaging.Akka;
using GridDomain.Node.AkkaMessaging.Waiting;
using Xunit;

namespace GridDomain.Tests.XUnit.MessageWaiting
{
   
    public class AkkaWaiter_message_test_filtered_message : TestKit
    {
        private AkkaMessageLocalWaiter _waiter;
        private LocalAkkaEventBusTransport _transport;
        private Task<IWaitResults> results;

        public AkkaWaiter_message_test_filtered_message()
        {
            _transport = new LocalAkkaEventBusTransport(Sys);
            _waiter = new AkkaMessageLocalWaiter(Sys, _transport, TimeSpan.FromSeconds(2));
            _waiter.Expect<string>(m => m.Like("Msg"));
            results = _waiter.Start(TimeSpan.FromMilliseconds(50));
        }
      

        [Fact]
        public void Timeout_should_be_fired_on_wait_and_no_message_publish()
        {
            var e = Assert.Throws<AggregateException>(() =>
            {
                var a = results.Result;
            });

            Assert.IsAssignableFrom<TimeoutException>(e.InnerExceptions.FirstOrDefault());
        }

        [Fact]
        public void Timeout_should_be_fired_on_wait_without_messages()
        {
            var e = Assert.Throws<AggregateException>(() => results.Wait());
            Assert.IsAssignableFrom<TimeoutException>(e.InnerExceptions.FirstOrDefault());
        }

        [Fact]
        public void Message_satisfying_filter_should_be_received()
        {
            _transport.Publish("TestMsg");
            Assert.Equal("TestMsg", results.Result.Message<string>());
        }

        [Fact]
        public void Message_not_satisfying_filter_should_not_be_received()
        {
            _transport.Publish("Test");

            var e = Assert.Throws<AggregateException>(() => results.Result.Message<string>());
            Assert.IsAssignableFrom<TimeoutException>(e.InnerException);
        }
    }
}