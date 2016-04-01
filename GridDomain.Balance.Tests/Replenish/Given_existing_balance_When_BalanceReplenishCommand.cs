using System;
using System.Collections.Generic;
using GridDomain.Balance.Commands;
using GridDomain.Balance.Domain;
using GridDomain.CQRS;
using GridDomain.Domain.Tests;
using GridDomain.Domain.Tests.Commanding;
using GridDomain.EventSourcing;
using NUnit.Framework;

namespace GridDomain.Balance.Tests.Replenish
{
    [TestFixture]
    public class Given_existing_balance_When_BalanceReplenishCommand: CommandSpecification<ReplenishBalanceCommand>
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
