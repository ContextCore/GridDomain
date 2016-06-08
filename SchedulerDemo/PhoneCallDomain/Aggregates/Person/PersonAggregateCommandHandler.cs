using System;
using GridDomain.CQRS.Messaging.MessageRouting;
using SchedulerDemo.PhoneCallDomain.Commands;

namespace SchedulerDemo.PhoneCallDomain.Aggregates.Person
{
    public class PersonAggregateCommandHandler : AggregateCommandsHandler<Person>
    {
        public PersonAggregateCommandHandler()
        {
            Map<AnswerToCallCommand>(c=>c.AbonentId, c =>
            {
                throw new NotImplementedException();
            });

            Map<HangupCommand>(c=>c.AbonentId, c =>
            {
                throw new NotImplementedException();
            });

            Map<InitiateCallCommand>(c=>c.InitiatorId, c =>
            {
                throw new NotImplementedException();
            });

            Map<ReceiveCallCommand>(c=>c.AbonentId, c =>
            {
                throw new NotImplementedException();
            });
        }
    }
}