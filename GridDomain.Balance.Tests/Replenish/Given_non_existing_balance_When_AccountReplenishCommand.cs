using BusinessNews.Domain.AccountAggregate;
using BusinessNews.Domain.AccountAggregate.Commands;
using GridDomain.CQRS;
using GridDomain.Tests;
using NUnit.Framework;

namespace BusinessNews.Test.Replenish
{
    [TestFixture]
    public class Given_non_existing_balance_When_AccountReplenishCommand :
        CommandSpecification<ReplenishAccountByCardCommand>
    {
        protected override ICommandHandler<ReplenishAccountByCardCommand> Handler
            => new AccountCommandsHandler(Repository);

        [Then]
        public void Exception_is_thrown()
        {
            Assert.Throws<AccountNotFoundException>(() => ExecuteCommand());
        }
    }
}