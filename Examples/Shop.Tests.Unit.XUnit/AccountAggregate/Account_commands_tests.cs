using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GridDomain.EventSourcing;
using GridDomain.Tests.Common;
using NMoneys;
using Ploeh.AutoFixture;
using Shop.Domain.Aggregates.AccountAggregate;
using Shop.Domain.Aggregates.AccountAggregate.Commands;
using Shop.Domain.Aggregates.AccountAggregate.Events;
using Xunit;

namespace Shop.Tests.Unit.XUnit.AccountAggregate.AggregateTests
{
    public class Account_Commands_tests
    {
        [Fact]
        public void When_account_replenished_Then_amount_should_be_increased()
        {
            ReplenishAccountByCardCommand command;
            Money initialAmount;
            var scenario = new AggregateScenario<Account, AccountCommandsHandler>();

            scenario.Given(new AccountCreated(scenario.Id, Guid.NewGuid(), 123),
                           new AccountReplenish(scenario.Id, Guid.NewGuid(), initialAmount = new Money(100)))
                     .When(command = new ReplenishAccountByCardCommand(scenario.Id, new Money(12), "xxx123"))
                     .Then(new AccountReplenish(scenario.Id, command.Id, command.Amount))
                     .Run()
                     .Check();

            Assert.Equal(initialAmount + command.Amount, scenario.Aggregate.Amount);
        }

        [Fact]
        public void When_pay_from_account_Then_amount_should_be_decreased()
        {
            PayForOrderCommand command;
            Money initialAmount;
            var scenario = new AggregateScenario<Account, AccountCommandsHandler>();
            var id = Guid.NewGuid();

            scenario.Given(new AccountCreated(id, Guid.NewGuid(), 34),
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
            var scenario = new AggregateScenario<Account, AccountCommandsHandler>();
            await scenario.Given(new AccountCreated(scenario.Id, Guid.NewGuid(), 34),
                                 new AccountReplenish(scenario.Id, Guid.NewGuid(), new Money(100)))
                          .When(new PayForOrderCommand(scenario.Id, new Money(10000), Guid.NewGuid()))
                          .RunAsync()
                          .ShouldThrow<NotEnoughMoneyException>();
        }


    }
}