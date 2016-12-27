using System;
using System.Collections.Generic;
using GridDomain.EventSourcing;
using GridDomain.Tests.Framework;
using NMoneys;
using NUnit.Framework;
using Ploeh.AutoFixture;
using Shop.Domain.Aggregates.AccountAggregate;
using Shop.Domain.Aggregates.AccountAggregate.Events;

namespace Shop.Tests.Unit.AccountAggregate.Aggregate
{
    [TestFixture]
    public class Account_hydration_tests: AggregateTest<Account>
    {
        private AccountCreatedEvent _createdEvent;
        private AccountReplenish _initialAmountEvent;

        protected override IEnumerable<DomainEvent> Given()
        {
            yield return _createdEvent = Data.Create<AccountCreatedEvent>();
            yield return _initialAmountEvent = new AccountReplenish(_createdEvent.SourceId, new Money(100));
        }

        [SetUp]
        public void Given_account_with_initial_amount()
        {
            Init();
        }

        [Test]
        public void Initial_amount_should_be_taken_from_replenish_event()
        {
            Assert.AreEqual(_initialAmountEvent.Amount, Aggregate.Amount);
        }

        [Test]
        public void Id_should_be_taken_from_created_event()
        {
            Assert.AreEqual(_createdEvent.SourceId, Aggregate.Id);
        }

        [Test]
        public void After_replenish_amount_is_increased_hydration()
        {
            var amount = new Money(123);
            var replenishEvent = new AccountReplenish(_createdEvent.SourceId,
                                                    amount);
            Aggregate.ApplyEvents(replenishEvent);

            Assert.AreEqual(_initialAmountEvent.Amount + amount,
                            Aggregate.Amount);
        }

        [Test]
        public void After_withdraw_amount_is_decreased_hydration()
        {
            var amount = new Money(12);
            var withdrawEvent = new AccountWithdrawal(_createdEvent.SourceId,
                                                      amount);

            Aggregate.ApplyEvents(withdrawEvent);

            Assert.AreEqual(_initialAmountEvent.Amount - amount,
                            Aggregate.Amount);
        }
    }
}
