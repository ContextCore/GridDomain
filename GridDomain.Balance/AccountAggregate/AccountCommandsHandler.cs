using System;
using BusinessNews.Domain.AccountAggregate.Commands;
using CommonDomain.Persistence;
using GridDomain.CQRS;
using GridDomain.Logging;

namespace BusinessNews.Domain.AccountAggregate
{
    [Obsolete("Use AggregateCommandsHandler<Account> instead")]
    public class AccountCommandsHandler : ICommandHandler<ReplenishAccountByCardCommand>,
        ICommandHandler<PayForBillCommand>,
        ICommandHandler<CreateAccountCommand>

    {
        private readonly ISoloLogger _log = LogManager.GetLogger();

        private readonly IRepository _repository;

        public AccountCommandsHandler(IRepository repository)
        {
            _repository = repository;
        }

        public void Handle(CreateAccountCommand msg)
        {
            _log.Debug("Handling command: {Command}", msg.ToPropsString());
            _repository.Save(new Account(msg.AccountId, msg.BusinessId), msg.Id);
        }

        public void Handle(PayForBillCommand msg)
        {
            _log.Debug("Handling command:  {Command}", msg.ToPropsString());
            var account = LoadAccount(msg.AccountId, msg.Id);
            account.PayBill(msg.Amount, msg.BillId);
            _repository.Save(account, msg.Id);
        }

        public void Handle(ReplenishAccountByCardCommand msg)
        {
            _log.Debug("Handling command: {Command}", msg.ToPropsString());
            var account = LoadAccount(msg.AccountId, msg.Id);
            account.Replenish(msg.Amount);
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