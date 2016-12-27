using System;
using System.Collections.Generic;
using GridDomain.EventSourcing;
using GridDomain.Tests.Framework;
using NMoneys;
using NUnit.Framework;
using Ploeh.AutoFixture;
using Shop.Domain.Aggregates.AccountAggregate;
using Shop.Domain.Aggregates.AccountAggregate.Commands;
using Shop.Domain.Aggregates.AccountAggregate.Events;

namespace Shop.Tests.Unit.AccountAggregate.Aggregate
{
    [TestFixture]
    public class Account_replenish_tests : AggregateCommandsTest<Account,AccountCommandsHandler>
    {
        protected override IEnumerable<DomainEvent> Given()
        {
            yield return new AccountCreatedEvent(Aggregate.Id,Guid.NewGuid());
            yield return new AccountReplenish(Aggregate.Id, new Money(100));
        }

        private ReplenishAccountByCardCommand _command;
        private Money _initialAmount;

        [OneTimeSetUp]
        public void When_account_replenish()
        {
            Init();
            _command = new ReplenishAccountByCardCommand(Aggregate.Id, new Money(12), "xxx123");
            _initialAmount = Aggregate.Amount;

            Execute(_command);
        }

        protected override IEnumerable<DomainEvent> Expected()
        {
            yield return new AccountReplenish(Aggregate.Id, _command.Amount);
        }

        [Test]
        public void Then_amount_should_be_increased()
        {
            Assert.AreEqual(_initialAmount + _command.Amount, Aggregate.Amount);
        }
    }
}