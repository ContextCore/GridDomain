using System;
using System.Linq;
using Akka.Actor;
using GridDomain.EventSourcing;
using NLog;

namespace GridDomain.Tests.Acceptance
{
    public class EventWaiter<T> : ReceiveActor where T : ISourcedEvent
    {
        private readonly Logger _log = LogManager.GetCurrentClassLogger();
        private int _numLeft;

        public EventWaiter(IActorRef notifyActor, int numLeft, params Guid[] sources)
        {
            _numLeft = numLeft;
            Receive<T>(
                msg =>
                {
                    if (sources.Contains(msg.SourceId) && --_numLeft <= 0)
                    {
                        _log.Info("got message for " + GetHashCode());
                        notifyActor.Tell(new ExpectedMessagesRecieved<T>(msg, numLeft, sources));
                    }
                });
        }
    }
}