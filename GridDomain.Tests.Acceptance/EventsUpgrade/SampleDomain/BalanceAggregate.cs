using System;
using GridDomain.EventSourcing.Sagas.FutureEvents;
using GridDomain.Tests.Acceptance.EventsUpgrade.SampleDomain.Events;

namespace GridDomain.Tests.Acceptance.EventsUpgrade.SampleDomain
{
    public class BalanceAggregate : Aggregate
    {
        private BalanceAggregate(Guid id) : base(id)
        {
            
        }

        public  BalanceAggregate(Guid id, decimal value):this(id)
        {
            RaiseEvent(new AggregateCreatedEvent(value,id));
        }

        public void ChangeState(int number)
        {
            RaiseEvent(new BalanceChangedEvent(number, Id));
        }

        private void Apply(AggregateCreatedEvent e)
        {
            Id = e.SourceId;
            Amount = e.Value;
        }
        
        private void Apply(BalanceChangedEvent e)
        {
            Amount += e.AmountChange;
        }

        public decimal Amount;

    }
}