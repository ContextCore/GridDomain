using System;
using System.Threading.Tasks;
using GridDomain.EventSourcing;
using GridDomain.Tests.XUnit.EventsUpgrade.Domain.Events;

namespace GridDomain.Tests.XUnit.EventsUpgrade.Domain
{
    public class BalanceAggregate : Aggregate
    {
        public decimal Amount;

        private BalanceAggregate(Guid id) : base(id) {}

        public BalanceAggregate(Guid id, decimal value) : this(id)
        {
            RaiseEvent(new AggregateCreatedEvent(value, id));
        }

        public async Task ChangeState(int number)
        {
            await Emit(new BalanceChangedEvent_V1(number, Id));
        }

        public async Task ChangeStateInFuture(DateTime when, int number, bool oldVersion = false)
        {
            if (oldVersion) await Emit(new BalanceChangedEvent_V0(number, Id), when);
            else await Emit(new BalanceChangedEvent_V1(number, Id), when);
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