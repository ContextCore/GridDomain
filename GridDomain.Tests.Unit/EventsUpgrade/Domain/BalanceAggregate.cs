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

        private BalanceAggregate(string id) : base(id) {}

        public BalanceAggregate(string id, decimal value) : this(id)
        {
            Emit(new[] {new AggregateCreatedEvent(value, id)});
        }
        public BalanceAggregate(Guid id, decimal value) : this(id.ToString(), value)
        {
        }

        public void ChangeState(int number)
        {
            Emit(new[] {new BalanceChangedEvent_V1(number, Id)});
        }

        public void ChangeStateInFuture(DateTime when, int number, bool oldVersion = false)
        {
            if (oldVersion)
            {
                Emit(new BalanceChangedEvent_V0(number, Id),when,null);
            }
            else
            {
                Emit(new BalanceChangedEvent_V1(number, Id),when,null);
            }
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