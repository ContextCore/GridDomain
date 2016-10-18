using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.TestKit.NUnit3;
using GridDomain.CQRS.Messaging.Akka;
using GridDomain.Node.AkkaMessaging.Waiting;
using NUnit.Framework;

namespace GridDomain.Tests.MessageWaiting
{

    public class AkkaWaiterTest
    {
        private AkkaEventBusTransport _transport;
        private ActorSystem _actorSystem;

        [SetUp]
        public void Configure()
        {
            _actorSystem = ActorSystem.Create("test");
            _transport = new AkkaEventBusTransport(_actorSystem);
            Waiter = new AkkaMessageLocalWaiter(_actorSystem, _transport);
        }

        [TearDown]
        public void Clear()
        {
            Waiter.Dispose();
            _actorSystem.Terminate().Wait();
        }

        protected AkkaMessageLocalWaiter Waiter { get; private set; }
        

        protected void Publish(params object[] messages)
        {
            foreach(var msg in messages)
              _transport.Publish(msg);
        }

        protected void ExpectMsg<T>(T msg, Predicate<T> filter = null)
        {
            Assert.AreEqual(msg, Waiter.WhenReceiveAll.Result.Message(filter));
        }

        protected void ExpectNoMsg<T>(T msg, TimeSpan? timeout = null) where T : class
        {
            if (!Waiter.WhenReceiveAll.Wait(timeout ?? DefaultTimeout))
                return;

            var e = Assert.Throws<AggregateException>(() => ExpectMsg(msg));
            Assert.IsInstanceOf<TimeoutException>(e.InnerException);
        }

        public TimeSpan DefaultTimeout { get; protected set; } = TimeSpan.FromMilliseconds(50);

        protected void ExpectNoMsg()        {
            var e = Assert.Throws<AggregateException>(() => ExpectMsg<object>(null));
            Assert.IsInstanceOf<TimeoutException>(e.InnerException);
        }
    }
}
