using System;
using Akka.Actor;
using Akka.Monitoring;
using Akka.Monitoring.Impl;
using GridDomain.CQRS;
using GridDomain.CQRS.Messaging;
using GridDomain.Logging;
using GridDomain.Node.AkkaMessaging;

namespace GridDomain.Node.Actors
{
    public class MessageHandlingActor<TMessage, THandler> : TypedActor where THandler : IHandler<TMessage>
                                                                    //   where TMessage : ISourcedEvent
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
            _monitor.IncrementMessagesReceived();
            _log.Trace("Handler actor got message: {Message}", msg.ToPropsString());

            try
            {
                _handler.Handle(msg);
            }
            catch (Exception e)
            {
                _log.Error(e);
                _publisher.Publish(MessageFault.New(msg, e, typeof(THandler)));
            }
            
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