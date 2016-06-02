using System;
using NMoneys;

namespace GridDomain.Balance.Domain.AccountAggregate.Commands
{
    public class WithdrawalAccountCommand : ChangeAccountCommand
    {
        public WithdrawalAccountCommand(Guid balanceId, Money amount, AccountChangeSource changeSource)
            : base(balanceId, amount, changeSource)
        {
        }
    }
}