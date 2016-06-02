using System;
using GridDomain.CQRS;

namespace GridDomain.Balance.Domain.BalanceAggregate.Commands
{
    public class CreateAccountCommand : Command
    {
        public CreateAccountCommand(Guid balanceId, Guid businessId)
        {
            BalanceId = balanceId;
            BusinessId = businessId;
        }

        public Guid BalanceId { get; private set; }
        public Guid BusinessId { get; set; }
    }
}