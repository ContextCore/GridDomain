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

namespace GridDomain.Balance.Tests.Withdrawal
{
    [TestFixture]
    public class Given_existing_empty_balance_When_BalanceWithdrawalCommand :
        CommandSpecification<WithdrawalBalanceCommand>
    {
        protected override ICommandHandler<WithdrawalBalanceCommand> Handler => new BalanceCommandsHandler(Repository);

        //   private Money balanceStartAmount = new Fixture().Create<Money>();
        protected override IEnumerable<DomainEvent> Given()
        {
            yield return new BalanceCreatedEvent(Command.BalanceId, Guid.NewGuid());
        }

        protected override IEnumerable<DomainEvent> Expected()
        {
            yield return new BalanceWithdrawalEvent(Command.BalanceId, Command.Amount);
        }

        [Then]
        public void Balance_amount_should_be_increased()
        {
            VerifyExpected();
        }
    }
}