using System;
using Akka.Actor;
using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.CQRS.Messaging;
using GridDomain.EventSourcing;
using GridDomain.Logging;

namespace GridDomain.Node.Actors
{
    public static class MessageHandlingStatuses
    {
        public const string PublishingFault = "publishing fault";
        public const string MessageProcessCasuedAnError = "message process cased an error";
        public const string MessageProcessed = "message processed";
        public const string PublishingNotification = "publishing notification";
    }
    
    public class MessageHandlingActor<TMessage, THandler> : TypedActor where THandler : IHandler<TMessage>
    {
        private readonly THandler _handler;
        private readonly ISoloLogger _log = LogManager.GetLogger();
        private readonly ActorMonitor _monitor;
        private readonly IPublisher _publisher;

        public MessageHandlingActor(THandler handler,IPublisher publisher)
        {
            _publisher = publisher;
            _handler = handler;
            _monitor = new ActorMonitor(Context,typeof(THandler).Name);
        }
        
        public virtual void Handle(TMessage msg)
        {
            Handle(new MessageMetadataEnvelop<TMessage>(msg, MessageMetadata.Empty()));
        }

        public virtual void Handle(IMessageMetadataEnvelop<TMessage> msg)
        {
            _monitor.IncrementMessagesReceived();
            _log.Trace("Handler actor got message: {@Message}", msg);

            try
            {
                _handler.Handle(msg.Message);
            }
            catch (Exception e)
            {
                _log.Error(e, "Handler actor raised an error on message process: {@Message}", msg);

                var metadata = msg.Metadata.CreateChild(Guid.Empty,
                                                        new ProcessEntry(typeof(THandler).Name, MessageHandlingStatuses.PublishingFault, MessageHandlingStatuses.MessageProcessCasuedAnError));

                var fault = Fault.New(msg.Message, e, GetSagaId(msg.Message), typeof(THandler));

                _publisher.Publish(fault, metadata);
            }
        }

        //TODO: add custom saga id mapping
        protected virtual Guid GetSagaId(TMessage msg)
        {
            ISourcedEvent e = msg as ISourcedEvent;
            if (e != null) return e.SagaId;
            return Guid.Empty;
        }

        protected override void PreStart()
        {
            _monitor.IncrementActorStarted();
        }

        protected override void PostStop()
        {
            _monitor.IncrementActorStopped();
        }

        protected override void PreRestart(Exception reason, object message)
        {
            _monitor.IncrementActorRestarted();
        }
    }
}