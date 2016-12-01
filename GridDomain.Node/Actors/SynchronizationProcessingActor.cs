using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Akka;
using Akka.Actor;
using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.CQRS.Messaging;
using GridDomain.CQRS.Messaging.MessageRouting;
using GridDomain.EventSourcing;
using GridDomain.Logging;
using GridDomain.Node.AkkaMessaging;

namespace GridDomain.Node.Actors
{ 
    class SynchronizationProcessingActor<T> : UntypedActor where T: IProjectionGroup 
    {
        private readonly T _group;
        private readonly IPublisher _publisher;
        private ISoloLogger _log = LogManager.GetLogger();

        public SynchronizationProcessingActor(T group, IPublisher publisher)
        {
            _group = @group;
            _publisher = publisher;
        }

        protected override void OnReceive(object message)
        {
            try
            {
                _group.Project(message);
            }
            catch (MessageProcessException ex)
            {
                _log.Error(ex, "Handler actor raised an error on message process: {@Message}", message);
                var fault = Fault.NewGeneric(message, ex.InnerException, ex.Type, GetSagaId(message));

                var withMetadata = message as IMessageMetadataEnvelop;
                if (withMetadata == null)
                {
                    _publisher.Publish(fault);
                }
                else
                {
                
                   var metadata = withMetadata.Metadata.CreateChild(Guid.Empty,
                                                      new ProcessEntry(Self.Path.Name,
                                                                       "publishing fault",
                                                                       "message process casued an error"));

                   _publisher.Publish(fault, metadata);
                }
            }
        }

        //TODO: add custom saga id mapping
        private Guid GetSagaId(object msg)
        {
            Guid? sagaId = null;

            msg.Match()
                .With<ISourcedEvent>(e => sagaId = e.SagaId)
                .With<IMessageMetadataEnvelop>(e => sagaId = (e.Message as ISourcedEvent)?.SagaId);

            if (sagaId.HasValue)
                return sagaId.Value;

            throw new CannotGetSagaFromMessage(msg);
        }
    }

    internal class CannotGetSagaFromMessage : Exception
    {
        public object Msg { get;}

        public CannotGetSagaFromMessage(object msg)
        {
            Msg = msg;
        }
    }
}
