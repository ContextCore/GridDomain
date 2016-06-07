﻿using System;
using GridDomain.EventSourcing;
using NMoneys;

namespace BusinessNews.Domain.AccountAggregate.Events
{
    public class AccountBalanceChangedEvent : DomainEvent
    {
        protected AccountBalanceChangedEvent(Guid balanceId, Money amount) : base(balanceId)
        {
            Amount = amount;
        }

        public Guid BalanceId => SourceId;
        public Money Amount { get; private set; }
    }
}