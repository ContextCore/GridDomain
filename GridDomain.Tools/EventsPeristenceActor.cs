using System;
using System.Collections.Generic;
using Akka.Actor;
using Akka.Persistence;
using CommonDomain;
using GridDomain.EventSourcing;
using GridDomain.Node.AkkaMessaging;

namespace GridDomain.Tools
{
    class EventsPeristenceActor<T> : ReceivePersistentActor where T : IAggregate
    {
        class LoadFailure : Failure
        {
            private Exception Exception;
            private object Message;

            public LoadFailure(Exception ex, object message)
            {
                Message = message;
                Exception = ex;
            }
        }

        internal class LoadComplete
        {
            public string PersistenceId { get; }

            public object[] Events { get; }

            public LoadComplete(string persistenceId, params object[] events)
            {
                this.PersistenceId = persistenceId;
                Events = events;
            }
        }

        private readonly IActorRef _notifyActor;
        private readonly List<object> _events = new List<object>();

        public EventsPeristenceActor(IActorRef notifyActor,string id)
        {
            _notifyActor = notifyActor;
            PersistenceId = id;

            Recover<DomainEvent>(e => _events.Add(e));
            Recover<RecoveryCompleted>(message =>
            {
                _notifyActor.Tell(new LoadComplete(id,_events.ToArray()));
                _events.Clear();
            });

            CommandAny(m => Persist(m, e => Sender.Tell(new MessagePersisted(m), Self)));
        }

        public class MessagePersisted
        {
            public object Message { get; }

            public MessagePersisted(object message)
            {
                Message = message;
            }
        }

        protected override void OnPersistFailure(Exception cause, object @event, long sequenceNr)
        {
            base.OnPersistFailure(cause, @event, sequenceNr);
            _notifyActor.Tell(new LoadFailure(cause, @event));

        }

        protected override void OnPersistRejected(Exception cause, object @event, long sequenceNr)
        {
            base.OnPersistRejected(cause, @event, sequenceNr);
            _notifyActor.Tell(new LoadFailure(cause, @event));

        }

        protected override void OnRecoveryFailure(Exception reason, object message = null)
        {
            base.OnRecoveryFailure(reason, message);
            _notifyActor.Tell(new LoadFailure(reason,message));
        }
 
        public override string PersistenceId { get; }
    }

  
}