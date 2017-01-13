using System;
using System.Threading.Tasks;
using GridDomain.CQRS;
using GridDomain.CQRS.Messaging;
using Shop.Domain.Aggregates.AccountAggregate.Events;
using Shop.ReadModel.Context;

namespace Shop.ReadModel
{

    public class AccountProjectionBuilder : IHandler<AccountCreated>,
                                            IHandler<AccountReplenish>,
                                            IHandler<AccountWithdrawal>
    {
        private readonly Func<ShopDbContext> _contextFactory;
        public AccountProjectionBuilder(Func<ShopDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task Handle(AccountCreated msg)
        {
            using (var context = _contextFactory())
            {
                var user = await context.Users.FindAsync(msg.UserId);
                var account = new Account()
                {
                    Created = msg.CreatedTime,
                    Id = msg.SourceId,
                    LastModified = msg.CreatedTime,
                    Login = user.Login,
                    Number = msg.AccountNumber,
                    UserId = msg.UserId
                };
                context.Accounts.Add(account);
                await context.SaveChangesAsync();
            }
        }

        public async Task Handle(AccountReplenish msg)
        {
            using (var context = _contextFactory())
            {
                var account = await context.Accounts.FindAsync(msg.SourceId);
                account.LastModified = msg.CreatedTime;
                var initialAmount = account.Amount;
                account.Amount += msg.Amount.Amount;

                var transaction = new AccountTransaction()
                {
                    AccountId = msg.SourceId,
                    ChangeAmount = msg.Amount.Amount,
                    Created = msg.CreatedTime,
                    Currency = msg.Amount.CurrencyCode.ToString(),
                    InitialAmount = initialAmount,
                    NewAmount = account.Amount,
                    Operation = AccountOperations.Replenish,
                    TransactionId = msg.ChangeId
                };

                context.TransactionHistory.Add(transaction);
                await context.SaveChangesAsync();
            }
        }

        public async Task Handle(AccountWithdrawal msg)
        {
            using (var context = _contextFactory())
            {
                var account = await context.Accounts.FindAsync(msg.SourceId);

                var initialAmount = account.Amount;
                account.LastModified = msg.CreatedTime;
                account.Amount -= msg.Amount.Amount;

                var transaction = new AccountTransaction()
                {
                    AccountId = msg.SourceId,
                    ChangeAmount = msg.Amount.Amount,
                    Created = msg.CreatedTime,
                    Currency = msg.Amount.CurrencyCode.ToString(),
                    InitialAmount = initialAmount,
                    NewAmount = account.Amount,
                    Operation = AccountOperations.Withdrawal,
                    TransactionId = msg.ChangeId
                };

                context.TransactionHistory.Add(transaction);
                await context.SaveChangesAsync();
            }
        }
    }
}