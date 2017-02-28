using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GridDomain.EventSourcing;
using GridDomain.Tests.Framework;
using NMoneys;
using NUnit.Framework;
using Shop.Domain.Aggregates.AccountAggregate;
using Shop.Domain.Aggregates.AccountAggregate.Commands;
using Shop.Domain.Aggregates.AccountAggregate.Events;

namespace Shop.Tests.Unit.AccountAggregate.Aggregate
{
    [TestFixture]
    public class Account_replenish_tests : AggregateCommandsTest<Account, AccountCommandsHandler>
    {
        protected override IEnumerable<DomainEvent> Given()
        {
            yield return new AccountCreated(Aggregate.Id, Guid.NewGuid(), 123);
            yield return new AccountReplenish(Aggregate.Id, Guid.NewGuid(), new Money(100));
        }

        private ReplenishAccountByCardCommand _command;
        private Money _initialAmount;

        [OneTimeSetUp]
        public async Task When_account_replenish()
        {
            Init();
            _command = new ReplenishAccountByCardCommand(Aggregate.Id, new Money(12), "xxx123");
            _initialAmount = Aggregate.Amount;

            await Execute(_command);
        }

        protected override IEnumerable<DomainEvent> Expected()
        {
            yield return new AccountReplenish(Aggregate.Id, _command.Id, _command.Amount);
        }

        [Test]
        public void Then_amount_should_be_increased()
        {
            Assert.AreEqual(_initialAmount + _command.Amount, Aggregate.Amount);
        }
    }
}