using Akka.Actor;
using GridDomain.CQRS;
using GridDomain.Node.AkkaMessaging;
using NLog;

namespace GridDomain.Tests.Acceptance
{
    public class MessageTraceHandlingDecoratorActor<TMessage, THandler> : UntypedActor
        where THandler : IHandler<TMessage>
        where TMessage : IMetadataMessage
    {
        private readonly Logger _log = LogManager.GetCurrentClassLogger();
        private readonly IActorRef _messageHandler;

        public MessageTraceHandlingDecoratorActor(IActorRef messageHandler)
        {
            _messageHandler = messageHandler;
        }

        protected override void OnReceive(object message)
        {
            var msg = message as IMetadataMessage;
            if (msg == null)
                throw new MessageUnexpectedTypeRecievedExpection();

            _log.Trace($"got message {message.GetType().Name} on actor path:{Self.Path} hashCode:{GetHashCode()}");
            msg.Metadata.Add(new MetadataEntry(
                $"actor {Self.Path} hashCode {GetHashCode()}",
                $"passing message to recipient{_messageHandler.Path}",
                $"to add this entry without affecting domain logic"));

            _messageHandler.Tell(message);
        }
    }
}