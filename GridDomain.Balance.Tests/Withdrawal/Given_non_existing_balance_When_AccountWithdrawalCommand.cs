using GridDomain.Balance.Domain.AccountAggregate;
using GridDomain.Balance.Domain.AccountAggregate.Commands;
using GridDomain.CQRS;
using GridDomain.Tests;
using NUnit.Framework;

namespace GridDomain.Balance.Tests.Withdrawal
{
    [TestFixture]
    public class Given_non_existing_balance_When_AccountWithdrawalCommand :
        CommandSpecification<PayForBillCommand>
    {
        protected override ICommandHandler<PayForBillCommand> Handler => new AccountCommandsHandler(Repository);


        [Then]
        public void Exception_is_thrown()
        {
            Assert.Throws<AccountNotFoundException>(() => ExecuteCommand());
        }
    }
}