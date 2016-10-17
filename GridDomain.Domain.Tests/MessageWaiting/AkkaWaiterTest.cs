using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.TestKit.NUnit3;
using GridDomain.CQRS.Messaging.Akka;
using GridDomain.Node.AkkaMessaging.Waiting;
using NUnit.Framework;

namespace GridDomain.Tests.MessageWaiting
{

    public class AkkaWaiterTest : TestKit
    {
        private AkkaEventBusTransport _transport;
        
        [SetUp]
        public void Configure()
        {
            _transport = new AkkaEventBusTransport(Sys);
            Waiter = new AkkaMessageLocalWaiter(Sys, _transport);
        }

        protected AkkaMessageLocalWaiter Waiter { get; private set; }
        

        protected Task<IWaitResults> Publish(params object[] messages)
        {
            _transport.Publish(messages);
            return Waiter.WhenReceiveAll;
        }

        protected void Expect<T>(T msg, Predicate<T> filter = null)
        {
            Assert.AreEqual(msg, Waiter.WhenReceiveAll.Result.Message<T>(filter));
        }
    }
}
