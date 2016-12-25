using System;
using GridDomain.CQRS;

namespace BusinessNews.Domain.AccountAggregate.Commands
{
    public class CreateAccountCommand : Command
    {
        public CreateAccountCommand(Guid accountId, Guid businessId)
        {
            AccountId = accountId;
            BusinessId = businessId;
        }

        public Guid AccountId { get; }
        public Guid BusinessId { get;}
    }
}