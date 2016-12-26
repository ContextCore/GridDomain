using System;

namespace BusinessNews.Domain.AccountAggregate
{
    public class AccountNotFoundException : Exception
    {
        public AccountNotFoundException(Guid accountId, Guid cmd) :
            base($"Cannot find balance {accountId}, requested by command {cmd}")
        {
        }
    }
}