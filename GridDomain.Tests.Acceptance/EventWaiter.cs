using System;
using System.Linq;
using Akka.Actor;
using GridDomain.EventSourcing;
using NLog;

namespace GridDomain.Tests.Acceptance
{
    public class EventWaiter<T> : ReceiveActor where T: ISourcedEvent
    {
        private int _numLeft;
        private Logger _log = LogManager.GetCurrentClassLogger();

        public EventWaiter(IActorRef notifyActor, int numLeft, params Guid[] sources)
        {
            _numLeft = numLeft;
            this.Receive<T>(
                msg =>
                {
                    if (sources.Contains(msg.SourceId) && --_numLeft <= 0)
                    {
                        _log.Info("got message for " + this.GetHashCode());
                        notifyActor.Tell(new ExpectedMessagesRecieved<T>(msg, numLeft, sources));
                    }
                });

        }
    }
}