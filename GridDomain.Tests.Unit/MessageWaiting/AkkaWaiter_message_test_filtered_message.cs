using System;
using System.Linq;
using System.Threading.Tasks;
using Akka.TestKit.Xunit2;
using Akka.Util;
using GridDomain.Common;
using GridDomain.CQRS;

using GridDomain.Node.AkkaMessaging.Waiting;
using GridDomain.Transport;
using Xunit;

namespace GridDomain.Tests.Unit.MessageWaiting
{
    public class AkkaWaiter_message_test_filtered_message : TestKit
    {

        public AkkaWaiter_message_test_filtered_message()
        {
            _transport = new LocalAkkaEventBusTransport(Sys);
            var waiter = new MessagesWaiter(Sys,_transport,TimeSpan.FromSeconds(2));

            waiter.Expect<string>(m => m.Like("Msg"));
            _results = waiter.Start(TimeSpan.FromSeconds(100));
        }

        private readonly LocalAkkaEventBusTransport _transport;
        private readonly Task<IWaitResult> _results;

        [Fact]
        public void Message_not_satisfying_filter_should_not_be_received()
        {
            _transport.Publish("Test", MessageMetadata.Empty);

            var e = Assert.Throws<AggregateException>(() => _results.Result.Message<string>());
            Assert.IsAssignableFrom<TimeoutException>(e.InnerException);
        }

        [Fact]
        public void Message_satisfying_filter_should_be_received()
        {
            _transport.Publish("TestMsg",MessageMetadata.Empty);
            Assert.Equal("TestMsg", _results.Result.Message<string>());
        }

        [Fact]
        public void Timeout_should_be_fired_on_wait_and_no_message_publish()
        {
            var e = Assert.Throws<AggregateException>(() =>
                                                      {
                                                          var a = _results.Result;
                                                      });

            Assert.IsAssignableFrom<TimeoutException>(e.InnerExceptions.FirstOrDefault());
        }

        [Fact]
        public void Timeout_should_be_fired_on_wait_without_messages()
        {
            var e = Assert.Throws<AggregateException>(() => _results.Wait());
            Assert.IsAssignableFrom<TimeoutException>(e.InnerExceptions.FirstOrDefault());
        }
    }
}