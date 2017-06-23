using System;
using GridDomain.EventSourcing;
using GridDomain.Tests.Framework;
using NMoneys;
using Ploeh.AutoFixture;
using Shop.Domain.Aggregates.AccountAggregate;
using Shop.Domain.Aggregates.AccountAggregate.Events;
using Xunit;

namespace Shop.Tests.Unit.XUnit.AccountAggregate.AggregateTests
{
    /// <summary>
    /// Tests for account aggregate hydration from events 
    /// Example for manual approach for applying events
    /// It is recommended to use hydration tests only for advanced event apply scenarios with big amoutn of logic
    /// </summary>
    public class Account_hydration_tests
    {
        public Account_hydration_tests()
        {
            Account = Aggregate.Empty<Account>();
            _created = Account.ApplyEvent(new Fixture().Create<AccountCreated>());
            _initialAmountEvent = Account.ApplyEvent(new AccountReplenish(_created.SourceId, Guid.NewGuid(), new Money(100)));
        }

        private Account Account { get; }
        private readonly AccountCreated _created;
        private readonly AccountReplenish _initialAmountEvent;

        [Fact]
        public void After_replenish_amount_is_increased_hydration()
        {
            var amount = new Money(123);
            Account.ApplyEvents(new AccountReplenish(_created.SourceId, Guid.NewGuid(), amount));
            Assert.Equal(_initialAmountEvent.Amount + amount, Account.Amount);
        }

        [Fact]
        public void After_withdraw_amount_is_decreased_hydration()
        {
            var amount = new Money(12);
            Account.ApplyEvents(new AccountWithdrawal(_created.SourceId, Guid.NewGuid(), amount));
            Assert.Equal(_initialAmountEvent.Amount - amount, Account.Amount);
        }

        [Fact]
        public void Id_should_be_taken_from_created_event()
        {
            Assert.Equal(_created.SourceId, Account.Id);
        }

        [Fact]
        public void Initial_amount_should_be_taken_from_replenish_event()
        {
            Assert.Equal(_initialAmountEvent.Amount, Account.Amount);
        }
    }
}