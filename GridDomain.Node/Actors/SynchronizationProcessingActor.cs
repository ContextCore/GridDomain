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
using GridDomain.Node.AkkaMessaging;

namespace GridDomain.Node.Actors
{ 
    class SynchronizationProcessingActor<T> : UntypedActor where T: IProjectionGroup 
    {
        private readonly T _group;

        public SynchronizationProcessingActor(T group)
        {
            _group = @group;
        }

        protected override void OnReceive(object message)
        {
            _group.Project(message);
        }
    }
}
