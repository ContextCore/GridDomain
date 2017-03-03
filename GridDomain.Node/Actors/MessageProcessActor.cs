using System;
using System.Threading.Tasks;
using Akka;
using Akka.Actor;
using Akka.Event;
using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.CQRS.Messaging;
using GridDomain.EventSourcing;

namespace GridDomain.Node.Actors
{
    public class MessageProcessActor<TMessage, THandler> : ReceiveActor where THandler : IHandler<TMessage>
                                                                        where TMessage : class, IHaveSagaId, IHaveId
    {
        private static readonly ProcessEntry FaltProcessEntry = new ProcessEntry(typeof(THandler).Name,
                                                                                 MessageHandlingStatuses.PublishingFault,
                                                                                 MessageHandlingStatuses.MessageProcessCasuedAnError);

        private readonly ILoggingAdapter _log = Context.GetLogger();

        private readonly ActorMonitor _monitor;
        protected readonly IPublisher Publisher;

        private int _publishFaultCount;

        public MessageProcessActor(THandler handler, IPublisher publisher)
        {
            Publisher = publisher;
            _monitor = new ActorMonitor(Context, typeof(THandler).Name);
            var handlerWithMetadata = handler as IHandlerWithMetadata<TMessage>;

            Func<IMessageMetadataEnvelop<TMessage>, Task> handlerExecute;

            if (handlerWithMetadata != null)
                handlerExecute = env => handlerWithMetadata.Handle(env.Message, env.Metadata);
            else
                handlerExecute = env => handler.Handle(env.Message);

            //to avoid creation of generic types in senders
            Receive<IMessageMetadataEnvelop>(msg =>
                                             {
                                                 _monitor.IncrementMessagesReceived();

                                                 try
                                                 {
                                                     var sender = Sender;
                                                     handlerExecute((IMessageMetadataEnvelop<TMessage>) msg)
                                                         .ContinueWith(t =>
                                                                       {
                                                                           var error = t?.Exception.UnwrapSingle();
                                                                           var executed = new HandlerExecuted(msg, error);
                                                                           sender.Tell(executed);

                                                                           TMessage m = (TMessage) msg.Message;
                                                                           Publisher.Publish(m, msg.Metadata.CreateChild(m.Id));
                                                                           if (error != null)
                                                                               PublishFault(msg, error);
                                                                       });
                                                 }
                                                 catch (Exception ex)
                                                 {
                                                     //for case when handler cannot create its task
                                                     Self.Tell(new HandlerExecuted(msg, ex), Sender);
                                                 }
                                             },
                                             m => m.Message is TMessage);
        }

        protected virtual void PublishFault(IMessageMetadataEnvelop msg, Exception ex)
        {
            _log.Error(ex,
                       "Handler actor raised an error on message process: {@Message}. Count: {count}",
                       msg,
                       ++_publishFaultCount);

            var metadata = msg.Metadata.CreateChild(Guid.Empty, FaltProcessEntry);

            var fault = Fault.NewGeneric(msg.Message, ex, ((TMessage)msg.Message).SagaId, typeof(THandler));

            Publisher.Publish(fault, metadata);
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