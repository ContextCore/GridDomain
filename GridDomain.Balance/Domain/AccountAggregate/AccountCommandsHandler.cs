using System;
using CommonDomain.Persistence;
using GridDomain.Balance.Domain.BalanceAggregate.Commands;
using GridDomain.CQRS;
using GridDomain.CQRS.Messaging.MessageRouting;
using GridDomain.Logging;
using NLog;

namespace GridDomain.Balance.Domain.BalanceAggregate
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
            var balance = LoadBalance(e.BalanceId, e.Id);
            balance.Replenish(e.Amount);
            _repository.Save(balance, Guid.NewGuid());
        }

        public void Handle(WithdrawalAccountCommand e)
        {
            _log.Debug("Handling command:" + e.ToPropsString());
            var balance = LoadBalance(e.BalanceId, e.Id);
            balance.Withdrawal(e.Amount);
            _repository.Save(balance, e.Id);
        }

        private Account LoadBalance(Guid balanceId, Guid commandId)
        {
            var balance = _repository.GetById<Account>(balanceId);
            //only aggregate factory can create balance with empty ownerId
            if (balance.OwnerId == Guid.Empty)
            {
                throw new BalanceNotFoundException(balanceId, commandId);
            }

            return balance;
        }
    }
}