using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GridDomain.EventSourcing;
using GridDomain.Tests.Framework;
using NMoneys;
using Shop.Domain.Aggregates.AccountAggregate;
using Shop.Domain.Aggregates.AccountAggregate.Commands;
using Shop.Domain.Aggregates.AccountAggregate.Events;
using Xunit;

namespace Shop.Tests.Unit.XUnit.AccountAggregate.Aggregate
{
    public class Account_withdrawal_tests : AggregateCommandsTest<Account, AccountCommandsHandler>
    {
        public Account_withdrawal_tests()// When_account_withdrawal()
        {
            Init();
            _initialAmount = Aggregate.Amount;

            _command = new PayForOrderCommand(Aggregate.Id, new Money(12), Guid.NewGuid());
            Execute(_command).Wait();
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

        [Fact]
        public void Amount_should_be_increased()
        {
            Assert.Equal(_initialAmount - _command.Amount, Aggregate.Amount);
        }

        [Fact]
        public async Task When_withdraw_too_much_Not_enough_money_error_is_occured()
        {
            var cmd = new PayForOrderCommand(Aggregate.Id, new Money(12000), Guid.NewGuid());

           await Assert.ThrowsAsync<NotEnoughMoneyException>(() => Execute(cmd));
        }
    }
}