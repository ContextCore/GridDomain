using System;
using BusinessNews.Domain.AccountAggregate.Commands;
using CommonDomain.Persistence;
using GridDomain.CQRS;
using GridDomain.Logging;
using NLog;

namespace BusinessNews.Domain.AccountAggregate
{
    [Obsolete("Use AggregateCommandsHandler<Account> instead")]
    public class AccountCommandsHandler : ICommandHandler<ReplenishAccountByCardCommand>,
        ICommandHandler<PayForBillCommand>,
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
            _repository.Save(new Account(e.AccountId, e.BusinessId), e.Id);
        }

        public void Handle(PayForBillCommand e)
        {
            _log.Debug("Handling command:" + e.ToPropsString());
            var account = LoadAccount(e.AccountId, e.Id);
            account.PayBill(e.Amount, e.BillId);
            _repository.Save(account, e.Id);
        }

        public void Handle(ReplenishAccountByCardCommand e)
        {
            _log.Debug("Handling command:" + e.ToPropsString());
            var account = LoadAccount(e.AccountId, e.Id);
            account.Replenish(e.Amount);
            _repository.Save(account, Guid.NewGuid());
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