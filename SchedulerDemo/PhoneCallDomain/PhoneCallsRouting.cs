using GridDomain.CQRS.Messaging;
using GridDomain.CQRS.Messaging.MessageRouting;
using SchedulerDemo.PhoneCallDomain.Aggregates.Person;
using SchedulerDemo.PhoneCallDomain.Sagas.PhoneCall;

namespace SchedulerDemo.PhoneCallDomain
{
    public class PhoneCallsRouting : IMessageRouteMap
    {
        public void Register(IMessagesRouter router)
        {
            router.RegisterAggregate<Person,PersonAggregateCommandHandler>();
            router.RegisterSaga(PhoneCallSaga.SagaDescriptor);
        }
    }
}
