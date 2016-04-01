using GridDomain.Balance.Commands;
using GridDomain.CQRS;
using GridDomain.Domain.Tests;
using GridDomain.Domain.Tests.Commanding;
using NUnit.Framework;

namespace GridDomain.Balance.Tests.Withdrawal
{
    [TestFixture]
    public class Given_non_existing_balance_When_BalanceWithdrawalCommand : CommandSpecification<WithdrawalBalanceCommand>
    {
        protected override ICommandHandler<WithdrawalBalanceCommand> Handler => new BalanceCommandsHandler(Repository);


        [Then]
        public void Exception_is_thrown()
        {
            Assert.Throws<BalanceNotFoundException>(() => ExecuteCommand());
        }
    }
}