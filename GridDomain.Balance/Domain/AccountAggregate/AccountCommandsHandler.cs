using System;
using CommonDomain.Persistence;
using GridDomain.Balance.Domain.AccountAggregate.Commands;
using GridDomain.CQRS;
using GridDomain.Logging;
using NLog;

namespace GridDomain.Balance.Domain.AccountAggregate
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

        public void Handle(CreateAccountCommand cmd)
        {
            _log.Debug("Handling command:" + cmd.ToPropsString());
            _repository.Save(new Account(cmd.BalanceId, cmd.BusinessId), cmd.Id);
        }

        public void Handle(ReplenishAccountByCardCommand cmd)
        {
            _log.Debug("Handling command:" + cmd.ToPropsString());
            var account = LoadAccount(cmd.BalanceId, cmd.Id);
            account.Replenish(cmd.Amount);
            _repository.Save(account, Guid.NewGuid());
        }

        public void Handle(PayForBillCommand cmd)
        {
            _log.Debug("Handling command:" + cmd.ToPropsString());
            var account = LoadAccount(cmd.BalanceId, cmd.Id);
            account.PayBill(cmd.Amount, cmd.BillId);
            _repository.Save(account, cmd.Id);
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