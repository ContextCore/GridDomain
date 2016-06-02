using System;
using System.Collections.Generic;
using GridDomain.Balance.Domain.AccountAggregate;
using GridDomain.Balance.Domain.AccountAggregate.Commands;
using GridDomain.Balance.Domain.AccountAggregate.Events;
using GridDomain.CQRS;
using GridDomain.EventSourcing;
using GridDomain.Tests;
using NUnit.Framework;

namespace GridDomain.Balance.Tests.Withdrawal
{
    [TestFixture]
    public class Given_existing_empty_balance_When_AccountWithdrawalCommand :
        CommandSpecification<WithdrawalAccountCommand>
    {
        protected override ICommandHandler<WithdrawalAccountCommand> Handler => new AccountCommandsHandler(Repository);
        private readonly Guid businessId = Guid.NewGuid();

        protected override IEnumerable<DomainEvent> Given()
        {
            yield return new AccountCreatedEvent(Command.BalanceId, businessId);
        }

        protected override IEnumerable<DomainEvent> Expected()
        {
            yield return new AccountWithdrawalEvent(Command.BalanceId, Command.Amount);
        }

        [Then]
        public void Balance_amount_should_be_increased()
        {
            VerifyExpected();
        }
    }
}