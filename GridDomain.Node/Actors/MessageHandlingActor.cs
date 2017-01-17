using System;
using System.Threading.Tasks;
using Akka;
using Akka.Actor;
using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.CQRS.Messaging;
using GridDomain.EventSourcing;
using GridDomain.Logging;

namespace GridDomain.Node.Actors
{
    public class MessageHandlingActor<TMessage, THandler> : ReceiveActor where THandler : IHandler<TMessage>
    {
        private readonly ILogger _log = LogManager.GetLogger();
        private readonly ActorMonitor _monitor;
        protected readonly IPublisher Publisher;

        public MessageHandlingActor(THandler handler, IPublisher publisher)
        {
            Publisher = publisher;
            _monitor = new ActorMonitor(Context, typeof(THandler).Name);

            Receive<IMessageMetadataEnvelop<TMessage>>(msg =>
            {
                _monitor.IncrementMessagesReceived();

                _log.Trace("Handler actor got message: {@Message}", msg);
                var handlerWithMetadata = handler as IHandlerWithMetadata<TMessage>;

                Func<Task> handlerExecute;
                if (handlerWithMetadata != null)
                {
                    handlerExecute = () => handlerWithMetadata.Handle(msg.Message, msg.Metadata);
                }
                else
                {
                    handlerExecute = () => handler.Handle(msg.Message);
                }

                try
                {
                    var sender = Sender;
                    handlerExecute().ContinueWith(t => new HandlerExecuted(msg, t?.Exception.UnwrapSingle()),
                                                            TaskContinuationOptions.ExecuteSynchronously)
                                     .PipeTo(Self,sender);
                }
                catch (Exception ex)
                {
                    //for case when handler cannot create its task
                    PublishFault(msg, ex);
                }
                
            });
            Receive<HandlerExecuted>(res =>
            {
                Sender.Tell(res);
                if (res.Error != null)
                    PublishFault(res.ProcessingMessage, res.Error);
            });
        }
    
    

        protected virtual void PublishFault(IMessageMetadataEnvelop msg, Exception ex)
        {
            _log.Error(ex, "Handler actor raised an error on message process: {@Message}", msg);

            var processEntry = new ProcessEntry(typeof(THandler).Name,
                MessageHandlingStatuses.PublishingFault,
                MessageHandlingStatuses.MessageProcessCasuedAnError);

            var metadata = msg.Metadata.CreateChild(Guid.Empty, processEntry);

            var fault = Fault.New(msg.Message, ex, GetSagaId(msg.Message), typeof(THandler));

            Publisher.Publish(fault, metadata);
        }

        //TODO: add custom saga id mapping
        protected Guid GetSagaId(object msg)
        {
            Guid? sagaId = null;

            msg.Match()
                .With<ISourcedEvent>(e => sagaId = e.SagaId)
                .With<IMessageMetadataEnvelop>(e => sagaId = (e.Message as ISourcedEvent)?.SagaId);

            if (sagaId.HasValue)
                return sagaId.Value;

            throw new CannotGetSagaFromMessage(msg);
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