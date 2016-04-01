using System;
using GridDomain.CQRS;

namespace GridDomain.Balance.Commands
{
    public class CreateBalanceCommand : Command
    {
        public Guid BalanceId { get; private set; }
        public Guid BusinessId { get; set; }

        public CreateBalanceCommand(Guid balanceId, Guid businessId)
        {
            BalanceId = balanceId;
            BusinessId = businessId;
        }
    }
}