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
    public class Account_withdrawal_tests : AggregateCommandsTest<Account,AccountCommandsHandler>
    {
        protected override IEnumerable<DomainEvent> Given()
        {
            yield return new AccountCreatedEvent(Aggregate.Id,Guid.NewGuid(),34);
            yield return new AccountReplenish(Aggregate.Id, new Money(100));
        }

        [SetUp]
        public void When_account_withdrawal()
        {
            Init();
            _initialAmount = Aggregate.Amount;

            _command = new PayForBillCommand(Aggregate.Id, new Money(12), Guid.NewGuid());
            Execute(_command);
        }

        protected override IEnumerable<DomainEvent> Expected()
        {
           yield return new AccountWithdrawal(Aggregate.Id, _command.Amount);
        }

        private PayForBillCommand _command;
        private Money _initialAmount;


        [Test]
        public void When_withdraw_too_much_Not_enough_money_error_is_occured()
        {
            var cmd = new PayForBillCommand(Aggregate.Id, new Money(12000), Guid.NewGuid());

            Assert.Throws<NotEnoughMoneyException>(() => Execute(cmd));
        }

        [Test]
        public void Amount_should_be_increased()
        {
            Assert.AreEqual(_initialAmount - _command.Amount, Aggregate.Amount);
        }
    }
}