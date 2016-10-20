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

    public abstract class AkkaWaiterTest
    {
        private AkkaEventBusTransport _transport;
        private ActorSystem _actorSystem;
        private Task<IWaitResults> _results;

        [SetUp]
        public void Configure()
        {
            _actorSystem = ActorSystem.Create("test");
            _transport = new AkkaEventBusTransport(_actorSystem);
            Waiter = new AkkaMessageLocalWaiter(_actorSystem, _transport);
            _results = ConfigureWaiter(Waiter);
        }

        protected abstract Task<IWaitResults> ConfigureWaiter(AkkaMessageLocalWaiter waiter);

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

        protected void ExpectMsg<T>(T msg, Predicate<T> filter = null, TimeSpan? timeout = null)
        {
            if(!_results.Wait(timeout ?? DefaultTimeout))
                throw new TimeoutException();

            Assert.AreEqual(msg, _results.Result.Message(filter));
        }

        protected void ExpectNoMsg<T>(T msg, TimeSpan? timeout = null) where T : class
        {
            if (!_results.Wait(timeout ?? DefaultTimeout))
                return;

            var e = Assert.Throws<TimeoutException>(() => ExpectMsg(msg));
        }

        public TimeSpan DefaultTimeout { get; protected set; } = TimeSpan.FromMilliseconds(50);

        protected void ExpectNoMsg()
        {
           ExpectNoMsg<object>(null);
        }
    }
}
