using System;
using System.Collections.Generic;
using System.Linq;
using Akka.Actor;
using GridDomain.EventSourcing;
using GridDomain.Logging;

namespace GridDomain.Tests.Framework
{
    public class ExplicitSourcesEventWaiter<T> : ReceiveActor where T : ISourcedEvent
    {
        private readonly Guid[] _collection;
        private readonly ISoloLogger _log = LogManager.GetLogger();
        private readonly HashSet<Guid> _set;

        public ExplicitSourcesEventWaiter(IActorRef notifyActor, params Guid[] sources)
        {
            _collection = sources;
            _set = new HashSet<Guid>(_collection);

            Receive<T>(
                msg =>
                {
                    _log.Info($"got message:{msg.ToPropsString()} msg left to notify: {_set.Count}");

                    if (!_set.Contains(msg.SourceId)) return;
                    _set.Remove(msg.SourceId);

                    if (_set.Any()) return;
                    _log.Info($"got all expected messages, will notify. last message: {msg.ToPropsString()}");
                    notifyActor.Tell(new ExpectedMessagesRecieved<T>(msg));
                });
        }
    }
}