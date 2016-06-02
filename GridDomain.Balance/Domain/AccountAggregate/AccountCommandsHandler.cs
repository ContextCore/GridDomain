using System;
using CommonDomain.Persistence;
using GridDomain.Balance.Domain.AccountAggregate.Commands;
using GridDomain.CQRS;
using GridDomain.Logging;
using NLog;

namespace GridDomain.Balance.Domain.AccountAggregate
{
    [Obsolete("Use AggregateCommandsHandler<Account> instead")]
    public class AccountCommandsHandler : ICommandHandler<ReplenishAccountCommand>,
                                          ICommandHandler<WithdrawalAccountCommand>,
                                          ICommandHandler<CreateAccountCommand>

    {
        private readonly ILogger _log = LogManager.GetCurrentClassLogger();

        private readonly IRepository _repository;

        public AccountCommandsHandler(IRepository repository)
        {
            _repository = repository;
        }

        public void Handle(CreateAccountCommand e)
        {
            _log.Debug("Handling command:" + e.ToPropsString());
            _repository.Save(new Account(e.BalanceId, e.BusinessId), e.Id);
        }

        public void Handle(ReplenishAccountCommand e)
        {
            _log.Debug("Handling command:" + e.ToPropsString());
            var account = LoadAccount(e.BalanceId, e.Id);
            account.Replenish(e.Amount);
            _repository.Save(account, Guid.NewGuid());
        }

        public void Handle(WithdrawalAccountCommand e)
        {
            _log.Debug("Handling command:" + e.ToPropsString());
            var account = LoadAccount(e.BalanceId, e.Id);
            account.Withdrawal(e.Amount);
            _repository.Save(account, e.Id);
        }

        private Account LoadAccount(Guid balanceId, Guid commandId)
        {
            var account = _repository.GetById<Account>(balanceId);
            //only aggregate factory can create account with empty ownerId
            if (account.OwnerId == Guid.Empty)
            {
                throw new AccountNotFoundException(balanceId, commandId);
            }

            return account;
        }
    }
}