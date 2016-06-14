using System;
using GridDomain.EventSourcing.Sagas;
using SchedulerDemo.PhoneCallDomain.Events;

namespace SchedulerDemo.PhoneCallDomain.Sagas.PhoneCall
{
    public class PhoneCallSagaFactory : ISagaFactory<PhoneCallSaga, CallInitiatedEvent>,
        ISagaFactory<PhoneCallSaga, PhoneCallSagaStateAggregate>,
        IEmptySagaFactory<PhoneCallSaga>
    {
        public PhoneCallSaga Create(CallInitiatedEvent message)
        {
            return new PhoneCallSaga(new PhoneCallSagaStateAggregate(Guid.Empty));
        }

        public PhoneCallSaga Create(PhoneCallSagaStateAggregate message)
        {
            return new PhoneCallSaga(message);
        }

        public PhoneCallSaga Create()
        {
            return Create(new PhoneCallSagaStateAggregate(Guid.Empty));
        }
    }
}