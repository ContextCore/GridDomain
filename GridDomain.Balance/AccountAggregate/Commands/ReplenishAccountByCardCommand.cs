using System;
using NMoneys;

namespace BusinessNews.Domain.AccountAggregate.Commands
{
    public class ReplenishAccountByCardCommand : ChangeAccountCommand
    {
        public ReplenishAccountByCardCommand(Guid accountId, Money amount)
            : base(accountId, amount)
        {
        }
    }
}