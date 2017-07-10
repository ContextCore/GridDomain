using System;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Event;
using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.CQRS.Messaging;
using GridDomain.EventSourcing;

namespace GridDomain.Node.Actors.Hadlers
{
    public class MessageProcessActor<TMessage, THandler> : ReceiveActor where THandler : IHandler<TMessage>
                                                                        where TMessage : class, IHaveSagaId, IHaveId
    {
        private static readonly ProcessEntry FaltProcessEntry = new ProcessEntry(typeof(THandler).Name,
                                                                                 MessageHandlingConstants.PublishingFault,
                                                                                 MessageHandlingConstants.MessageProcessCasuedAnError);

        private readonly ILoggingAdapter _log = Context.GetLogger();

        private readonly ActorMonitor _monitor;
        protected readonly IPublisher Publisher;

        private int _publishFaultCount;

        public MessageProcessActor(THandler handler, IPublisher publisher)
        {
            Publisher = publisher;
            _monitor = new ActorMonitor(Context, typeof(THandler).Name);
            //to avoid creation of generic types in senders
            Receive<IMessageMetadataEnvelop>(envelop =>
                                             {
                                                 _monitor.IncrementMessagesReceived();
                                                 Task handleTask;

                                                 try
                                                 {
                                                      handleTask = handler.Handle((TMessage) envelop.Message, envelop.Metadata);
                                                 }
                                                 catch (Exception ex)
                                                 {
                                                      handleTask = Task.FromException(ex);
                                                 }

                                                 handleTask.ContinueWith(t =>
                                                 {
                                                     var error = t?.Exception.UnwrapSingle();
                                                     if(error != null)
                                                         PublishError((TMessage)envelop.Message, envelop.Metadata, error);
                                                     return new HandlerExecuted(envelop, error);
                                                 }).PipeTo(Sender);
                                             },
                                             m => m.Message is TMessage);
        }

        protected virtual void PublishError(TMessage msg, IMessageMetadata metadata, Exception ex)
        {
            var messageProcessException = new MessageProcessFailedException(ex);
            _log.Error(ex,
                       "Handler actor raised an error on message process: {@msg}. Count: {count}",
                       msg,
                       ++_publishFaultCount);

            var faultMetadata = metadata.CreateChild(msg.Id, FaltProcessEntry);

            var fault = Fault.NewGeneric(msg, ex, msg.SagaId, typeof(THandler));

            Publisher.Publish(fault, faultMetadata);
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