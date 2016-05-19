using System;
using System.Collections.Generic;
using GridDomain.Balance.Domain;
using GridDomain.Balance.Domain.BalanceAggregate;
using GridDomain.Balance.Domain.BalanceAggregate.Commands;
using GridDomain.Balance.Domain.BalanceAggregate.Events;
using GridDomain.CQRS;
using GridDomain.EventSourcing;
using GridDomain.Tests;
using NUnit.Framework;

namespace GridDomain.Balance.Tests.Replenish
{
    [TestFixture]
    public class Given_existing_balance_When_BalanceReplenishCommand : CommandSpecification<ReplenishBalanceCommand>
    {
        protected override ICommandHandler<ReplenishBalanceCommand> Handler => new BalanceCommandsHandler(Repository);

        protected override IEnumerable<DomainEvent> Given()
        {
            //balance already exists
            yield return new BalanceCreatedEvent(Command.BalanceId, Guid.NewGuid());
        }

        protected override IEnumerable<DomainEvent> Expected()
        {
            yield return new BalanceReplenishEvent(Command.BalanceId, Command.Amount);
        }

        [Then]
        public void Balance_Should_Be_Replenished()
        {
            VerifyExpected();
        }
    }
}