using System;
using GridDomain.CQRS;

namespace BusinessNews.Domain.BusinessAggregate
{
    public class RegisterNewBusinessCommand : Command
    {
        public RegisterNewBusinessCommand(Guid businessId, string name, Guid accountId)
        {
            BusinessId = businessId;
            Name = name;
            AccountId = accountId;
        }

        public Guid BusinessId {get;}
        public Guid AccountId {get;}
        public string Name { get; }
    }
}