using System;
using Akka.Actor;
using Akka.Persistence;

namespace GridDomain.Tests.Acceptance.EventsUpgrade.SampleDomain
{
    public class PersistEventsSaveActor : PersistentActor
    {
        public PersistEventsSaveActor(string persistenteId)
        {
            PersistenceId = persistenteId;
        }
      // public PersistEventsSaveActor()
      // {
      //     CommandAny(o => PersistEvent(o));
      //     RecoverAny(o => RecoverEvent(o));
      // }

        private void RecoverEvent(object o)
        {
            int c = 1;
        }

        protected override bool AroundReceive(Receive receive, object message)
        {
            return base.AroundReceive(receive, message);
        }

        protected override void OnRecoveryFailure(Exception reason, object message = null)
        {
            base.OnRecoveryFailure(reason, message);
        }

        protected override void Unhandled(object message)
        {
            base.Unhandled(message);
        }

        protected override bool ReceiveRecover(object message)
        {
            return true;
        }

        protected override bool ReceiveCommand(object message)
        {
            PersistEvent(message);
            return true;
        }

        protected override void OnPersistFailure(Exception cause, object @event, long sequenceNr)
        {
            base.OnPersistFailure(cause, @event, sequenceNr);
        }

        protected override void OnPersistRejected(Exception cause, object @event, long sequenceNr)
        {
            base.OnPersistRejected(cause, @event, sequenceNr);
        }

        private void PersistEvent(object o)
        {
            Persist(o, e => { });
        }

        public override string PersistenceId { get; }
    }
}