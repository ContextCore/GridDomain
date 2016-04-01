using System;
using System.Diagnostics;
using System.Threading;
using NLog;

namespace GridDomain.Domain.Tests.ProcessingWait
{
    //TODO: учитывать в качестве таймаута время между сообщениями, а не полное
    public class MessageWaiter
    {
        readonly ManualResetEvent _waitEvent = new ManualResetEvent(false);
        private readonly TimeSpan _timeout;

        private IStopCondition _waitStopCondition;
        private Action<object> _onRecieve;
        private Stopwatch _watch = new Stopwatch();
        private Logger _log = LogManager.GetCurrentClassLogger();

        public MessageWaiter(TimeSpan timeout)
        {
            _timeout = timeout;
        }

        public static MessageWaiter NewDebugFriendly(TimeSpan timeoutWithoudDebug)
        {
            return new MessageWaiter(Debugger.IsAttached ? TimeSpan.FromMinutes(10):timeoutWithoudDebug);
        }
        //TODO: переделать на builder, использовать Task для возвращаемого значения
        public void WaitForMessage<T>(Action act)
        {
            WaitForMessage(act, (Action<T>) (o => { }));
        }

        public void WaitForMessage<T>(Action act, Action<T> onRecieve)
        {
            _waitEvent.Reset();
            _onRecieve = t => onRecieve((T)t);
            StartWith(new MessageOfType<T>());
            act();
            Wait();
        }



        private void Wait()
        {
            _watch.Restart();
            if (!_waitEvent.WaitOne(_timeout))
                throw new TimeoutException();
        }

        public void WaitManyDifferent<T>(Action act,int count, Func<T,T,bool> comparer)
        {
            _waitEvent.Reset();
            StartWith(new ManyUniqueMessagesOfType<T>(comparer, count));
            act();
            Wait();
        }

        private bool IsStarted => _waitStopCondition != null;

        private void StartWith(IStopCondition condition)
        {
            _waitStopCondition = condition;
        }

        public void Notify(object msg)
        {
            if (IsStarted && _waitStopCondition.IsMeet(msg))
            {
                _onRecieve?.Invoke(msg);
                _watch.Stop();
                _log.Info("Ожидание заняло " + _watch.Elapsed);
                _waitEvent.Set();
            }
        }
    }
}