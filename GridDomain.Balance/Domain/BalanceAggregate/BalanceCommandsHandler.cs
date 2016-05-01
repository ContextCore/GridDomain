using System;
using CommonDomain.Persistence;
using GridDomain.Balance.Commands;
using GridDomain.CQRS;
using GridDomain.Logging;
using NLog;

namespace GridDomain.Balance
{
    public class BalanceCommandsHandler : ICommandHandler<ReplenishBalanceCommand>,
                                          ICommandHandler<WithdrawalBalanceCommand>,
                                          ICommandHandler<CreateBalanceCommand>

    {
        private readonly ILogger _log = LogManager.GetCurrentClassLogger();

        private readonly IRepository _repository;

        public BalanceCommandsHandler(IRepository repository)
        {
            _repository = repository;
        }

        public void Handle(CreateBalanceCommand e)
        {
            _log.Debug("Handling command:" + e.ToPropsString());
            _repository.Save(new Domain.MoneyBalance(e.BalanceId, e.BusinessId), e.Id);
        }

        public void Handle(ReplenishBalanceCommand e)
        {
            _log.Debug("Handling command:" + e.ToPropsString());
            var balance = LoadBalance(e.BalanceId, e.Id);
            balance.Replenish(e.Amount);
            _repository.Save(balance, Guid.NewGuid());
        }

        public void Handle(WithdrawalBalanceCommand e)
        {
            _log.Debug("Handling command:" + e.ToPropsString());
            var balance = LoadBalance(e.BalanceId, e.Id);
            balance.Withdrawal(e.Amount);
            _repository.Save(balance, e.Id);
        }

        private Domain.MoneyBalance LoadBalance(Guid balanceId, Guid commandId)
        {
            var balance = _repository.GetById<Domain.MoneyBalance>(balanceId);
            //only aggregate factory can create balance with empty ownerId
            if (balance.OwnerId == Guid.Empty)
            {
                throw new BalanceNotFoundException(balanceId, commandId);
            }

            return balance;
        }
    }
}