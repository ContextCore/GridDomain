using System;
using System.Collections.Generic;
using GridDomain.EventSourcing;
using GridDomain.Tests.Framework;
using NMoneys;
using Ploeh.AutoFixture;
using Shop.Domain.Aggregates.AccountAggregate;
using Shop.Domain.Aggregates.AccountAggregate.Events;
using Xunit;

namespace Shop.Tests.Unit.XUnit.AccountAggregate.Aggregate
{
    public class Account_hydration_tests : AggregateTest<Account>
    {
        public Account_hydration_tests()// Given_account_with_initial_amount()
        {
            Init();
        }

        private AccountCreated _created;
        private AccountReplenish _initialAmountEvent;

        protected override IEnumerable<DomainEvent> Given()
        {
            yield return _created = Data.Create<AccountCreated>();
            yield return _initialAmountEvent = new AccountReplenish(_created.SourceId, Guid.NewGuid(), new Money(100));
        }

        [Fact]
        public void After_replenish_amount_is_increased_hydration()
        {
            var amount = new Money(123);
            var replenishEvent = new AccountReplenish(_created.SourceId, Guid.NewGuid(), amount);
            Aggregate.ApplyEvents(replenishEvent);

            Assert.Equal(_initialAmountEvent.Amount + amount, Aggregate.Amount);
        }

        [Fact]
        public void After_withdraw_amount_is_decreased_hydration()
        {
            var amount = new Money(12);
            var withdrawEvent = new AccountWithdrawal(_created.SourceId, Guid.NewGuid(), amount);

            Aggregate.ApplyEvents(withdrawEvent);

            Assert.Equal(_initialAmountEvent.Amount - amount, Aggregate.Amount);
        }

        [Fact]
        public void Id_should_be_taken_from_created_event()
        {
            Assert.Equal(_created.SourceId, Aggregate.Id);
        }

        [Fact]
        public void Initial_amount_should_be_taken_from_replenish_event()
        {
            Assert.Equal(_initialAmountEvent.Amount, Aggregate.Amount);
        }
    }
}