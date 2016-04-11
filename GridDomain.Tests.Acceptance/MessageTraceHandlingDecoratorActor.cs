using Akka.Actor;
using GridDomain.CQRS;
using NLog;

namespace GridDomain.Node.AkkaMessaging
{
    public class MessageTraceHandlingDecoratorActor<TMessage, THandler> : UntypedActor where THandler : IHandler<TMessage>
        where TMessage: IMetadataMessage
    {
        private readonly IActorRef _messageHandler;
        private readonly Logger _log = LogManager.GetCurrentClassLogger();

        public MessageTraceHandlingDecoratorActor(IActorRef messageHandler)
        {
            _messageHandler = messageHandler;
        }

        protected override void OnReceive(object message)
        {
            IMetadataMessage msg = message as IMetadataMessage;
            if (msg == null)
                throw new MessageUnexpectedTypeRecievedExpection();

            _log.Trace($"got message {message.GetType().Name} on actor path:{Self.Path} hashCode:{this.GetHashCode()}");
            msg.Metadata.Add(new MetadataEntry(
                $"actor {Self.Path} hashCode {GetHashCode()}",
                $"passing message to recipient{_messageHandler.Path}",
                $"to add this entry without affecting domain logic"));

            _messageHandler.Tell(message);
        }
    }
}