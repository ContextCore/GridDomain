using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Akka;
using Akka.Actor;
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
                _log.Error(ex);
                _publisher.Publish(Fault.NewGeneric(message,ex.InnerException,ex.Type,GetSagaId(message)));
            }
        }

        //TODO: add custom saga id mapping
        private Guid GetSagaId(object msg)
        {
            ISourcedEvent e = msg as ISourcedEvent;
            if (e != null) return e.SagaId;
            return Guid.Empty;
        }
    }
}
