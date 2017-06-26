using System;
using System.Threading.Tasks;
using GridDomain.Tests.Common;
using NMoneys;
using Shop.Domain.Aggregates.AccountAggregate;
using Shop.Domain.Aggregates.AccountAggregate.Commands;
using Shop.Domain.Aggregates.AccountAggregate.Events;
using Xunit;

namespace Shop.Tests.Unit.XUnit.AccountAggregate.AggregateTests
{
    public class Account_Commands_tests
    {
        [Fact]
        public async Task When_account_replenished_Then_amount_should_be_increased()
        {
            ReplenishAccountByCardCommand command;
            Money initialAmount;
            var scenario = AggregateScenario.New<Account, AccountCommandsHandler>();

            await scenario.Given(new AccountCreated(scenario.Aggregate.Id, Guid.NewGuid(), 123),
                           new AccountReplenish(scenario.Aggregate.Id, Guid.NewGuid(), initialAmount = new Money(100)))
                     .When(command = new ReplenishAccountByCardCommand(scenario.Aggregate.Id, new Money(12), "xxx123"))
                     .Then(new AccountReplenish(scenario.Aggregate.Id, command.Id, command.Amount))
                     .Run()
                     .Check();

            Assert.Equal(initialAmount + command.Amount, scenario.Aggregate.Amount);
        }

        [Fact]
        public async Task When_pay_from_account_Then_amount_should_be_decreased()
        {
            PayForOrderCommand command;
            Money initialAmount;
            var scenario = AggregateScenario.New<Account,AccountCommandsHandler>();
            var id = Guid.NewGuid();

            await scenario.Given(new AccountCreated(id, Guid.NewGuid(), 34),
                           new AccountReplenish(id, Guid.NewGuid(), initialAmount = new Money(100)))
                    .When(command = new PayForOrderCommand(id, new Money(12), Guid.NewGuid()))
                    .Then(new AccountWithdrawal(id, command.Id, command.Amount))
                    .Run()
                    .Check();

            Assert.Equal(initialAmount - command.Amount, scenario.Aggregate.Amount);
        }

        [Fact]
        public async Task When_withdraw_too_much_Not_enough_money_error_is_occured()
        {
            var scenario = AggregateScenario.New<Account,AccountCommandsHandler>();
            await scenario.Given(new AccountCreated(scenario.Aggregate.Id, Guid.NewGuid(), 34),
                                 new AccountReplenish(scenario.Aggregate.Id, Guid.NewGuid(), new Money(100)))
                          .When(new PayForOrderCommand(scenario.Aggregate.Id, new Money(10000), Guid.NewGuid()))
                          .Run()
                          .ShouldThrow<NotEnoughMoneyException>();
        }


    }
}