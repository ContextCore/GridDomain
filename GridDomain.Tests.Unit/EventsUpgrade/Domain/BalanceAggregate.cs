using System;
using System.Collections.Generic;
using GridDomain.EventSourcing;
using GridDomain.Node.Configuration.Composition;
using GridDomain.Scheduling;
using GridDomain.Tests.Unit.EventsUpgrade.Domain.Commands;
using GridDomain.Tests.Unit.EventsUpgrade.Domain.Events;

namespace GridDomain.Tests.Unit.EventsUpgrade.Domain
{

    public class BalanceAggregateDescriptor : IAggregateCommandsHandlerDescriptor
    {
        public IReadOnlyCollection<Type> RegisteredCommands { get; }
        public Type AggregateType { get; } = typeof(BalanceAggregate);
    }
    
    public class BalanceAggregate : FutureEventsAggregate
    {
        public decimal Amount;

        private BalanceAggregate(string id) : base(id)
        {
            Execute<ChangeBalanceCommand>(c => ChangeState(c.Parameter));

            Execute<CreateBalanceCommand>(c =>  Emit(new AggregateCreatedEvent(c.Parameter, id)));

            Execute<ChangeBalanceInFuture>(c => ChangeStateInFuture(c.RaiseTime, c.Parameter, c.UseLegacyEvent));
        }

        public BalanceAggregate(string id, decimal value) : this(id)
        {
            Emit(new AggregateCreatedEvent(value, id));
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
                Emit(new BalanceChangedEvent_V0(number, Id),when);
            }
            else
            {
                Emit(new BalanceChangedEvent_V1(number, Id),when);
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