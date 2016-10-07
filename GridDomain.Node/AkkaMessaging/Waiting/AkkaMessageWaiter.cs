using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Akka.Actor;
using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.CQRS.Messaging.Akka;
using MemBus.Support;

namespace GridDomain.Node.AkkaMessaging.Waiting
{
    class AkkaMessageWaiter : IMessageWaiter
    {
        private readonly IActorSubscriber _subscriber;
        private readonly TimeSpan _defaultTimeout;
        private readonly ConcurrentBag<object> _allMessages = new ConcurrentBag<object>();
        private readonly Inbox _inbox;
        protected readonly IDictionary<Type, Predicate<object>> Filters = new Dictionary<Type, Predicate<object>>();

        public AkkaMessageWaiter(ActorSystem system, IActorSubscriber subscriber, TimeSpan defaultTimeout)
        {
            _defaultTimeout = defaultTimeout;
            _subscriber = subscriber;
            _inbox = Inbox.Create(system);
            OnMessageReceived += (s, e) => RememberMessage(e);
        }

        public void Subscribe<TMsg>(Predicate<TMsg> filter = null)
        {
            Filters[typeof(TMsg)] = o => (filter ?? (t => true))((TMsg) o);

            _subscriber.Subscribe<TMsg>(_inbox.Receiver);
        }

        protected virtual bool RememberMessage(object msg)
        {
            if (!IsMessageExpected(msg)) return false;
            _allMessages.Add(msg);
            return true;
        }

        protected virtual bool IsWaitEnded(object lastMessage)
        {
            return IsFault(lastMessage) || AllExpectedMessagesReceived();
        }

        private bool AllExpectedMessagesReceived()
        {
            return Filters.Values.All(f => _allMessages.Any(m => f(m)));
        }

        private bool IsFault(object lastMessage)
        {
            if (!(lastMessage is Failure) && !(lastMessage is IFault) && !(lastMessage is Status.Failure)) return false;
            _allMessages.Add(lastMessage);
            return true;
        }

        protected virtual bool IsMessageExpected(object msg)
        {
            Predicate<object> filter;
            return Filters.TryGetValue(msg.GetType(), out filter) && filter(msg);
        }

        private void Start()
        {
            _inbox.ReceiveAsync(_defaultTimeout)
                  .ContinueWith(t =>
                  {
                      OnMessageReceived(this, t.Result);
                      if(!IsWaitEnded(t)) Start();
                      WaitEnd.SetResult(true);
                      WaitComplete(this, null);
                  }, TaskContinuationOptions.OnlyOnRanToCompletion);
        }

        public event EventHandler<object> OnMessageReceived = delegate{};
        public event EventHandler WaitComplete = delegate{};

        protected readonly TaskCompletionSource<bool> WaitEnd = new TaskCompletionSource<bool>();

        public IReadOnlyCollection<object> AllReceivedMessages => _allMessages;

        public Task WaitAll()
        {
           Start();
           return WaitEnd.Task;
        }

        public TMsg Received<TMsg>(Predicate<TMsg> selector = null)
        {
            var sel = selector ?? (m => true);
            return _allMessages.OfType<TMsg>().FirstOrDefault(i => sel(i));
        }
    }
}