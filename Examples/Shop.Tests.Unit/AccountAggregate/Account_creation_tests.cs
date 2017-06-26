using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GridDomain.EventSourcing;
using GridDomain.Tests.Common;
using Ploeh.AutoFixture;
using Shop.Domain.Aggregates.AccountAggregate;
using Shop.Domain.Aggregates.AccountAggregate.Commands;
using Shop.Domain.Aggregates.AccountAggregate.Events;
using Xunit;

namespace Shop.Tests.Unit.XUnit.AccountAggregate.AggregateTests
{
    /// <summary>
    /// Tests for Account aggregate creation from command. 
    /// Example for aggregate scenario usage
    /// </summary>
    public class Account_creation_tests
    {
        [Fact]
        public async Task Account_create_by_command()
        {
            var command = new Fixture().Create<CreateAccountCommand>();

            var scenario = await AggregateScenario.New<Account, AccountCommandsHandler>()
                                                  .When(command)
                                                  .Then(new AccountCreated(command.AccountId, command.UserId, command.Number))
                                                  .Run()
                                                  .Check();

            //Aggregate_should_take_id_from_command()
            Assert.Equal(command.AccountId, scenario.Aggregate.Id);
            //Aggregate_should_take_number_from_command()
            Assert.Equal(command.Number, scenario.Aggregate.Number);
            //Aggregate_should_take_user_from_command()
            Assert.Equal(command.UserId, scenario.Aggregate.UserId);
        }
    }
}