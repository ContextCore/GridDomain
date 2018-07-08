using System;
using System.Linq;
using System.Threading.Tasks;
using Akka.TestKit.Xunit2;
using GridDomain.Common;
using GridDomain.CQRS;

using GridDomain.Node.AkkaMessaging.Waiting;
using GridDomain.Node.Configuration;
using GridDomain.Tests.Common.Configuration;
using GridDomain.Transport;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.Unit.MessageWaiting
{
    public class AkkaWaiter_One_Message_Test:TestKit
    
    {

        public AkkaWaiter_One_Message_Test(ITestOutputHelper output):base("test",output)
        {
            Transport = new LocalAkkaEventBusTransport(Sys);
            Waiter = new MessagesWaiter(Sys, Transport, TimeSpan.FromSeconds(100));  
            _results = Waiter.Expect<string>().Create();
            _testmsg = "TestMsg";
            Transport.Publish(_testmsg,MessageMetadata.Empty);
            
        }
        
        protected virtual IActorTransport Transport { get; } 
        protected virtual IMessageWaiter Waiter { get; }
        
      

        private readonly string _testmsg;
        private readonly Task<IWaitResult<string>> _results;

        [Fact]
        public void Message_is_included_in_all_results()
        {
            var messages = _results.Result;
            Assert.Contains(_testmsg, messages.All.OfType<IMessageMetadataEnvelop>().Select(m => m.Message));
        }

        [Fact]
        public void Message_is_included_in_results_with_metadata()
        {
            Assert.Contains(_testmsg, _results.Result.MessageWithMetadata<string>().Message);
        }

        [Fact]
        public void Message_is_included_in_typed_results()
        {
            Assert.Equal(_testmsg, _results.Result.Message<string>());
        }

        [Fact]
        public async Task Message_is_waitable()
        {
            await _results;
        }

        [Fact]
        public async Task Multiply_waites_completes_after_message_was_received()
        {
            await _results;
            await _results;
            await _results;
            await _results;
        }
    }
}