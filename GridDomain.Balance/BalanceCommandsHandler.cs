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

        public void Handle(ReplenishBalanceCommand command)
        {
            _log.Debug("Handling command:" + command.ToPropsString());
            var balance = LoadBalance(command.BalanceId, command.Id);
            balance.Replenish(command.Amount);
            _repository.Save(balance,Guid.NewGuid());
        }

        private Domain.Balance LoadBalance(Guid balanceId, Guid commandId)
        {
            var balance = _repository.GetById<Domain.Balance>(balanceId);
            //only aggregate factory can create balance with empty ownerId
            if (balance.OwnerId == Guid.Empty)
            {
                throw new BalanceNotFoundException(balanceId, commandId);
            }

            return balance;
        }

        public void Handle(WithdrawalBalanceCommand command)
        {
            _log.Debug("Handling command:" + command.ToPropsString());
            var balance = LoadBalance(command.BalanceId, command.Id);
            balance.Withdrawal(command.Amount);
            _repository.Save(balance, command.Id);
        }

        public void Handle(CreateBalanceCommand command)
        {
            _log.Debug("Handling command:" + command.ToPropsString());
            _repository.Save(new Domain.Balance(command.BalanceId,command.BusinessId), command.Id);
        }
    }
}