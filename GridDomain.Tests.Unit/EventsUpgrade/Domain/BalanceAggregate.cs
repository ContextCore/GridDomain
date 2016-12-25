using System;
using GridDomain.EventSourcing;
using GridDomain.Tests.Unit.EventsUpgrade.Domain.Events;

namespace GridDomain.Tests.Unit.EventsUpgrade.Domain
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
            RaiseEvent(new BalanceChangedEvent_V1(number, Id));
        }

        public void ChangeStateInFuture(DateTime when, int number, bool oldVersion = false)
        {
            if(oldVersion)
                RaiseEvent(when, new BalanceChangedEvent_V0(number, Id));
            else
                RaiseEvent(when, new BalanceChangedEvent_V1(number,Id));
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

        public decimal Amount;

    }
}