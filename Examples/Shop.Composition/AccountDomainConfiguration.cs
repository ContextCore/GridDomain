using GridDomain.Configuration;
using GridDomain.Node.Configuration.Composition;
using Shop.Domain.Aggregates.AccountAggregate;
using Account = Shop.Domain.Aggregates.AccountAggregate.Account;

namespace Shop.Composition
{
    class AccountDomainConfiguration : IDomainConfiguration {
        public void Register(IDomainBuilder builder)
        {
            builder.RegisterAggregate(DefaultAggregateDependencyFactory.New<Account,AccountCommandsHandler>());
        }
    }
}