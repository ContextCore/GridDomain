using GridDomain.Balance.Domain.BalanceAggregate;
using GridDomain.Balance.Domain.BalanceAggregate.Commands;
using GridDomain.CQRS;
using GridDomain.Tests;
using NUnit.Framework;

namespace GridDomain.Balance.Tests.Withdrawal
{
    [TestFixture]
    public class Given_non_existing_balance_When_BalanceWithdrawalCommand :
        CommandSpecification<WithdrawalBalanceCommand>
    {
        protected override ICommandHandler<WithdrawalBalanceCommand> Handler => new BalanceCommandsHandler(Repository);


        [Then]
        public void Exception_is_thrown()
        {
            Assert.Throws<BalanceNotFoundException>(() => ExecuteCommand());
        }
    }
}