using System;
using System.Collections.Generic;
using GridDomain.Balance.Domain.AccountAggregate;
using GridDomain.Balance.Domain.AccountAggregate.Commands;
using GridDomain.Balance.Domain.AccountAggregate.Events;
using GridDomain.CQRS;
using GridDomain.EventSourcing;
using GridDomain.Tests;
using NUnit.Framework;

namespace GridDomain.Balance.Tests.Replenish
{
    [TestFixture]
    public class Given_existing_balance_When_AccountReplenishCommand : CommandSpecification<ReplenishAccountCommand>
    {
        protected override ICommandHandler<ReplenishAccountCommand> Handler => new AccountCommandsHandler(Repository);
        private readonly Guid businessId = Guid.NewGuid();

        protected override IEnumerable<DomainEvent> Given()
        {
            //account already exists
            yield return new AccountCreatedEvent(Command.BalanceId, businessId);
        }

        protected override IEnumerable<DomainEvent> Expected()
        {
            yield return new AccountBalanceReplenishEvent(Command.BalanceId, Command.Amount);
        }

        [Then]
        public void Balance_Should_Be_Replenished()
        {
            VerifyExpected();
        }
    }
}