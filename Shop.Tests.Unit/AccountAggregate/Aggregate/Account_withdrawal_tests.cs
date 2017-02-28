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
    public class Account_withdrawal_tests : AggregateCommandsTest<Account, AccountCommandsHandler>
    {
        [SetUp]
        public async Task When_account_withdrawal()
        {
            Init();
            _initialAmount = Aggregate.Amount;

            _command = new PayForOrderCommand(Aggregate.Id, new Money(12), Guid.NewGuid());
            await Execute(_command);
        }

        protected override IEnumerable<DomainEvent> Given()
        {
            yield return new AccountCreated(Aggregate.Id, Guid.NewGuid(), 34);
            yield return new AccountReplenish(Aggregate.Id, Guid.NewGuid(), new Money(100));
        }

        protected override IEnumerable<DomainEvent> Expected()
        {
            yield return new AccountWithdrawal(Aggregate.Id, _command.Id, _command.Amount);
        }

        private PayForOrderCommand _command;
        private Money _initialAmount;

        [Test]
        public void Amount_should_be_increased()
        {
            Assert.AreEqual(_initialAmount - _command.Amount, Aggregate.Amount);
        }

        [Test]
        public void When_withdraw_too_much_Not_enough_money_error_is_occured()
        {
            var cmd = new PayForOrderCommand(Aggregate.Id, new Money(12000), Guid.NewGuid());

            Assert.Throws<NotEnoughMoneyException>(() => Execute(cmd));
        }
    }
}