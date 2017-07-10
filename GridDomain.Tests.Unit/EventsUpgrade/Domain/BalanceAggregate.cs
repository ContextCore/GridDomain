using System;
using GridDomain.EventSourcing;
using GridDomain.Node.Configuration.Composition;
using GridDomain.Scheduling;
using GridDomain.Tests.Unit.EventsUpgrade.Domain.Events;

namespace GridDomain.Tests.Unit.EventsUpgrade.Domain
{
 
    public class BalanceAggregate : FutureEventsAggregate
    {
        public decimal Amount;

        private BalanceAggregate(Guid id) : base(id) {}

        public BalanceAggregate(Guid id, decimal value) : this(id)
        {
            Produce(new AggregateCreatedEvent(value, id));
        }

        public void ChangeState(int number)
        {
             Produce(new BalanceChangedEvent_V1(number, Id));
        }

        public void ChangeStateInFuture(DateTime when, int number, bool oldVersion = false)
        {
            if (oldVersion)
                Emit(new BalanceChangedEvent_V0(number, Id), when);
            else
                Emit(new BalanceChangedEvent_V1(number, Id), when);
        }

        private void Apply(AggregateCreatedEvent e)
        {
            Id = e.SourceId;
            Amount = e.Value;
        }

        private void Apply(BalanceChangedEvent_V1 e)
        {
            Amount += e.AmountChange;
        }
    }
}