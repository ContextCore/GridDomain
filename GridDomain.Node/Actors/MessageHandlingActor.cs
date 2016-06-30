using Akka.Actor;
using GridDomain.CQRS;
using GridDomain.Logging;

namespace GridDomain.Node.Actors
{
    public class MessageHandlingActor<TMessage, THandler> : UntypedActor where THandler : IHandler<TMessage>
    {
        private readonly THandler _handler;
        private readonly ISoloLogger _log = LogManager.GetLogger();

        public MessageHandlingActor(THandler handler)
        {
            _handler = handler;
            _log.Trace($"Created message handler actor {GetType()}");
        }

        protected override void OnReceive(object msg)
        {
            _log.Trace($"Handler actor got message: {msg.ToPropsString()}");
            _handler.Handle((TMessage)msg);
        }
    }
}