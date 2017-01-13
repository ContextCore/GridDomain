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
    class SynchronizationProcessingActor<T> : MessageHandlingActor<object,IProjectionGroup> where T: IProjectionGroup 
    {

        public SynchronizationProcessingActor(T group, IPublisher publisher):base(group, publisher)
        {
        
        }

        protected override void PublishFault(IMessageMetadataEnvelop<object> msg, Exception ex)
        {
            var projectionGroupException = ex as ProjectionGroupMessageProcessException;
            if (projectionGroupException == null)
            {
                base.PublishFault(msg, ex);
                return;
            }

            var processEntry = new ProcessEntry(typeof(T).Name,
                                                MessageHandlingStatuses.PublishingFault,
                                                MessageHandlingStatuses.MessageProcessCasuedAnError);

            var metadata = msg.Metadata.CreateChild(Guid.Empty, processEntry);
            var fault = Fault.NewGeneric(msg.Message, ex, projectionGroupException.HandlerType, GetSagaId(msg.Message));
            Publisher.Publish(fault, metadata);
        }
    }
}
