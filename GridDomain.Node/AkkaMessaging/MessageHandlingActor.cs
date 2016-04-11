using Akka.Actor;
using Akka.IO;
using GridDomain.CQRS;
using GridDomain.Logging;
using NLog;

namespace GridDomain.Node.AkkaMessaging
{
    public class MessageHandlingActor<TMessage, THandler> : UntypedActor where THandler : IHandler<TMessage>
    {
        private readonly THandler _handler;
        private readonly Logger _log = LogManager.GetCurrentClassLogger();

        public MessageHandlingActor(THandler handler)
        {
            _handler = handler;
            _log.Trace($"Created message handler actor {GetType()}");
        }

        protected override void OnReceive(object msg)
        {
            _log.Trace($"Handler actor got message: {msg.ToPropsString()}");
            _handler.Handle((TMessage) msg);
        }
    }
}