using System;
using System.Collections.Generic;
using BusinessNews.Domain.AccountAggregate;
using BusinessNews.Domain.AccountAggregate.Commands;
using BusinessNews.Domain.AccountAggregate.Events;
using GridDomain.CQRS;
using GridDomain.EventSourcing;
using GridDomain.Tests;
using NUnit.Framework;

namespace BusinessNews.Test.Replenish
{
    [TestFixture]
    public class Given_existing_balance_When_AccountReplenishCommand : CommandSpecification<ReplenishAccountByCardCommand>
    {
        protected override ICommandHandler<ReplenishAccountByCardCommand> Handler => new AccountCommandsHandler(Repository);
        private readonly Guid businessId = Guid.NewGuid();

        protected override IEnumerable<DomainEvent> Given()
        {
            //account already exists
            yield return new AccountCreatedEvent(Command.AccountId, businessId);
        }

        protected override IEnumerable<DomainEvent> Expected()
        {
            yield return new AccountBalanceReplenishEvent(Command.AccountId, Command.Amount);
        }

        [Then]
        public void Balance_Should_Be_Replenished()
        {
            VerifyExpected();
        }
    }
}