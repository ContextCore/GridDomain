using System;
using NEventStore;

namespace GridDomain.Domain.Tests.ProcessingWait
{
    public class WaitableObserver : IObserver<ICommit>
    {
        private readonly IObserver<ICommit> _observer;
        private readonly MessageWaiter _waiter;

        public void OnNext(ICommit commit)
        {
            _observer.OnNext(commit);

            foreach (var @event in commit.Events)
            {
                _waiter.Notify(@event.Body);
            }
        }

        public WaitableObserver(IObserver<ICommit> observer, MessageWaiter waiter)
        {
            _waiter = waiter;
            _observer = observer;
        }

        public void OnError(Exception error)
        {
            _observer.OnError(error);
        }

        public void OnCompleted()
        {
            _observer.OnCompleted();
        }
    }
}