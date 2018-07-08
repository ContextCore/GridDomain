using System;
using System.Linq;
using System.Threading.Tasks;
using Akka.Actor;
using GridDomain.Common;
using GridDomain.CQRS;

using GridDomain.Node.AkkaMessaging.Waiting;
using GridDomain.Transport;
using Xunit;

namespace GridDomain.Tests.Unit.MessageWaiting
{
    public abstract class AkkaWaiterTest : IDisposable
    {
        private readonly ActorSystem _actorSystem;
        private readonly Task<IWaitResult> _results;
        private readonly LocalAkkaEventBusTransport _transport;

        protected AkkaWaiterTest()
        {
            _actorSystem = TestActorSystem.Create();
            _transport = new LocalAkkaEventBusTransport(_actorSystem);
            Waiter = new MessagesWaiter(_actorSystem, _transport, TimeSpan.FromSeconds(1));
            _results = ConfigureWaiter(Waiter);
        }

        protected MessagesWaiter Waiter { get; }

        public TimeSpan DefaultTimeout { get; } = TimeSpan.FromMilliseconds(150);

        public void Dispose()
        {
            _actorSystem.Terminate().Wait();
        }

        protected abstract Task<IWaitResult> ConfigureWaiter(MessagesWaiter waiter);

        protected void Publish(params object[] messages)
        {
            foreach (var msg in messages)
                _transport.Publish(msg, MessageMetadata.Empty);
        }

        protected async Task ExpectMsg<T>(T msg, Predicate<T> filter = null)
        {
            await _results;

            filter = filter ?? (t => true);
            Assert.Equal(msg,
                         _results.Result.All.OfType<IMessageMetadataEnvelop>()
                                            .Select(m => m.Message)
                                            .OfType<T>()
                                            .FirstOrDefault(t => filter(t)));
        }

        protected void ExpectNoMsg<T>(T msg, TimeSpan? timeout = null) where T : class
        {
            if (!_results.Wait(timeout ?? DefaultTimeout))
                return;

            var e = Assert.ThrowsAsync<TimeoutException>(() => ExpectMsg(msg));
        }

        protected void ExpectNoMsg()
        {
            ExpectNoMsg<object>(null);
        }
    }
}