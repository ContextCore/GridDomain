using System;
using System.Collections.Generic;
using Akka.Actor;
using Akka.Persistence;

namespace GridDomain.Tests.Acceptance.Persistence
{
    class SqlJournalPingActor : PersistentActor
    {
        List<string> _events = new List<string>();
        private readonly IActorRef _notifyActor;

        public SqlJournalPingActor(IActorRef notifyActor)
        {
            _notifyActor = notifyActor;
            var plugin = Akka.Persistence.Persistence.Instance.Apply(Context.System).JournalFor(null);
            plugin.Tell(new object());
        }

        protected override void Unhandled(object message)
        {
            base.Unhandled(message);
        }

        protected override void OnPersistFailure(Exception cause, object @event, long sequenceNr)
        {
            base.OnPersistFailure(cause, @event, sequenceNr);
        }

        protected override void OnRecoveryFailure(Exception reason, object message = null)
        {
            base.OnRecoveryFailure(reason, message);
        }

        protected override void OnPersistRejected(Exception cause, object @event, long sequenceNr)
        {
            base.OnPersistRejected(cause, @event, sequenceNr);
        }

        protected override bool ReceiveRecover(object message)
        {
            if (message is SnapshotOffer)
            {
                _events = (List<string>)((SnapshotOffer)message).Snapshot;
            }
            return true;
        }

        protected override bool ReceiveCommand(object message)
        {
            if (message is SqlJournalPing)
            {
                var m = message as SqlJournalPing;
                _events.Add(m.Payload);
                _notifyActor.Tell(new Persisted() { Payload = m.Payload });
            }
            return true;
        }

        public override string PersistenceId => "test";
    }
}