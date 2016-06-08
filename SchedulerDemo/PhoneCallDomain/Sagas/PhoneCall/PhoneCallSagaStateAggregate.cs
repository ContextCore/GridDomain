using System;
using GridDomain.EventSourcing.Sagas;
using SchedulerDemo.PhoneCallDomain.Events;

namespace SchedulerDemo.PhoneCallDomain.Sagas.PhoneCall
{
    public class PhoneCallSagaStateAggregate : SagaStateAggregate<PhoneCallSaga.States,PhoneCallSaga.Transitions>
    {
        public PhoneCallSagaStateAggregate(Guid id) : base(id)
        {
        }

        public PhoneCallSagaStateAggregate(Guid id, PhoneCallSaga.States state) : base(id, state)
        {
        }

        public Guid InitiatorId { get; private set; }

        public Guid AbonentId { get; private set; }

        public DateTime? ConversationStartedAt { get; private set; }
        public DateTime? CallInitiatedAt { get; private set; }

        public void Apply(CallInitiatedEvent evt)
        {
            InitiatorId = evt.Initiator;
            AbonentId = evt.Abonent;
            CallInitiatedAt = DateTime.UtcNow;
        }
    }
}