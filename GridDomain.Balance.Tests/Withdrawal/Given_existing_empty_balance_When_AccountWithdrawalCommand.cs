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
        CommandSpecification<PayForBillCommand>
    {
        protected override ICommandHandler<PayForBillCommand> Handler => new AccountCommandsHandler(Repository);
        private readonly Guid businessId = Guid.NewGuid();
        private readonly Guid billId = Guid.NewGuid();

        protected override IEnumerable<DomainEvent> Given()
        {
            yield return new AccountCreatedEvent(Command.BalanceId, businessId);
        }

        protected override IEnumerable<DomainEvent> Expected()
        {
            yield return new PayedForBillEvent(Command.BalanceId, Command.Amount, billId);
        }

        [Then]
        public void Balance_amount_should_be_increased()
        {
            VerifyExpected();
        }
    }
}