using System;
using NMoneys;
using Shop.Domain.Aggregates.AccountAggregate.Events;
using Shop.ReadModel.Context;
using Xunit;

namespace Shop.Tests.Unit.AccountAggregate.ProjectionBuilder
{
    public class Account_transactions_projection_tests : Account_projection_builder_test
    {
        private readonly AccountReplenish _msgReplenish;
        private readonly AccountWithdrawal _msgWithdrawal;

        public Account_transactions_projection_tests()
        {
            // Given_account_created_and_projecting()
            var msgCreated = new AccountCreated(Guid.NewGuid(), Guid.NewGuid(), 42);
            var user = new User {Id = msgCreated.UserId, Login = "test"};

            _msgReplenish = new AccountReplenish(msgCreated.SourceId, Guid.NewGuid(), new Money(100));
            _msgWithdrawal = new AccountWithdrawal(msgCreated.SourceId, Guid.NewGuid(), new Money(30));

            using (var context = CreateContext())
            {
                context.Users.Add(user);
                context.SaveChanges();
            }

            ProjectionBuilder.Handle(msgCreated).Wait();
            ProjectionBuilder.Handle(_msgReplenish).Wait();
            ProjectionBuilder.Handle(_msgWithdrawal).Wait();
        }

        [Fact]
        public void Should_create_entries_for_replenish()
        {
            using (var context = CreateContext())
            {
                var history = context.TransactionHistory.Find(_msgReplenish.ChangeId);
                Assert.NotNull(history);
            }
        }

        [Fact]
        public void Should_create_entries_for_withdrawal()
        {
            using (var context = CreateContext())
            {
                var history = context.TransactionHistory.Find(_msgWithdrawal.ChangeId);
                Assert.NotNull(history);
            }
        }

        [Fact]
        public void Should_fill_fields_for_replenish()
        {
            using (var context = CreateContext())
            {
                var history = context.TransactionHistory.Find(_msgReplenish.ChangeId);
                Assert.Equal(_msgReplenish.SourceId, history.AccountId);
                Assert.Equal(_msgReplenish.Amount.Amount, history.ChangeAmount);
                Assert.Equal(_msgReplenish.CreatedTime, history.Created);
                Assert.Equal(_msgReplenish.Amount.CurrencyCode.ToString(), history.Currency);
                Assert.Equal(0, history.InitialAmount);
                Assert.Equal(_msgReplenish.Amount.Amount, history.NewAmount);
                Assert.Equal(AccountOperations.Replenish, history.Operation);
                Assert.Equal(_msgReplenish.ChangeId, history.TransactionId);
//                Assert.Equal(1, history.TransactionNumber);
            }
        }

        [Fact]
        public void Should_fill_fields_for_withdrawal()
        {
            using (var context = CreateContext())
            {
                var history = context.TransactionHistory.Find(_msgWithdrawal.ChangeId);

                Assert.Equal(_msgWithdrawal.SourceId, history.AccountId);
                Assert.Equal(_msgWithdrawal.Amount.Amount, history.ChangeAmount);
                Assert.Equal(_msgWithdrawal.CreatedTime, history.Created);
                Assert.Equal(_msgWithdrawal.Amount.CurrencyCode.ToString(), history.Currency);
                Assert.Equal(_msgReplenish.Amount.Amount, history.InitialAmount);
                Assert.Equal((_msgReplenish.Amount - _msgWithdrawal.Amount).Amount, history.NewAmount);
                Assert.Equal(AccountOperations.Withdrawal, history.Operation);
                Assert.Equal(_msgWithdrawal.ChangeId, history.TransactionId);
                Assert.Equal(2, history.TransactionNumber);
            }
        }

        [Fact]
        public void Should_update_modified_time_for_account()
        {
            using (var context = CreateContext())
            {
                var account = context.Accounts.Find(_msgWithdrawal.SourceId);
                Assert.Equal(_msgWithdrawal.CreatedTime, account.LastModified);
            }
        }
    }
}